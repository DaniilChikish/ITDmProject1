using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DeusUtility;
using DeusUtility.UI;
using DeusUtility.Serialization;
namespace ITDmProject
{
    public class GlobalControllerDesktop : MonoBehaviour
    {
        public Dictionary<string, Transform> wordObjs;
        public GameObject wordPrefab;
        private MotionController motor;
        public Material pointMat;
        private int iter;
        public List<string> keys; //debug only
        private bool serverUp;
        public bool ServerUp { get { return serverUp; }}
        private GameObject server;
        private NetworkManager manager;
        //storage
        private Stack<Vector3> randPositions;
        private Stack<Quaternion> randRotations;
        //settings
        private SerializeSettingsDesktop settings;
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
        public string ServerName
        {
            get { return settings.serverName; }
            set
            {
                SettingsSaved = false;
                settings.serverName = value;
            }
        }
        public float Duration
        {
            get { return settings.duration; }
            set
            {
                SettingsSaved = false;
                motor.duration = value;
                settings.duration = value;
            }
        }
        public float Delay
        {
            get { return settings.delay; }
            set
            {
                SettingsSaved = false;
                motor.delay = value;
                settings.delay = value;
            }
        }
        public int Radius
        {
            get { return settings.sphereRadius; }
            set
            {
                SettingsSaved = false;
                settings.sphereRadius = value;
            }
        }
        public int ObjectRadius
        {
            get { return settings.objectRadius; }
            set
            {
                SettingsSaved = false;
                settings.objectRadius = value;
            }
        }
        public int MaxPoints
        {
            get
            {
                float outp;
                float V = 3f / 4f * Mathf.PI * Mathf.Pow(Radius, 3);
                float v = 3f / 4f * Mathf.PI * Mathf.Pow(ObjectRadius, 3);
                outp = Mathf.Round(V / v / 4) - 1;
                if (outp > 1000) return 1000;
                else return Convert.ToInt32(outp);
            }
        }
        public int ObjectsInSphere(int sphereRad, int objRad)
        {
            float outp;
            float V = 3f / 4f * Mathf.PI * Mathf.Pow(Radius, 3);
            float v = 3f / 4f * Mathf.PI * Mathf.Pow(ObjectRadius, 3);
            outp = Mathf.Round(V / v / 4) - 1;
            if (outp > 1000) return 1000;
            else return Convert.ToInt32(outp);
        }
        public Vector2 Resolution
        {
            get { return new Vector2(settings.screenExpWidth, settings.screenExpHeight); }
            set { SettingsSaved = false; settings.screenExpWidth = value.x; settings.screenExpHeight = value.y; }
        }
        //texts
        private Dictionary<string, string> texts;
		public string Texts(string key)
		{
			if (texts.ContainsKey(key))
				return texts[key];
            else return key;
		}
        // Use this for initialization
        void OnEnable()
        {
            wordObjs = new Dictionary<string, Transform>();
            keys = new List<string>();
            randPositions = new Stack<Vector3>();
            randRotations = new Stack<Quaternion>();
            texts = new Dictionary<string, string>();
            iter = 0;
            Debug.Log("Try to load settings.");
            LoadSettings();
            Debug.Log("Try to load localisation.");
            LoadLocalisationTexts();
        }

