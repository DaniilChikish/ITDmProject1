using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeusUtility;
using DeusUtility.Serialization;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.Networking;
using System;

namespace ITDmProject
{
    public class GlobalControllerMobile : MonoBehaviour
    {
        //client
        public NetBroadcastReciever broadcastReciever;
        public NetMessageTransmitter messageTransmitter;
        public List<ServerInfo> Servers;
        //settings
        private SerializeSettingsMobile settings;
        public bool SettingsSaved;
        public Languages Localisation
        {
            get { return settings.localisation; }
            set
            {
                SettingsSaved = false;
                settings.localisation = value;
            }
        }
		//texts
		public Dictionary<string, string> Texts;
        //
        void OnEnable()
        {
            LoadSettings();
            LoadLocalisationTexts();
        }

        public void SaveSettings()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(SerializeSettingsMobile));
            string path = Application.streamingAssetsPath + "/settingsMobile.dat";
            // получаем поток, куда будем записывать сериализованный объект
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                formatter.Serialize(fs, settings);
            }
            SettingsSaved = true;
            Debug.Log(path + " - saved");
        }
        public void SetDefault()
        {
            settings = new SerializeSettingsMobile();
            settings.localisation = Languages.English;

            SaveSettings();
        }
        public void LoadSettings()
        {
            Debug.Log("load settings");

            string path = Application.streamingAssetsPath + "/settingsMobile.dat";
            // передаем в конструктор тип класса
            XmlSerializer formatter = new XmlSerializer(typeof(SerializeSettingsMobile));
            // десериализация
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                settings = (SerializeSettingsMobile)formatter.Deserialize(fs);
                Debug.Log(path + " - ok");
                fs.Close();
            }
            catch (Exception)
            {
                Debug.Log("set default");
                SetDefault();
            }
            SettingsSaved = true;
        }

        private void Start()
        {
            Servers = new List<ServerInfo>();
            RunClient();
        }

        private void Update()
        {

        }
        public void RunClient()
        {
            Servers.Clear();
            if (broadcastReciever)
            {
                Destroy(broadcastReciever);
                Debug.Log("Existing reciever destriyed");
            }
            GameObject body = new GameObject("reciever");
            this.gameObject.AddComponent<NetworkManager>();
			broadcastReciever = body.AddComponent<NetBroadcastReciever>();
        }
        public void ConnectTo(int serverIndex)
        {
            if (!messageTransmitter)
            {
                GameObject body = new GameObject("transmitter");
                //body.AddComponent<NetworkManager>();
                messageTransmitter = body.gameObject.AddComponent<NetMessageTransmitter>();
                Debug.Log("Transmiter created");
			}
            {
            //    Destroy(messageTransmitter);
			//	Debug.Log("Existing transmitter destriyed");
			}
            //if (NetworkManager.singleton.client.isConnected)
            //{
            //    NetworkManager.singleton.client.Disconnect();
            //    Debug.Log("Axisting connection down");
            //}
            NetworkManager.singleton.networkAddress = Servers[serverIndex].Address;
			NetworkManager.singleton.networkPort = Convert.ToInt16(Servers[serverIndex].Port);
			NetworkManager.singleton.StartClient();
        }
        public void Send(string word)
        {
            messageTransmitter.Send(word);
        }

        public void LoadLocalisationTexts()
        {
            string path = Application.streamingAssetsPath + "/local";
            switch (Localisation)
            {
                case Languages.English:
                    {
                        path += "/eng/base_eng.xml";
                        break;
                    }
                case Languages.Russian:
                    {
                        path += "/rus/base_rus.xml";
                        break;
                    }
                case Languages.temp:
                    {
                        path += "/temp.xml";
                        break;
                    }
            }

            //SaveText();//debug only
            //SerializeData<string, string> textsSer = new SerializeData<string, string>();
            // передаем в конструктор тип класса
            XmlSerializer formatter = new XmlSerializer(typeof(SerializeData<string, string>));
            // десериализация
            Debug.Log("open - " + path);
            FileStream fs = new FileStream(path, FileMode.Open);
            SerializeData<string, string> serialeze = (SerializeData<string, string>)formatter.Deserialize(fs);
            serialeze.OnAfterDeserialize();
            Texts = new Dictionary<string, string>();
            Texts = serialeze.Data;
            Debug.Log(Texts.ToString());
		}
    }
}
