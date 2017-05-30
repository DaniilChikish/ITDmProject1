using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace ITDmProject
{
    public class NetMessageTransmitter : NetworkBehaviour
    {
        private GlobalControllerMobile Global;

        // Use this for initialization
        void Start()
        {
            Global = FindObjectOfType<GlobalControllerMobile>();
            NetworkManager.singleton.client.RegisterHandler(0, RecieveMessage);
        }

        private void RecieveMessage(NetworkMessage netMsg)
        {
            string text = netMsg.ReadMessage<StringMessage>().value;
        }

        public void SendMessage(string word)
        {
            StringMessage myMessage = new StringMessage();
            //getting the value of the input
            myMessage.value = word;
            //sending to server
            NetworkManager.singleton.client.Send(0, myMessage);
        }
    }
}