        public void SaveSettings()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(SerializeSettingsDesktop));
            string path = Application.streamingAssetsPath + "/settings.dat";
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
            settings = new SerializeSettingsDesktop();
            settings.localisation = Languages.English;
            settings.serverName = ValidString.ReplaceChar(Environment.MachineName, '#', '_');
            //settings.musicLevel = 50;
            //settings.soundLevel = 50;
            settings.sphereRadius = 30;
            settings.objectRadius = 5;
            settings.delay = 3;
            settings.duration = 1.5f;
            SaveSettings();
        }
        public void LoadSettings()
        {
            Debug.Log("load settings");

            string path = Application.streamingAssetsPath + "/settings.dat";
            // передаем в конструктор тип класса
            XmlSerializer formatter = new XmlSerializer(typeof(SerializeSettingsDesktop));
            // десериализация
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                settings = (SerializeSettingsDesktop)formatter.Deserialize(fs);
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
            motor = GameObject.FindObjectOfType<MotionController>();
            RunServerBroadcast();
        }

        public void RunServerBroadcast()
        {
            FullStorage();
            LoadWords();
            motor.duration = Duration;
            motor.delay = Delay;

            //
            if (server)
                Destroy(server);
            server = new GameObject("server");
            server.AddComponent<NetBroadcastTransmitter>();
            //manager = server.AddComponent<NetworkManager>();
            //manager.StartServer(new ConnectionConfig());
        }
        public void RunServerMessanger()
        {
            server.AddComponent<NetMessageReciever>();
        }
        public void SetServerUp()
        {
			serverUp = true;
            Debug.Log("Up = " + ServerUp);
        }
        public void ServerDown()
        {
            serverUp = false;
            server.GetComponent<NetBroadcastTransmitter>().Down();
            Destroy(server);
        }

        private void Update()
        {
            if (ServerUp)
            {
                if (motor.targets.Count == 0)
                {
                    Capture(keys[iter]);
                    iter++;
                    if (iter >= keys.Count)
                        iter = 0;
                    //Debug.Log ("delta:" + Time.deltaTime);
                }
            }
        }

        public void LoadLocalisationTexts()
        {
			try
			{           
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
                //Texts = new Dictionary<string, string>();
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
			Debug.Log(serialeze.ToString());
			//Texts = new Dictionary<string, string>();
			texts = serialeze.Data;
		}

        private void FullStorage()
        {
            int maxP = MaxPoints;
            GameObject mainSphere = new GameObject("mainSphere");
            mainSphere.transform.position = Vector3.zero;

            double[] randPosXArr = Randomizer.Uniform(-Radius, Radius, maxP * 6);
            double[] randPosYArr = Randomizer.Uniform(-Radius, Radius, maxP * 6);
            double[] randPosZArr = Randomizer.Uniform(-Radius, Radius, maxP * 6);

            double[] randRotXArr = Randomizer.Uniform(0, 360, maxP);
            double[] randRotYArr = Randomizer.Uniform(0, 360, maxP);
            double[] randRotZArr = Randomizer.Uniform(0, 360, maxP);

            randPositions.Clear();
            randRotations.Clear();
            GameObject point;
            int i = 0, j = 0;
            while (maxP > 0)
            {
                Vector3 position = new Vector3(0, 0, 0);
                Quaternion rotation = new Quaternion();
                do
                {
                    position.x = Convert.ToSingle(randPosXArr[i]);
                    position.y = Convert.ToSingle(randPosYArr[i]);
                    position.z = Convert.ToSingle(randPosZArr[i]);
                    i++;
                } while ((Vector3.Distance(position, this.transform.position) > Radius) || (Physics.OverlapSphere(position, ObjectRadius).Length != 0));

                rotation.x = Convert.ToSingle(randRotXArr[j]);
                rotation.y = Convert.ToSingle(randRotYArr[j]);
                rotation.z = Convert.ToSingle(randRotZArr[j]);
                j++;

                randPositions.Push(position);
                randRotations.Push(rotation);

                point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                point.transform.SetParent(mainSphere.transform);
                point.transform.position = position;
                point.transform.localScale = Vector3.one * ObjectRadius;
                point.GetComponent<MeshRenderer>().material = pointMat;

                maxP--;
            }
        }
        private void LoadWords()
        {
            Clear();

            string path = Application.streamingAssetsPath + "/words.dat";
            string[] words = File.ReadAllLines(path);

            CreateWords(words);
        }

        public void SaveWords()
        {
            //string[] wordArr = wordObjs.Keys;
        }
        public void Clear()
        {
            wordObjs.Clear();
            keys.Clear();
            iter = 0;
            if (this.transform.childCount > 0)
            {
                GameObject child;
                for (int i = this.transform.childCount - 1; i >= 0; i--)
                {
                    child = this.transform.GetChild(i).gameObject;
                    Destroy(child);
                }
            }
        }

        //public void CreateWords(string[] words)
        //{
        //    double[] randPosArr = Randomizer.Uniform(-Radius, Radius, words.Length * 9);
        //    double[] randRotArr = Randomizer.Uniform(0, 360, words.Length * 3);
        //    int i = 0;
        //    int j = 0;
        //    string ID;
        //    Vector3 position = new Vector3(0, 0, 0);
        //    Quaternion rotation = new Quaternion();
        //    Transform parent = this.gameObject.transform;
        //    foreach (string w in words)
        //    {
        //        do
        //        {
        //            position.x = Convert.ToSingle(randPosArr[i]);
        //            position.y = Convert.ToSingle(randPosArr[i + 1]);
        //            position.z = Convert.ToSingle(randPosArr[i + 2]);
        //            i += 1;
        //        } while ((Vector3.Distance(position, this.transform.position) > Radius) && (Physics.OverlapSphere(position, 7).Length == 0));
        //        i += 2;
        //        rotation.x = Convert.ToSingle(randRotArr[j]);
        //        rotation.y = Convert.ToSingle(randRotArr[j + 1]);
        //        rotation.z = Convert.ToSingle(randRotArr[j + 2]);
        //        j += 3;

        //        ID = w + "_" + position.GetHashCode().ToString();

        //        GameObject inW = Instantiate(wordPrefab, position, rotation, parent);

        //        inW.GetComponent<Word>().Instant(ID, w);
        //        keys.Add(ID); //debug
        //        wordObjs.Add(ID, inW.transform);
        //    }
        //}
        private string CreateWord(string text)
        {
            string ID = "";
            if (randPositions.Count == 0 || randRotations.Count == 0)
            { Debug.Log("Storage empty"); }
            else
            {
                ID = text + "_" + randPositions.Peek().GetHashCode().ToString();

                GameObject inW = Instantiate(wordPrefab, randPositions.Pop(), randRotations.Pop(), this.gameObject.transform);

                inW.GetComponent<Word>().Instant(ID, text);
                keys.Add(ID); //debug
                wordObjs.Add(ID, inW.transform);
            }
            Debug.Log(text + " created as " + ID);
            return ID;
        }
        private void CreateWords(string[] words)
        {
            foreach (string text in words)
            {
                CreateWord(text);
            }
        }
        public void Capture(string id)
        {
            if (wordObjs.ContainsKey(id))
                motor.Capture(wordObjs[id]);
        }
        public void AddNCaptere(string text)
        {
            Capture(CreateWord(text));
        }
        private void OnApplicationQuit()
        {
            ServerDown();
        }
    }
}