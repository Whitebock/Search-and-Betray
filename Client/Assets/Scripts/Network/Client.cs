using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Assets.Scripts.Network
{
    public sealed class Client
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

        public event ByteEvent DataReceived = delegate { };
        public event ByteEvent Timeout = delegate { };
        public event ByteEvent ConnectionLost = delegate { };

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
                throw new Exception("Already connected to a Server");
            try
            {
                tcpClient = new TcpClient();
                tcpClient.ReceiveTimeout = 5000;
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

            if (packet.Length > 65535)
                throw new ArgumentException("Packet must be smaller than 65535 bytes, because the header is 2 bytes long", "packet");

            // Get the length of the packet as header.
            byte[] header = BitConverter.GetBytes((UInt16)packet.Length);

            // Combine header and packet.
            byte[] data = new byte[header.Length + packet.Length];
            header.CopyTo(data, 0);
            packet.CopyTo(data, header.Length);

            // Send the data.
            try
            {
                writer.Write(data);
                writer.Flush();
            }
            catch (SocketException)
            {
                //Disconnect();
                ConnectionLost(new byte[0]);
            }
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
            try
            {
                s.Read(header, 0, 2);
            }
            catch (IOException e)
            {
                // Timeout.
                Timeout(new byte[0]);
                EndRead();
                Disconnect();
                throw new IOException("Read Timeout", e);
            }
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
                byte[] packet = new byte[0];
                try
                {
                    // Get the header (length of the packet).
                    byte[] header = new byte[2];
                    s.Read(header, 0, 2);
                    UInt16 length = BitConverter.ToUInt16(header, 0);

                    // Now read the actual data.
                    packet = new byte[length];
                    s.Read(packet, 0, length);
                }
                catch (IOException)
                {
                    // Timeout.
                    Timeout(new byte[0]);
                    EndRead();
                    Disconnect();
                    return;
                }

                DataReceived(packet);

                tcpClient.GetStream().BeginRead(new byte[] { 0 }, 0, 0, OnRead, null);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion
    }
}
