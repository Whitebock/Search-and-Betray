using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace WhiteNet.Client
{
    public class Client
    {
        #region Attributes
        private TcpClient tcpClient;

        private BinaryWriter writer;

        private bool connected;
        private bool reading;
        private bool listening;
        #endregion

        #region Delegates

        public delegate void ByteEvent(byte[] data);

        #endregion

        #region Events

        private event ByteEvent DataReceived = delegate { };

        #endregion

        #region Properties
        public bool Connected
        {
            get { return connected; }
        }
        public bool Reading
        {
            get { return reading; }
        }
        public bool Listening
        {
            get { return listening; }
        }
        #endregion

        #region Constructors
        public Client()
        {
            connected = false;
            reading = false;
            listening = false;
        }

        #endregion

        #region Connection Methodes
        public void Connect(IPAddress address, int port)
        {
            if (connected)
                throw new Exception("Alread connected to a Server");
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(address, port);
                writer = new BinaryWriter(tcpClient.GetStream());
                connected = true;
            }
            catch (SocketException)
            {
                throw new Exception(String.Format("No Server running at {0}:{1}", address, port));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Disconnect()
        {
            if (!connected)
                throw new Exception("Not connected to a Server");
            tcpClient.Close();
            writer.Close();
            writer = null;
            connected = false;
        }
        #endregion

        #region Send Methodes

        public void Send(byte[] packet)
        {
            if (!connected)
                throw new Exception("Not connected to a Server");

            if (packet.Length > 255)
                throw new ArgumentException("Packet must be smaller than 255 bytes, because the header is 2 bytes long", "packet");

            // Get the length of the packet as header.
            byte[] header = BitConverter.GetBytes((UInt16)packet.Length);

            // Combine header and packet.
            byte[] data = new byte[header.Length + packet.Length];
            header.CopyTo(data, 0);
            packet.CopyTo(data, header.Length);
            
            // Send the data.
            writer.Write(data);
            writer.Flush();
        }

        #endregion

        #region Read Methodes
        public byte[] Read()
        {
            if (reading)
                throw new Exception("Already reading");

            reading = true;

            Stream s = tcpClient.GetStream();

            // Get the header (length of the packet).
            byte[] header = new byte[2];
            s.Read(header, 0, 2);
            UInt16 length = BitConverter.ToUInt16(header, 0);

            // Now read the actual data.
            byte[] packet = new byte[length];
            s.Read(packet, 0, length);

            reading = false;

            return packet;
        }
        public void BeginRead()
        {
            if (!connected)
                throw new Exception("Not connected to a Server");
            if (reading)
                throw new Exception("Already reading");
            reading = true;
            tcpClient.GetStream().BeginRead(new byte[] { 0 }, 0, 0, OnRead, tcpClient);
        }
        public void EndRead()
        {
            reading = false;
        }
        #endregion

        #region Thread Methodes
        private void OnRead(IAsyncResult result)
        {
            try
            {
                Stream s = tcpClient.GetStream();

                // Get the header (length of the packet).
                byte[] header = new byte[2];
                s.Read(header, 0, 2);
                UInt16 length = BitConverter.ToUInt16(header, 0);

                // Now read the actual data.
                byte[] packet = new byte[length];
                s.Read(packet, 0, length);

                DataReceived(packet);

                if (tcpClient.Connected && reading)
                    tcpClient.GetStream().BeginRead(new byte[] { 0 }, 0, 0, OnRead, null);
                else
                    reading = false;
            }
            catch (Exception) { }
        }
        #endregion
    }
}
