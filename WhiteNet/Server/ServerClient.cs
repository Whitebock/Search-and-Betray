using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WhiteNet.Server
{
    public class ServerClient
    {
        #region Attributes
        private TcpClient tcpClient;
        private IPAddress address;

        private BinaryWriter writer;


        private bool reading;
        #endregion

        #region Properties
        public IPAddress Address
        {
            get { return address; }
        }

        public bool Reading
        {
            get { return reading; }
        }
        #endregion

        #region Delegates

        public delegate void ByteEvent(byte[] data);

        #endregion

        #region Events

        public event ByteEvent DataReceived = delegate { };

        #endregion

        #region Constructors
        public ServerClient(TcpClient tcp)
        {
            this.tcpClient = tcp;
            IPEndPoint endpoint = (IPEndPoint)tcp.Client.LocalEndPoint;
            address = endpoint.Address;

            writer = new BinaryWriter(tcp.GetStream());
            reading = false;
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

        #region Send Methodes
        public void Send(byte[] packet)
        {
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

        public override string ToString()
        {
            return String.Format("ServerClient[{0}]", address);
        }
    }
}
