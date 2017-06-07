using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace ITDmProject
{
    public class NetMessageReciever : NetMessager //desctop part
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
                NetworkServer.RegisterHandler(PutWord, PutWordHandler);
                NetworkServer.RegisterHandler(OperationRequest, OperationRequestHandler);
                NetworkServer.RegisterHandler(WordListData, WordListDataHandler);
                NetworkServer.RegisterHandler(StopListData, StopListDataHandler);
                NetworkServer.RegisterHandler(SettingsData, SettingsUpdateHandler);
                NetworkServer.RegisterHandler(MsgType.AddPlayer, OnAddPlayer);
				Global.SetServerUp();
            }
        }
        private void PutWordHandler(NetworkMessage message)
        {
            string word = message.ReadMessage<StringMessage>().value;
            Debug.Log("Recieved - " + word);
            CreateWord(word);

        }
        private void OperationRequestHandler(NetworkMessage message)
        {
            string request = message.ReadMessage<StringMessage>().value;
            Debug.Log("Recieved - " + request);
            switch (request)
            {
                case "getWords":
                    {
                        SendAnswer(Operation.WordList, message.conn.connectionId);
                        break;
                    }
                case "getStop":
                    {
                        SendAnswer(Operation.StopList, message.conn.connectionId);
                        break;
                    }
                case "getSettings":
                    {
                        SendAnswer(Operation.Settings, message.conn.connectionId);
                        break;
                    }
            }
        }
        private void SendAnswer(Operation oper, int connectionId)
        {
            StringMessage message = new StringMessage();
            string data;
            switch (oper)
            {
                case Operation.WordList:
                    {
                        data = WordsListString();
                        NetworkServer.SendToClient(connectionId, WordListData, message); break;
                    }
                case Operation.StopList:
                    {
                        data = StopListString();
                        NetworkServer.SendToClient(connectionId, StopListData, message); break;
                    }
                case Operation.Settings:
                    {
                        data = SettingsString();
                        NetworkServer.SendToClient(connectionId, SettingsData, message); break;
                    }
            }
        }
        private string WordsListString()
        {
            StringBuilder outp = new StringBuilder("words");
            foreach(string x in Global.keysList)
            {
                outp.Append("#" + x);
            }
            return outp.ToString();
        }
        private string StopListString()
        {
            StringBuilder outp = new StringBuilder("stop");
            foreach (string x in Global.stopList)
            {
                outp.Append("#" + x);
            }
            return outp.ToString();
        }
        private string SettingsString()
        {
            StringBuilder outp = new StringBuilder("settings");
            outp.Append("#" + Global.Duration);
            outp.Append("#" + Global.Delay);
            outp.Append("#" + Global.Radius);
            outp.Append("#" + Global.ObjectRadius);
            return outp.ToString();
        }
        private void WordListDataHandler(NetworkMessage message)
        {
            string[] data = message.ReadMessage<StringMessage>().value.Split('#');
            Global.SetWords(data);
        }
        private void StopListDataHandler(NetworkMessage message)
        {
            string[] data = message.ReadMessage<StringMessage>().value.Split('#');
            Global.SetStopList(data);
        }
        private void SettingsUpdateHandler(NetworkMessage message)
        {
            string[] data = message.ReadMessage<StringMessage>().value.Split('#');      
            Global.Duration = Convert.ToSingle(data[1]);
            Global.Delay = Convert.ToSingle(data[2]);
            Global.Radius = Convert.ToInt32(data[3]);
            Global.ObjectRadius = Convert.ToInt32(data[4]);
            Global.SaveSettings();
        }

        private void OnAddPlayer(NetworkMessage netMsg)
		{
            Debug.Log("Resieve 'AddPlayer' Msg");
		}

        public void CreateWord(string word)
        {
            Global.AddNCaptere(word);
        }
    }
}
