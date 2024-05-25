using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace serverApp
{
    
    class Client
    {
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        public string userName { get; set; }
        public int id_dep { get; set; }
        public string dep { get; set; }
        public bool isAuthorized { get; set; }


        public Client(TcpClient client) {

            ClientSocket = client;
            UID = Guid.NewGuid();
            userName = "none";
            isAuthorized = false;
            dep = "all";

        }

    }

}
