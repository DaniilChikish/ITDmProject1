using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace ITDmProject
{
    public class NetMessageReciever : NetworkBehaviour
    {
        private GlobalControllerDesktop Global;

        // Use this for initialization
        void Start()
        {
            Global = FindObjectOfType<GlobalControllerDesktop>();
            //if the client is also the server
            if (NetworkServer.active)
            {
                //registering the server handler
                NetworkServer.RegisterHandler(666, ServerReceiveMessage);
                Global.SetServerUp();
            }
        }
        private void ServerReceiveMessage(NetworkMessage message)
        {
            string word = message.ReadMessage<StringMessage>().value;
            Debug.Log("Recieved - " + word);
            CreateWord(word);
            message.conn.Disconnect();
            //StringMessage myMessage = new StringMessage();
            //we are using the connectionId as player name only to exemplify
            //myMessage.value = "OK";
            //sending to all connected clients
            //NetworkServer.SendToClient(message.conn.connectionId, 666, myMessage);
        }
        public void CreateWord(string word)
        {
            Global.AddNCaptere(word);
        }
    }
}
