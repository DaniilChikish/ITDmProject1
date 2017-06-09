using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace ITDmProject
{
    public class NetMessageTransmitter : NetMessager //modile part
    {
        private GlobalControllerMobile Global;

        // Use this for initialization
        void Start()
        {
			Global = FindObjectOfType<GlobalControllerMobile>();
            NetworkManager.singleton.client.RegisterHandler(MsgType.Connect, ConnectMessageHandler);
        }
        private void OnConnectedToServer()
        {
            Debug.Log("Messager connected");
            if (!Global.Connected)
                Global.OnConnectedToServer();
        }
        private void OnDisconnectedFromServer(NetworkDisconnection info)
        {
            Debug.Log(info);
            Global.Reconnect();
        }
        public void OpenConnection()
        {
			NetworkManager.singleton.client.RegisterHandler(PutWord, PutWordHandler);
            NetworkManager.singleton.client.RegisterHandler(OperationRequest, OperationRequestHandler);
            NetworkManager.singleton.client.RegisterHandler(WordListData, WordListDataHandler);
            NetworkManager.singleton.client.RegisterHandler(StopListData, StopListDataHandler);
            NetworkManager.singleton.client.RegisterHandler(SettingsData, SettingsUpdateHandler);
        }
		private void ConnectMessageHandler(NetworkMessage netMsg)
        {
            OnConnectedToServer();
        }
        private void PutWordHandler(NetworkMessage netMsg)
        {
            string text = netMsg.ReadMessage<StringMessage>().value;
        }
        private void OperationRequestHandler(NetworkMessage netMsg)
        {
            throw new NotImplementedException();
        }
        private void WordListDataHandler(NetworkMessage message)
        {
            Debug.Log("WordList recieved.");
			string content = message.ReadMessage<StringMessage>().value;
			//Debug.Log(content);
			string[] data = content.Split('#');
			List<string> buff = new List<string>(data);
			buff.Remove(buff[0]);
            Global.wordList.Clear();
            Global.wordList.AddRange(buff);
            Global.WordListRecieved = true;
            Global.GetData();
        }
        private void StopListDataHandler(NetworkMessage message)
        {
			Debug.Log("StopList recieved.");
			string content = message.ReadMessage<StringMessage>().value;
			//Debug.Log(content);
			string[] data = content.Split('#');
			List<string> buff = new List<string>(data);
			buff.Remove(buff[0]);
            Global.stopList.Clear();
            Global.stopList.AddRange(buff);
            Global.StopListRecieved = true;
			Global.GetData();
        }
        private void SettingsUpdateHandler(NetworkMessage message)
        {
            Debug.Log("Settings recieved.");
            string content = message.ReadMessage<StringMessage>().value;
            //Debug.Log(content);
            string[] data = content.Split('#');
            Global.Duration = Convert.ToSingle(data[1]);
            Global.Delay = Convert.ToSingle(data[2]);
            Global.Radius = Convert.ToInt32(data[3]);
            Global.ObjectRadius = Convert.ToInt32(data[4]);
            Global.ServerName = data[5];
            Global.SettingsRecieved = true;
            Global.DesctopSettingsSaved = true;
			Global.GetData();
        }
        public void Send(string word)
        {
            StringMessage message = new StringMessage();
            //getting the value of the input
            message.value = word;
            //sending to server
            NetworkManager.singleton.client.Send(PutWord, message);
            Debug.Log(word + " - Sent");
            //Destroy(this);
        }
        public void GetData(Operation oper)
        {
            StringMessage message = new StringMessage();
            switch (oper)
            {
                case Operation.WordList:
                    {
                        message.value = "getWords";
                        break;
                    }
                case Operation.StopList:
                    {
                        message.value = "getStop";
                        break;
                    }
                case Operation.Settings:
                    {
                        message.value = "getSettings";
                        break;
                    }
            }
            //sending to server
            NetworkManager.singleton.client.Send(OperationRequest, message);
            Debug.Log(oper.ToString() + "Request - Sent");
        }
        public void PushData(Operation oper)
        {
            StringMessage data;
            switch (oper)
            {
                case Operation.WordList:
                    {
                        data = WordsListString();
                        NetworkManager.singleton.client.Send(WordListData, data); break;
                    }
                case Operation.StopList:
                    {
                        data = StopListString();
                        NetworkManager.singleton.client.Send(StopListData, data); break;
                    }
                case Operation.Settings:
                    {
                        data = SettingsString();
                        NetworkManager.singleton.client.Send(SettingsData, data); break;
                    }
            }
        }
        private StringMessage WordsListString()
        {
            StringBuilder outp = new StringBuilder("words");
            foreach (string x in Global.wordList)
            {
                outp.Append("#" + x);
            }
			return new StringMessage(outp.ToString());
		}
        private StringMessage StopListString()
        {
            StringBuilder outp = new StringBuilder("stop");
            foreach (string x in Global.stopList)
            {
                outp.Append("#" + x);
            }
			return new StringMessage(outp.ToString());
        }
        private StringMessage SettingsString()
        {
            StringBuilder outp = new StringBuilder("settings");
            outp.Append("#" + Global.Duration);
            outp.Append("#" + Global.Delay);
            outp.Append("#" + Global.Radius);
            outp.Append("#" + Global.ObjectRadius);
            outp.Append("#" + Global.ServerName);
			return new StringMessage(outp.ToString());
		}

        private void OnDestroy()
        {
            NetworkManager.singleton.client.Shutdown();
			//NetworkManager.Shutdown();
			Debug.Log("Transmitter destroyed");
        }
    }
}
