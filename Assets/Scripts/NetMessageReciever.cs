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
                NetworkServer.RegisterHandler(DeleteWord, DeleteWordHandler);
                NetworkServer.RegisterHandler(AddStopMsg, AddStopHandler);
                NetworkServer.RegisterHandler(DeleteStopMsg, DeleteStopHandler);
                NetworkServer.RegisterHandler(OperationRequest, OperationRequestHandler);
                NetworkServer.RegisterHandler(WordListData, WordListDataHandler);
                NetworkServer.RegisterHandler(StopListData, StopListDataHandler);
				NetworkServer.RegisterHandler(SettingsData, SettingsUpdateHandler);
                NetworkServer.RegisterHandler(DisconnectRequest, DisconnectHandler);
                NetworkServer.RegisterHandler(MsgType.AddPlayer, OnAddPlayer);
				Global.SetServerUp();
            }
        }
        private void PutWordHandler(NetworkMessage message)
        {
            string word = message.ReadMessage<StringMessage>().value;
            Debug.Log("Recieved - Put " + word);
            CreateWordF(word);

        }
		private void DeleteWordHandler(NetworkMessage message)
		{
			string word = message.ReadMessage<StringMessage>().value;
			Debug.Log("Recieved - Delete " + word);
            DeleteWordF(word);
		}
		private void AddStopHandler(NetworkMessage message)
		{
			string word = message.ReadMessage<StringMessage>().value;
			Debug.Log("Recieved - Add Stop " + word);
			AddStopF(word);
		}
		private void DeleteStopHandler(NetworkMessage message)
		{
			string word = message.ReadMessage<StringMessage>().value;
			Debug.Log("Recieved - Delete Stop " + word);
			DeleteStopF(word);
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
            StringMessage data;
            switch (oper)
            {
                case Operation.WordList:
                    {
                        data = WordsListString();
                        NetworkServer.SendToClient(connectionId, WordListData, data); break;
                    }
                case Operation.StopList:
                    {
                        data = StopListString();
                        NetworkServer.SendToClient(connectionId, StopListData, data); break;
                    }
                case Operation.Settings:
                    {
                        data = SettingsString();
                        NetworkServer.SendToClient(connectionId, SettingsData, data); break;
                    }
            }
        }
        private StringMessage WordsListString()
        {
            StringBuilder outp = new StringBuilder("words");
            foreach(string x in Global.keysList)
            {
                outp.Append("#" + x.Split('_')[0]);
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
        private void WordListDataHandler(NetworkMessage message)
        {
            string[] data = message.ReadMessage<StringMessage>().value.Split('#');
            List<string> buff = new List<string>(data);
            buff.Remove(buff[0]);
            Global.SetWords(buff.ToArray());
        }
        private void StopListDataHandler(NetworkMessage message)
        {
            string[] data = message.ReadMessage<StringMessage>().value.Split('#');
			List<string> buff = new List<string>(data);
			buff.Remove(buff[0]);
            Global.SetStopList(buff.ToArray());
        }
        private void SettingsUpdateHandler(NetworkMessage message)
        {
            string[] data = message.ReadMessage<StringMessage>().value.Split('#');      
            Global.Duration = Convert.ToSingle(data[1]);
            Global.Delay = Convert.ToSingle(data[2]);
            Global.Radius = Convert.ToInt32(data[3]);
            Global.ObjectRadius = Convert.ToInt32(data[4]);
            Global.ServerName = data[5];
            Global.SaveSettings();
        }

        private void OnAddPlayer(NetworkMessage netMsg)
		{
            Debug.Log("Client " + netMsg.conn.address + " connected.");
            StringMessage ansver = new StringMessage("OK");
            NetworkServer.SendToClient(netMsg.conn.connectionId, MsgType.Connect, ansver);
		}
        private void DisconnectHandler(NetworkMessage netMsg)
        {
            Debug.Log("Client " + netMsg.conn.address + " disconnecting.");
            netMsg.conn.Disconnect();
        }
        public void CreateWordF(string word)
        {
            Global.AddNCaptere(word);
        }
		public void DeleteWordF(string word)
		{
			Global.RemoveWord(word);
		}
        public void AddStopF(string word)
		{
			Global.AddStop(word);
		}
        public void DeleteStopF(string word)
		{
			Global.RemoveStop(word);
		}
    }
}
