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
        public List<string> wordList;
        public List<string> stopList;
        public SerializeSettingsDesktop desctopSettings;
        public bool DesctopSettingsSaved;
        public bool Connected;
        public bool SettingsRecieved;
        public bool WordListRecieved;
        public bool StopListRecieved;
        public ServerInfo CurrentConnection
        {
            get
            {
                return new ServerInfo(NetworkManager.singleton.networkAddress, NetworkManager.singleton.networkPort, ServerName);
            }
        }
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
        private Dictionary<string, string> texts;
        public string Texts(string key)
        {

            if (texts.ContainsKey(key))
                return texts[key];
            else return key;
        }
        //
        public string ServerName
        {
            get { return desctopSettings.serverName; }
            set
            {
                DesctopSettingsSaved = false;
                desctopSettings.serverName = value;
            }
        }
        public float Duration
        {
            get { return desctopSettings.duration; }
            set
            {
                DesctopSettingsSaved = false;
                desctopSettings.duration = value;
            }
        }
        public float Delay
        {
            get { return desctopSettings.delay; }
            set
            {
                DesctopSettingsSaved = false;
                desctopSettings.delay = value;
            }
        }
        public int Radius
        {
            get { return desctopSettings.sphereRadius; }
            set
            {
                DesctopSettingsSaved = false;
                desctopSettings.sphereRadius = value;
            }
        }
        public int ObjectRadius
        {
            get { return desctopSettings.objectRadius; }
            set
            {
                DesctopSettingsSaved = false;
                desctopSettings.objectRadius = value;
            }
        }
        public int ObjectsInSphere(int sphereRad, int objRad)
        {
            float outp;
            float V = 3f / 4f * Mathf.PI * Mathf.Pow(sphereRad, 3);
            float v = 3f / 4f * Mathf.PI * Mathf.Pow(objRad, 3);
            outp = Mathf.Round(V / v / 4) - 1;
            if (outp > 1000) return 1000;
            else return Convert.ToInt32(outp);
        }
        //
        private void OnEnable()
        {
            texts = new Dictionary<string, string>();
            Debug.Log("Try to load settings.");
            LoadSettings();
            Debug.Log("Try to load localisation.");
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
            settings.lastServerAddres = "";
            settings.lastServerPort = 0;
            //SaveSettings();
        }
        public void LoadSettings()
        {
            Debug.Log("Load settings");

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
                Debug.Log("Set default");
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
                Debug.Log("Existing reciever destroyed");
            }
            GameObject body = new GameObject("reciever");
            this.gameObject.AddComponent<NetworkManager>();
			broadcastReciever = body.AddComponent<NetBroadcastReciever>(); 
            if (settings.lastServerAddres != "" && settings.lastServerPort != 0)
            {
                ConnectTo(settings.lastServerAddres, settings.lastServerPort);
            }
        }
        public void ConnectTo(string addres, int port)
        {
            Debug.Log("Try to connect");
            Debug.Log("\t addres:" + addres);
            Debug.Log("\t port:" + port);
            Debug.Log("\t name:" + name);
            if (!messageTransmitter)
            {
                GameObject body = new GameObject("transmitter");
                //body.AddComponent<NetworkManager>();
                messageTransmitter = body.gameObject.AddComponent<NetMessageTransmitter>();
                Debug.Log("Transmiter created");
            }
            if (NetworkManager.singleton.isNetworkActive)
                NetworkManager.singleton.StopClient();
            NetworkManager.singleton.networkAddress = addres;
            NetworkManager.singleton.networkPort = port;
            NetworkManager.singleton.StartClient();
        }
        public void OnConnectedToServer()
        {
            Debug.Log("Main Connected.");
            Connected = true;
            settings.lastServerAddres = NetworkManager.singleton.networkAddress;
            settings.lastServerPort = NetworkManager.singleton.networkPort;
            messageTransmitter.OpenConnection();
            GetData();
        }
        private void OnFailedToConnect(NetworkConnectionError error)
        {
            Debug.Log("Faild to connect. " + error.ToString());
        }
        public void ConnectTo(int serverIndex)
        {
            ConnectTo(Servers[serverIndex].Address, Servers[serverIndex].Port);
        }
        public void Send(string word)
        {
            wordList.Add(word);
            messageTransmitter.Send(word);
        }
        public void GetData()
        {
            if (!SettingsRecieved)
                messageTransmitter.GetData(NetMessager.Operation.Settings);
            else if (!WordListRecieved)
                messageTransmitter.GetData(NetMessager.Operation.WordList);
            else if (!StopListRecieved)
                messageTransmitter.GetData(NetMessager.Operation.StopList);
            else return;
        }
        public void GetWordsList()
        {
            messageTransmitter.GetData(NetMessager.Operation.WordList);
        }
        public void PushSettings()
        {
            DesctopSettingsSaved = true;
            messageTransmitter.PushData(NetMessager.Operation.Settings);
        }
        public void PushWordsList()
        {
            messageTransmitter.PushData(NetMessager.Operation.WordList);
        }
        public void PushStopList()
        {
            messageTransmitter.PushData(NetMessager.Operation.StopList);
        }
        public void LoadLocalisationTexts()
        {
            try
            {
                Debug.Log("platform - " + Application.platform);
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        {
                            LoadLocalisationTextsAndroid();
                            break;
                        }
                    default:
                        {
                            LoadLocalisationTextsDefault();
                            break;
                        }
                }
            }
            catch (Exception)
            {
                texts = HardDefaultStorage.GetLocalisationDefault();
            }
        }
        private void LoadLocalisationTextsAndroid()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "local");
            switch (Localisation)
            {
                case Languages.English:
                    {
                        path = Path.Combine(path, "/eng/base_eng.xml");
                        break;
                    }
                case Languages.Russian:
                    {
                        path = Path.Combine(path, "/rus/base_rus.xml");
                        break;
                    }
                case Languages.Default:
                    {
                        path += "/default.xml";
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
            Debug.Log(serialeze.ToString());
            //texts = new Dictionary<string, string>();
            texts = serialeze.Data;
        }
        private void LoadLocalisationTextsDefault()
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
                case Languages.Default:
                    {
                        path += "/default.xml";
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
            Debug.Log(serialeze.ToString());
            //texts = new Dictionary<string, string>();
            texts = serialeze.Data;
        }
        private void OnApplicationQuit()
        {
            Debug.Log("Settings saved on quit.");
            SaveSettings();
        }
    }
}
