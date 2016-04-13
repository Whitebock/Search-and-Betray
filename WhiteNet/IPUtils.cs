using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace WhiteNet
{
    public class IPUtils
    { 
        public static string PrettifyAddress(IPAddress address, IPAddress subnetmask = null)
        {
            byte[] addressbytes = address.GetAddressBytes();
            string format = String.Format("{0}.{1}.{2}.{3}", addressbytes[0], addressbytes[1], addressbytes[2], addressbytes[3]);

            if (subnetmask != null)
            {
                byte[] subnetbytes = subnetmask.GetAddressBytes();
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(subnetbytes);

                int i = BitConverter.ToInt32(subnetbytes, 0);
                format += "/" + i;
            }

            return format;
        }
        public static IPAddress GetLocalAddress(bool ipv6 = false)
        {
            IPAddress localhost = null;
            foreach (IPAddress ipaddress in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (ipaddress.AddressFamily == (ipv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork))
                {
                    localhost = ipaddress;
                    break;
                }
            }
            return localhost;
        }

        /// <summary>
        /// Gets your public IP Address by sending an WebRequest
        /// to <see cref="http://ip-api.com"/>
        /// </summary>
        /// <returns>Public IP Address</returns>
        public static IPAddress GetPublicAddress()
        {
            HttpWebResponse response;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://ip-api.com/csv");
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception)
            {
                return null;
            }

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string[] lines = reader.ReadToEnd().Split(',');

            IPAddress result = IPAddress.Parse(lines[lines.Length - 1]);
            return result;
        }

        public static bool ValidIPEndPoint(IPEndPoint e, double waittime = 0.05)
        {
            using (TcpClient tcp = new TcpClient())
            {
                IAsyncResult ar = tcp.BeginConnect(e.Address, e.Port, null, null);
                WaitHandle wh = ar.AsyncWaitHandle;
                try
                {
                    if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(waittime), false))
                    {
                        tcp.Close();
                        return false;
                    }

                    tcp.EndConnect(ar);
                    return true;
                }
                finally
                {
                    wh.Close();
                }
            }
        }

        /// <summary>
        /// Will try to find the IPv4 SubnetMask for the provided <paramref name="address"/>
        /// If the Address is null, it will use the first one it finds
        /// </summary>
        /// <param name="address">Local IP Address</param>
        /// <returns>IP Address representing a subnetmask</returns>
        /// <exception cref="System.ArgumentException">Thrown when the subnetmask cannot be found</exception>
        public static IPAddress GetLocalSubnetMask(IPAddress address = null)
        {
            if (address == null)
                address = GetLocalAddress();

            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastIPAddressInformation.Address))
                        {
                            return unicastIPAddressInformation.IPv4Mask;

                        }
                    }
                }
            }
            throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
        }
    }
}
