using System;
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
        #endregion

        #region Delegates

        public delegate void ByteEvent(byte[] data);

        #endregion

        #region Events

        private event ByteEvent dataReceived = delegate { };

        #endregion

        #region Properties

        //Attributes
        public bool Connected
        {
            get { return connected; }
        }
        public bool Reading
        {
            get { return reading; }
        }

        //Events
        public ByteEvent DataReceived
        {
            get { return dataReceived; }
            set { dataReceived = value; }
        }
        #endregion

        #region Constructors
        public Client(TcpClient tcp = null)
        {
            connected = false;
            reading = false;

            if (tcp == null)
                tcpClient = new TcpClient();
            else
                tcpClient = tcp;
        }

        #endregion

        #region Connection Methodes
        public void Connect(IPAddress address, int port)
        {
            if (connected)
                throw new Exception("Already connected to a Server");
            try
            {
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

            writer.Write(packet);
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
            byte[] packet = new byte[s.Length];
            s.Read(packet, 0, (int)s.Length);

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
                byte[] packet = new byte[s.Length];
                s.Read(packet, 0, (int)s.Length);

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
