using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteNet.Server;

namespace Server.NS_Model
{
    public class CCC_Player
    {
        #region Attibutes
        private string username;
        private ServerClient client;
        #endregion

        #region Properties
        public string Username
        {
            get { return username; }
        }

        public ServerClient Client
        {
            get { return client; }
        }
        #endregion

        #region Constructors
        public CCC_Player(ServerClient client)
        {

        }
        #endregion

        
    }
}
