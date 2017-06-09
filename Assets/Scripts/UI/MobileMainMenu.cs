using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeusUtility;
using DeusUtility.UI;
using System;

namespace ITDmProject
{
    public class MobileMainMenu : MonoBehaviour
    {
        protected enum MenuWindow { Admin, Servers, PutWord, Options, Lists, About }
        public Rect mainRect;
        public GUISkin Skin;
        public Vector2 screenRatio;
        public float scale;
        public DeusUtility.UI.ScreenOrientation orientation;
        private MenuWindow CurWin;
        UIWindowInfo[] Windows;
        private Vector2 scrollAboutPosition = Vector2.zero;
        private Vector2 scrollServersPosition = Vector2.zero;
        private Vector2 scrollWordsPosition = Vector2.zero;
        private Vector2 scrollStopPosition = Vector2.zero;
        private string newWordInput = "";
        private string newStopInput = "";
        private bool WordListChanged = false;
        private bool StopListChanged = false;
        private string serverNameInput;
        GlobalControllerMobile Global;
        bool langChanged;
        string sendingWord;
        private float sentBackCount;
        private const string ConsolePass = "@deusaccess";


        void Start()
        {
            Global = FindObjectOfType<GlobalControllerMobile>();

            Windows = new UIWindowInfo[3];
            sendingWord = "Input";
            //serverNameInput = Global.ServerName;
            //Windows[5] = new SDWindowInfo(new Rect(0, Screen.height-100, 100, 100));//info
                CurWin = MenuWindow.PutWord;
        }
        private void Update()
        {
            if (sentBackCount > 0)
                sentBackCount -= Time.deltaTime;
            OrientAnalys();
            ScaleScreen();
        }
        public void OrientAnalys()
        {
            switch (Screen.orientation)
            {
                case UnityEngine.ScreenOrientation.Portrait:
                case UnityEngine.ScreenOrientation.PortraitUpsideDown:
                    {
                        orientation = DeusUtility.UI.ScreenOrientation.Portrait;
                        break;
                    }
                case UnityEngine.ScreenOrientation.Landscape:
                case UnityEngine.ScreenOrientation.AutoRotation:
                case UnityEngine.ScreenOrientation.LandscapeRight:
                case UnityEngine.ScreenOrientation.Unknown:
                    {
                        orientation = DeusUtility.UI.ScreenOrientation.Landscape;
                        break;
                    }
            }
        }
        public void ScaleScreen()
        {
            screenRatio = UIUtil.GetRatio();
            switch (orientation)
            {
                case DeusUtility.UI.ScreenOrientation.Landscape:
                    {
                        scale = Screen.width / (1280f / 1.4f);
                        mainRect = new Rect(0, 0, Screen.width / scale, Screen.height / scale);
                        Windows[0] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(new Vector2(16, 9), screenRatio) / scale * 1f, PositionAnchor.Center, mainRect.size));//main
                        Windows[1] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(new Vector2(8, 5.0f), screenRatio) / scale * 1f, PositionAnchor.Center, mainRect.size));//launch
                        Windows[2] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(new Vector2(6.8f, 8.4f), screenRatio) / scale * 1f, PositionAnchor.Center, mainRect.size));//admin
                        break;
                    }
                case DeusUtility.UI.ScreenOrientation.Portrait:
                    {
                        scale = Screen.width / (720f / 1.4f);
                        mainRect = new Rect(0, 0, Screen.width / scale, Screen.height / scale);
                        Windows[0] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(new Vector2(9, 16), screenRatio) / scale * 1f, PositionAnchor.Center, mainRect.size));//main
                        Windows[1] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(new Vector2(8, 5.0f), screenRatio) / scale * 1f, PositionAnchor.Center, mainRect.size));//launch
						Windows[2] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(new Vector2(6.8f, 8.4f), screenRatio) / scale * 1f, PositionAnchor.Center, mainRect.size));//admin
						break;
                    }
            }
        }
        void OnGUI()
        {
            GUI.skin = Skin;
            GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
            GUI.BeginGroup(mainRect);
            {
                switch (CurWin)
                {
                    case MenuWindow.PutWord:
                        {
                            GUI.Window(1, Windows[1].rect, DrawLaunchW, "");
                            break;
                        }
                    case MenuWindow.Admin:
                        {
                            GUI.Window(2, Windows[2].rect, DrawAdminW, "");
                            break;
                        }
                    case MenuWindow.Servers:
                        {
                            GUI.Window(0, Windows[0].rect, DrawServersW, "");
                            break;
                        }
                    case MenuWindow.Lists:
                        {
                            GUI.Window(0, Windows[0].rect, DrawListsW, "");
                            break;
                        }
                    case MenuWindow.Options:
                        {
                            GUI.Window(0, Windows[0].rect, DrawOptionsW, "");
                            break;
                        }
                    case MenuWindow.About:
                        {
                            GUI.Window(0, Windows[0].rect, DrawAboutW, "");
                            break;
                        }
                }
                UIUtil.Exclamation(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.LeftDown, mainRect.size), "Jogo Deus");
                UIUtil.Exclamation(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.RightDown, mainRect.size), "v. " + Application.version);
            }
            GUI.EndGroup();
        }

        private void DrawAdminW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "Administration");
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 100)), Global.Texts("Connection")))
            {
                CurWin = MenuWindow.Servers;
            }
            if (Global.SettingsRecieved)
            {
                if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 160)), Global.Texts("Lists")))
                {
                    CurWin = MenuWindow.Lists;
                }
                if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 220)), Global.Texts("Options")))
                {
                    CurWin = MenuWindow.Options;
                }
            }
            else
            {
                UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 200)), Global.Texts("No data"));
            }
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 280)), Global.Texts("About")))
            {
                CurWin = MenuWindow.About;
            }
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 340)), Global.Texts("Back")))
            {
                CurWin = MenuWindow.PutWord;
            }
        }

        private void DrawListsW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "Lists");
            switch (orientation)
            {
                case DeusUtility.UI.ScreenOrientation.Landscape:
                    {
                        //words list
                        Rect viewRectLeft = new Rect(0, 0, (Windows[windowID].rect.size.x) / 2 - 110, 50 + 55 * Global.wordList.Count);
                        Rect scrollRectLeft = UIUtil.GetRect(new Vector2(viewRectLeft.size.x + 20, Windows[windowID].rect.size.y - (135f + 110f)), PositionAnchor.LeftUp, Windows[windowID].rect.size, new Vector2(70, 135));
                        if (Global.WordListRecieved)
                            UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(viewRectLeft.size.x, 20), PositionAnchor.LeftUp, Windows[windowID].rect.size, new Vector2(70, 110)), Global.Texts("Words list"));
                        else
                            UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(viewRectLeft.size.x, 20), PositionAnchor.LeftUp, Windows[windowID].rect.size, new Vector2(70, 110)), Global.Texts("Words list no data"));
                        scrollWordsPosition = GUI.BeginScrollView(scrollRectLeft, scrollWordsPosition, viewRectLeft);
                        {
                            int i;
                            for (i = 0; i < Global.wordList.Count; i++)
                            {
                                UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(viewRectLeft.width - 150, 50), PositionAnchor.LeftUp, viewRectLeft.size, new Vector2(10, 20 + 55 * i)), Global.wordList[i]);
                                if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(120, 50), PositionAnchor.RightUp, viewRectLeft.size, new Vector2(-10, 5 + 55 * i)), Global.Texts("Delete")))
                                {
                                    Global.wordList.Remove(Global.wordList[i]);
                                    WordListChanged = true;
                                    i--;
                                }
                            }
                            newWordInput = GUI.TextField(UIUtil.GetRect(new Vector2(viewRectLeft.width - 150, 50), PositionAnchor.LeftUp, viewRectLeft.size, new Vector2(10, 5 + 55 * i)), newWordInput);
                            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(120, 50), PositionAnchor.RightUp, viewRectLeft.size, new Vector2(-10, 5 + 55 * i)), Global.Texts("Add")))
                            {
                                Global.wordList.Add(newWordInput);
                                scrollWordsPosition.y += 55;
                                newWordInput = "";
                                WordListChanged = true;
                            }
                        }
                        GUI.EndGroup();
                        //words list end

                        //stop list
                        Rect viewRectRight = new Rect(0, 0, (Windows[windowID].rect.size.x) / 2 - 110, 50 + 55 * Global.stopList.Count);
                        Rect scrollRectRight = UIUtil.GetRect(new Vector2(viewRectRight.size.x + 20, Windows[windowID].rect.size.y - (135f + 110f)), PositionAnchor.RightUp, Windows[windowID].rect.size, new Vector2(-70, 135));
                        if (Global.StopListRecieved)
                            UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(viewRectRight.size.x, 20), PositionAnchor.RightUp, Windows[windowID].rect.size, new Vector2(-70, 110)), Global.Texts("Stop list"));
                        else
                            UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(viewRectRight.size.x, 20), PositionAnchor.RightUp, Windows[windowID].rect.size, new Vector2(-70, 110)), Global.Texts("Stop list no data"));
                        scrollStopPosition = GUI.BeginScrollView(scrollRectRight, scrollStopPosition, viewRectRight);
                        {
                            int i;
                            for (i = 0; i < Global.stopList.Count; i++)
                            {
                                UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(viewRectLeft.width - 150, 50), PositionAnchor.LeftUp, viewRectRight.size, new Vector2(10, 20 + 55 * i)), Global.stopList[i]);
                                if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(120, 50), PositionAnchor.RightUp, viewRectRight.size, new Vector2(-10, 5 + 55 * i)), Global.Texts("Delete")))
                                {
                                    Global.stopList.Remove(Global.stopList[i]);
                                    StopListChanged = true;
                                    i--;
                                }
                            }
                            newStopInput = GUI.TextField(UIUtil.GetRect(new Vector2(viewRectLeft.width - 150, 50), PositionAnchor.LeftUp, viewRectRight.size, new Vector2(10, 5 + 55 * i)), newStopInput);
                            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(120, 50), PositionAnchor.RightUp, viewRectRight.size, new Vector2(-10, 5 + 55 * i)), Global.Texts("Add")))
                            {
                                Global.stopList.Add(newStopInput);
                                scrollStopPosition.y += 55;
                                StopListChanged = true;
                                newStopInput = "";
                            }
                        }
                        GUI.EndGroup();
                        //stop list end
                        break;
                    }
                case DeusUtility.UI.ScreenOrientation.Portrait:
                    {
						//words list
                        Rect viewRectUp = new Rect(0, 0, Windows[windowID].rect.size.x - 140, 50 + 55 * Global.wordList.Count);
                        Rect scrollRectUp = UIUtil.GetRect(new Vector2(viewRectUp.size.x + 20, (Windows[windowID].rect.size.y - (135f + 110f))/2-20), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 135));
						if (Global.WordListRecieved)
							UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(viewRectUp.size.x, 20), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 110)), Global.Texts("Words list"));
						else
							UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(viewRectUp.size.x, 20), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 110)), Global.Texts("Words list no data"));
						scrollWordsPosition = GUI.BeginScrollView(scrollRectUp, scrollWordsPosition, viewRectUp);
						{
							int i;
							for (i = 0; i < Global.wordList.Count; i++)
							{
								UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(viewRectUp.width - 150, 50), PositionAnchor.LeftUp, viewRectUp.size, new Vector2(10, 20 + 55 * i)), Global.wordList[i]);
								if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(120, 50), PositionAnchor.RightUp, viewRectUp.size, new Vector2(-10, 5 + 55 * i)), Global.Texts("Delete")))
								{
									Global.wordList.Remove(Global.wordList[i]);
									WordListChanged = true;
									i--;
								}
							}
							newWordInput = GUI.TextField(UIUtil.GetRect(new Vector2(viewRectUp.width - 150, 50), PositionAnchor.LeftUp, viewRectUp.size, new Vector2(10, 5 + 55 * i)), newWordInput);
							if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(120, 50), PositionAnchor.RightUp, viewRectUp.size, new Vector2(-10, 5 + 55 * i)), Global.Texts("Add")))
							{
								Global.wordList.Add(newWordInput);
								scrollWordsPosition.y += 55;
								newWordInput = "";
								WordListChanged = true;
							}
						}
						GUI.EndGroup();
						//words list end

						//stop list
                        Rect viewRectDown = new Rect(0, 0, Windows[windowID].rect.size.x - 140, 50 + 55 * Global.stopList.Count);
                        Rect scrollRectDown = UIUtil.GetRect(new Vector2(viewRectDown.size.x + 20, (Windows[windowID].rect.size.y - (135f + 110f)) / 2 - 20), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, scrollRectUp.size.y + 135f + 40f));
						if (Global.StopListRecieved)
							UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(viewRectDown.size.x, 20), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, scrollRectUp.size.y + 110f + 40f)), Global.Texts("Stop list"));
						else
							UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(viewRectDown.size.x, 20), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, scrollRectUp.size.y + 110f + 40f)), Global.Texts("Stop list no data"));
						scrollStopPosition = GUI.BeginScrollView(scrollRectDown, scrollStopPosition, viewRectDown);
						{
							int i;
							for (i = 0; i < Global.stopList.Count; i++)
							{
								UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(viewRectUp.width - 150, 50), PositionAnchor.LeftUp, viewRectDown.size, new Vector2(10, 20 + 55 * i)), Global.stopList[i]);
								if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(120, 50), PositionAnchor.RightUp, viewRectDown.size, new Vector2(-10, 5 + 55 * i)), Global.Texts("Delete")))
								{
									Global.stopList.Remove(Global.stopList[i]);
									StopListChanged = true;
									i--;
								}
							}
							newStopInput = GUI.TextField(UIUtil.GetRect(new Vector2(viewRectUp.width - 150, 50), PositionAnchor.LeftUp, viewRectDown.size, new Vector2(10, 5 + 55 * i)), newStopInput);
							if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(120, 50), PositionAnchor.RightUp, viewRectDown.size, new Vector2(-10, 5 + 55 * i)), Global.Texts("Add")))
							{
								Global.stopList.Add(newStopInput);
								scrollStopPosition.y += 55;
								StopListChanged = true;
								newStopInput = "";
							}
						}
						GUI.EndGroup();
                        //stop list end
                        break;
					}
            }
            if (WordListChanged||StopListChanged)
            {
                if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts("Regenerate")))
                {
                    if (WordListChanged)
                    {
                        Global.PushWordsList();
                        WordListChanged = false;
                    }
                    if (StopListChanged)
                    {
                        Global.PushStopList();
                        StopListChanged = false;
                    }
                }
            }
            else 
            {
                if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts("Back")))
                    CurWin = MenuWindow.Admin;
			}
        }

        void DrawServersW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "Connections");
            if (Global.Connected)
            {
                UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(200, 20), PositionAnchor.LeftUp, Windows[windowID].rect.size, new Vector2(70, 110)), Global.Texts("Connected"));
                //UIUtil.TextContainerTitle(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 50)), Global.Texts("Connected"));
                ServerInfo current = Global.CurrentConnection;
                UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(200, 20), PositionAnchor.RightUp, Windows[windowID].rect.size, new Vector2(-70, 110)), current.Name);
            }
            else
            {
                UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(200, 20), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 110)), Global.Texts("Disconnected"));
            }
            //servers list
            Rect viewRect = new Rect(0, 0, Windows[windowID].rect.size.x - 170, 50 + 55 * Global.Servers.Count);
            UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(200, 20), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 145)), Global.Servers.Count.ToString() + " Servers");
            scrollServersPosition = GUI.BeginScrollView(
                UIUtil.GetRect(new Vector2(Windows[windowID].rect.size.x - 150, Windows[windowID].rect.size.y - (175f + 110f)), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 175)),
                scrollServersPosition, viewRect);
            {
                int i;
                for (i = 0; i < Global.Servers.Count; i++)
                {
                    UIUtil.TextContainerTitle(UIUtil.GetRect(new Vector2(viewRect.width - 170, 50), PositionAnchor.LeftUp, viewRect.size, new Vector2(10, 20 + 55 * i)), Global.Servers[i].Name);
                    if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(150, 50), PositionAnchor.RightUp, viewRect.size, new Vector2(-10, 5 + 55 * i)), Global.Texts("Connect")))
                    {
                        Global.ConnectTo(i);
                        CurWin = MenuWindow.Admin;
                    }
                }
            }
            GUI.EndGroup();
            //servers list end
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts("Back")))
            {
                CurWin = MenuWindow.Admin;
            }
        }
        void DrawLaunchW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "WordDisplay");
            GUIStyle local = new GUIStyle(Skin.GetStyle("TextField"));
            local.fontSize = 26;
            sendingWord = ValidString.ReplaceChar(GUI.TextField(UIUtil.GetRect(new Vector2(250, 60), PositionAnchor.Center, Windows[windowID].rect.size), sendingWord, 15, local), '#', '_');
            if (sentBackCount > 0)
                UIUtil.TextStyle1(UIUtil.GetRect(new Vector2(50, 20), PositionAnchor.Center, Windows[windowID].rect.size, new Vector2(100, 45)), Global.Texts("Sent"));
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(250, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts("Send")))
            {
                if (sendingWord == ConsolePass)
                {
                    CurWin = MenuWindow.Admin;
                }
                else
                {
                    if (Global.Connected)
                    {
                        Global.Send(sendingWord);
                        sentBackCount = 5;
                    }
                }
            }
        }
        void DrawOptionsW(int windowID)
        {
            float fBuffer;
            int iBuffer;
            UIUtil.WindowTitle(Windows[windowID], Global.Texts("Options"));
            GUI.BeginGroup(UIUtil.GetRect(new Vector2(300, 55), PositionAnchor.LeftUp, Windows[windowID].rect.size, new Vector2(70, 100)));
            UIUtil.Label(new Rect(50, 0, 200, 20), Global.Texts("Duration") + " - " + Global.Duration);
            fBuffer = Convert.ToSingle(Math.Round(GUI.HorizontalSlider(new Rect(0, 40, 300, 13), Global.Duration, 0.2f, 10f), 1));
            if (Global.Duration != fBuffer)
                Global.Duration = fBuffer;
            GUI.EndGroup();

            GUI.BeginGroup(UIUtil.GetRect(new Vector2(300, 55), PositionAnchor.RightUp, Windows[windowID].rect.size, new Vector2(-70, 100)));
            UIUtil.Label(new Rect(50, 0, 200, 20), Global.Texts("Delay") + " - " + Global.Delay);
            fBuffer = Convert.ToSingle(Math.Round(GUI.HorizontalSlider(new Rect(0, 40, 300, 13), Global.Delay, 0.0f, 10f), 1));
            if (Global.Delay != fBuffer)
                Global.Delay = fBuffer;
            GUI.EndGroup();

            GUI.BeginGroup(UIUtil.GetRect(new Vector2(300, 55), PositionAnchor.LeftUp, Windows[windowID].rect.size, new Vector2(70, 165)));
            UIUtil.Label(new Rect(50, 0, 200, 20), Global.Texts("Sphere radius") + " - " + Global.Radius);
            iBuffer = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(0, 40, 300, 13), Global.Radius, 10f, 1000f));
            if (Global.Radius != iBuffer)
                Global.Radius = iBuffer;
            GUI.EndGroup();

            GUI.BeginGroup(UIUtil.GetRect(new Vector2(300, 55), PositionAnchor.RightUp, Windows[windowID].rect.size, new Vector2(-70, 165)));
            UIUtil.Label(new Rect(50, 0, 200, 20), Global.Texts("Object radius") + " - " + Global.ObjectRadius);
            iBuffer = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(0, 40, 300, 13), Global.ObjectRadius, 1f, Global.Radius / 4));
            if (iBuffer > Global.Radius / 4) iBuffer = Global.Radius / 4;
            if (Global.ObjectRadius != iBuffer)
                Global.ObjectRadius = iBuffer;
			GUI.EndGroup();

            UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(200, 20), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 230) ), Global.ObjectsInSphere(Global.Radius, Global.ObjectRadius).ToString() + " - objects");

            GUI.BeginGroup(UIUtil.GetRect(new Vector2(300, 90), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 250)));
            UIUtil.Label(new Rect(50, 0, 200, 20), Global.Texts("Server name"));
            GUI.TextField(new Rect(0, 30, 300, 50), Global.ServerName);
            GUI.EndGroup();

            string[] radios = new string[2];
            radios[0] = "English";
            radios[1] = "Русский";
            int radioSelected = (int)Global.Localisation;
            GUI.BeginGroup(UIUtil.GetRect(new Vector2(100, 110), PositionAnchor.LeftDown, Windows[windowID].rect.size, new Vector2(70, -100)));
            UIUtil.Label(new Rect(0, 0, 100, 20), Global.Texts("Language"));
            radioSelected = UIUtil.ToggleList(new Rect(0, 40, 100, 74), radioSelected, radios);
            GUI.EndGroup();
            if (radioSelected != (int)Global.Localisation)
            {
                Global.Localisation = (Languages)radioSelected;
                langChanged = true;
            }

            if (Global.DesctopSettingsSaved)
            {
                if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(100), Windows[windowID].Bottom - 100, UIUtil.Scaled(200), 50), Global.Texts("Back")))
                {
                    CurWin = MenuWindow.Admin;
                    if (langChanged)
                        Global.LoadLocalisationTexts();
                }
            }
            else
            {
                if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(100), Windows[windowID].Bottom - 100, UIUtil.Scaled(200), 50), Global.Texts("Save")))
                {
                    Global.PushSettings();
                    Global.SaveSettings();
                }
            }
        }
        void DrawAboutW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.Texts("About"));
            GUI.BeginGroup(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(140), 100, UIUtil.Scaled(280), 150), GUI.skin.GetStyle("textContainer"));
            UIUtil.TextContainerTitle(new Rect(27, 10, UIUtil.Scaled(220), 20), Global.Texts("Development"));
            UIUtil.TextContainerText(new Rect(27, 40, UIUtil.Scaled(220), 60), Global.Texts("Develop_content"));
            GUI.EndGroup();

            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(UIUtil.Scaled(200), 50), PositionAnchor.RightDown, Windows[windowID].rect.size, new Vector2(-50, -50)), Global.Texts("Developer page")))
            {
                Application.OpenURL("https://www.linkedin.com/in/%D0%B4%D0%B0%D0%BD%D0%B8%D0%B8%D0%BB-%D1%87%D0%B8%D0%BA%D0%B8%D1%88-5809a2108/");
            }
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(UIUtil.Scaled(200), 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts("Back")))
            {
                CurWin = MenuWindow.Servers;
            }
        }
    }
}
