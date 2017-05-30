using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
namespace ITDmProject
{
    public class NetBroadcastReciever : NetworkDiscovery
    {
        GlobalControllerMobile Global;
        void Start()
        {
            Global = FindObjectOfType<GlobalControllerMobile>();
            startClient();
        }

        public void startClient()
        {
            Initialize();
            StartAsClient();
        }

        public override void OnReceivedBroadcast(string fromAddress, string data)
        {
            string[] deparse = data.Split('#');
            Debug.Log("Found " + deparse[1] + "from - " + fromAddress);
            Global.Servers.Add(new ServerInfo(fromAddress, deparse[0], deparse[1]));
        }
    }
    public class ServerInfo
    {
        public string address;
        public string port;
        public string name;
        public string Address { get { return address; } }
        public string Port { get { return port; } }
        public string Name { get { return address; } }
        public ServerInfo(string address, string port, string name)
        {
            this.address = address;
            this.port = port;
            this.name = name;
        }
    }
}
