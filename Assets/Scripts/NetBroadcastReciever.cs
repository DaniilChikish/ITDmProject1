using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
namespace ITDmProject
{
    public class NetBroadcastReciever : NetworkDiscovery //mobile part
    {
        GlobalControllerMobile Global;
        void Start()
        {
            Global = FindObjectOfType<GlobalControllerMobile>();
            showGUI = false;
            startClient();
        }

        public void startClient()
        {
            Initialize();
            StartAsClient();
            Debug.Log("Broadcast client started");
        }
        private void OnConnectedToServer()
        {
			Debug.Log("Connected");
        }
        public override void OnReceivedBroadcast(string fromAddress, string data)
        {
            string[] deparse = data.Split('#');
            //Debug.Log("Recieve " + data);
            //Debug.Log("from " + fromAddress);
            ServerInfo finded = new ServerInfo(fromAddress, Convert.ToInt32(deparse[0]), deparse[1]);
            if (!Global.Servers.Exists(x => x.Address == fromAddress))
                Global.Servers.Add(finded);
        }
        private void OnDestroy()
        {
            //if (NetworkManager.singleton.isActiveAndEnabled)
        }
    }
    public class ServerInfo
    {
        private string address;
        private int port;
        private string name;
        public string Address { get { return address; } }
        public int Port { get { return port; } }
        public string Name { get { return name; } }
        public ServerInfo(string address, int port, string name)
        {
            this.address = address;
            this.port = port;
            this.name = name;
        }
    }
}
