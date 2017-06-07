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
        }
        public void OpenConnection()
        {
			NetworkManager.singleton.client.RegisterHandler(PutWord, PutWordHandler);
            NetworkManager.singleton.client.RegisterHandler(OperationRequest, OperationRequestHandler);
            NetworkManager.singleton.client.RegisterHandler(WordListData, WordListDataHandler);
            NetworkManager.singleton.client.RegisterHandler(StopListData, StopListDataHandler);
            NetworkManager.singleton.client.RegisterHandler(SettingsData, SettingsUpdateHandler);
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
            string[] data = message.ReadMessage<StringMessage>().value.Split('#');
            Global.keysList.Clear();
            Global.keysList.AddRange(data);
        }
        private void StopListDataHandler(NetworkMessage message)
        {
            string[] data = message.ReadMessage<StringMessage>().value.Split('#');
            Global.stopList.Clear();
            Global.stopList.AddRange(data);
        }
        private void SettingsUpdateHandler(NetworkMessage message)
        {
            string[] data = message.ReadMessage<StringMessage>().value.Split('#');
            Global.Duration = Convert.ToSingle(data[1]);
            Global.Delay = Convert.ToSingle(data[2]);
            Global.Radius = Convert.ToInt32(data[3]);
            Global.ObjectRadius = Convert.ToInt32(data[4]);
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
        public void PutData(Operation oper)
        {
            StringMessage message = new StringMessage();
            string data;
            switch (oper)
            {
                case Operation.WordList:
                    {
                        data = WordsListString();
                        NetworkManager.singleton.client.Send(WordListData, message); break;
                    }
                case Operation.StopList:
                    {
                        data = StopListString();
                        NetworkManager.singleton.client.Send(StopListData, message); break;
                    }
                case Operation.Settings:
                    {
                        data = SettingsString();
                        NetworkManager.singleton.client.Send(SettingsData, message); break;
                    }
            }
        }
        private string WordsListString()
        {
            StringBuilder outp = new StringBuilder("words");
            foreach (string x in Global.keysList)
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

        private void OnDestroy()
        {
            NetworkManager.singleton.client.Shutdown();
			//NetworkManager.Shutdown();
			Debug.Log("Transmitter destroyed");
        }
    }
}
