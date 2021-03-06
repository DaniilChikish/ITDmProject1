﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeusUtility;
using DeusUtility.UI;
using System;

namespace ITDmProject
{
    public class MobileMainMenu : MonoBehaviour
    {
        protected enum MenuWindow { Admin, Servers, PutWord, Options, WordList, StopList, About }
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
        private Vector2 scrollSpeed = Vector2.zero;
        public float scrollDrag;
        private string newWordInput = "";
        private string newStopInput = "";
        private bool WordListChanged = false;
        private bool StopListChanged = false;
        private string serverNameInput;
        private TouchScreenKeyboard mobileKeyboard;
        private GlobalControllerMobile Global;
        bool langChanged;
        string sendingWord;
        private float labelBackCount;
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
            if (labelBackCount > 0)
                labelBackCount -= Time.deltaTime;
            if (scrollSpeed.magnitude > 1)
            {
                scrollSpeed -= scrollSpeed.normalized * scrollDrag * Time.deltaTime;
            }
            else scrollSpeed = Vector2.zero;
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
                        if (mobileKeyboard != null && mobileKeyboard.active)
                        {
                            Windows[0] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(new Vector2(16, 9), screenRatio) / scale * 1f, PositionAnchor.Down, mainRect.size, new Vector2(0, -UIUtil.GetAndroidTouchKeyboardHeight())));//main
                            Windows[1] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(new Vector2(8, 5.0f), screenRatio) / scale * 1f, PositionAnchor.Down, mainRect.size, new Vector2(0, -UIUtil.GetAndroidTouchKeyboardHeight())));//launch
                        }
                        else
                        {
							Windows[0] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(new Vector2(16, 9), screenRatio) / scale * 1f, PositionAnchor.Center, mainRect.size));//main
							Windows[1] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(new Vector2(8, 5.0f), screenRatio) / scale * 1f, PositionAnchor.Center, mainRect.size));//launch
                        }
                        Windows[2] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(new Vector2(6.8f, 8.4f), screenRatio) / scale * 1f, PositionAnchor.Center, mainRect.size));//admin
                        break;
                    }
                case DeusUtility.UI.ScreenOrientation.Portrait:
                    {
                        scale = Screen.width / (720f / 1.4f);
                        mainRect = new Rect(0, 0, Screen.width / scale, Screen.height / scale);
                        if (mobileKeyboard != null && mobileKeyboard.active)
                        {
                            Windows[0] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(new Vector2(9, 16), screenRatio) / scale * 1f, PositionAnchor.Down, mainRect.size, new Vector2(0, -UIUtil.GetAndroidTouchKeyboardHeight())));//main
                            Windows[1] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(new Vector2(8, 5.0f), screenRatio) / scale * 1f, PositionAnchor.Down, mainRect.size, new Vector2(0, -UIUtil.GetAndroidTouchKeyboardHeight())));//launch
                        }
                        else
                        {
							Windows[0] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(new Vector2(9, 16), screenRatio) / scale * 1f, PositionAnchor.Center, mainRect.size));//main
							Windows[1] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(new Vector2(8, 5.0f), screenRatio) / scale * 1f, PositionAnchor.Center, mainRect.size));//launch
                        }
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
                    case MenuWindow.WordList:
                        {
                            GUI.Window(0, Windows[0].rect, DrawWordListW, "");
                            break;
                        }
                    case MenuWindow.StopList:
                        {
                            GUI.Window(0, Windows[0].rect, DrawStopListW, "");
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
            if (Global.Connected&&Global.SettingsRecieved)
            {
                if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 150)), Global.Texts("Word list")))
                {
                    CurWin = MenuWindow.WordList;
                }
                if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 200)), Global.Texts("Stop list")))
                {
                    CurWin = MenuWindow.StopList;
                }
                if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 250)), Global.Texts("Options")))
                {
                    CurWin = MenuWindow.Options;
                }
            }
            else
            {
                UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 200)), Global.Texts("No data"));
            }
            //if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 300)), Global.Texts("About")))
            //{
            //    CurWin = MenuWindow.About;
            //}
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 350)), Global.Texts("Back")))
            {
                CurWin = MenuWindow.PutWord;
            }
        }

        private void DrawWordListW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "Word lists");
            //words list
            Rect viewRect = new Rect(0, 0, Windows[windowID].rect.size.x - 140, 50 + 55 * Global.wordList.Count);
            Rect scrollRect = UIUtil.GetRect(new Vector2(viewRect.size.x + 20, (Windows[windowID].rect.size.y - (135f + 140f))), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 135));
            if (Global.WordListRecieved)
                UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(viewRect.size.x, 20), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 110)), Global.Texts("Words list"));
            else
                UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(viewRect.size.x, 20), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 110)), Global.Texts("Words list no data"));
            scrollWordsPosition = GUI.BeginScrollView(scrollRect, scrollWordsPosition, viewRect);
            if (Application.platform == RuntimePlatform.Android)
            {
                scrollWordsPosition += scrollSpeed;
                Vector2 speedBuff = UIUtil.TouchScroll(scrollWordsPosition, scrollRect, 1, scale) - scrollWordsPosition;
                if (speedBuff != Vector2.zero)
                    scrollSpeed = speedBuff;
            }
            else
            {
                scrollWordsPosition += scrollSpeed;
                Vector2 speedBuff = UIUtil.MouseScroll(scrollWordsPosition, scrollRect, 0.5f, scale) - scrollWordsPosition;
                if (speedBuff != Vector2.zero)
                    scrollSpeed = speedBuff;
                //scrollWordsPosition = UIUtil.MouseScroll(scrollWordsPosition, scrollRect, 0.5f, scale);
            }
            {
                int i;
                for (i = 0; i < Global.wordList.Count; i++)
                {
                    UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(viewRect.width - 150, 50), PositionAnchor.LeftUp, viewRect.size, new Vector2(10, 20 + 55 * i)), Global.wordList[i]);
                    if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(120, 50), PositionAnchor.RightUp, viewRect.size, new Vector2(-10, 5 + 55 * i)), Global.Texts("Delete")))
                    {
                        Global.DeleteWord(Global.wordList[i]);
                        //Global.wordList.Remove(Global.wordList[i]);
                        WordListChanged = true;
                        i--;
                    }
                }
                newWordInput = ValidString.ReplaceChar(GUI.TextField(UIUtil.GetRect(new Vector2(viewRect.width - 150, 50), PositionAnchor.LeftUp, viewRect.size, new Vector2(10, 5 + 55 * i)), newWordInput), '#', '_');
                bool enter = false;
                if (Application.platform == RuntimePlatform.Android)
                {
                    if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(120, 50), PositionAnchor.RightUp, viewRect.size, new Vector2(-10, 5 + 55 * i)), Global.Texts("Add")))
                        enter = true;
                    else
                    {
                        //enter = mobileKeyboard.done;
                        newWordInput = ValidString.ReplaceChar(mobileKeyboard.text, '#', '_');
                    }
                }
                else
                {
                    enter |= (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(120, 50), PositionAnchor.RightUp, viewRect.size, new Vector2(-10, 5 + 55 * i)), Global.Texts("Add")) || Input.GetKey(KeyCode.Return));
                }
                if (enter)
                {
                    if (newWordInput != "")
                        if (Global.Allowed(newWordInput))
                        {
                            Global.SendWord(newWordInput);
                            //Global.wordList.Add(newWordInput);
                            scrollWordsPosition.y += 55;
                            newWordInput = "";
                            if (mobileKeyboard != null)
                                mobileKeyboard.text = "";
                            WordListChanged = true;
                        }
                        else
                        {
                            labelBackCount = 5;
                        }
                }
            }
            GUI.EndGroup();
            if (labelBackCount > 0)
                UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(150, 20), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -100)), Global.Texts("Not alloved"));
            //words list end

            //if (WordListChanged)
            //{
            //if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts("Regenerate")))
            //{
            //
            //        //Global.PushWordsList();
            //        WordListChanged = false;
            //    }
            //}
            //else
            //{
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts("Back")))
                CurWin = MenuWindow.Admin;
            //}
        }
        private void DrawStopListW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "Stop list");

            //stop list
            Rect viewRect = new Rect(0, 0, Windows[windowID].rect.size.x - 140, 50 + 55 * Global.stopList.Count);
            Rect scrollRect = UIUtil.GetRect(new Vector2(viewRect.size.x + 20, (Windows[windowID].rect.size.y - (135f + 140f))), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 135));
            if (Global.StopListRecieved)
                UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(viewRect.size.x, 20), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 110f)), Global.Texts("Stop list"));
            else
                UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(viewRect.size.x, 20), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 110f)), Global.Texts("Stop list no data"));
            scrollStopPosition = GUI.BeginScrollView(scrollRect, scrollStopPosition, viewRect);
            if (Application.platform == RuntimePlatform.Android)
            {
                scrollStopPosition += scrollSpeed;
                Vector2 speedBuff = UIUtil.TouchScroll(scrollStopPosition, scrollRect, 1, scale) - scrollStopPosition;
                if (speedBuff != Vector2.zero)
                    scrollSpeed = speedBuff;
            }
            else
            {
                scrollStopPosition += scrollSpeed;
                Vector2 speedBuff = UIUtil.MouseScroll(scrollStopPosition, scrollRect, 0.5f, scale) - scrollStopPosition;
                if (speedBuff != Vector2.zero)
                    scrollSpeed = speedBuff;
                //scrollWordsPosition = UIUtil.MouseScroll(scrollWordsPosition, scrollRect, 0.5f, scale);
            }
            {
                int i;
                for (i = 0; i < Global.stopList.Count; i++)
                {
                    UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(viewRect.width - 150, 50), PositionAnchor.LeftUp, viewRect.size, new Vector2(10, 20 + 55 * i)), Global.stopList[i]);
                    if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(120, 50), PositionAnchor.RightUp, viewRect.size, new Vector2(-10, 5 + 55 * i)), Global.Texts("Delete")))
                    {
                        Global.stopList.Remove(Global.stopList[i]);
                        StopListChanged = true;
                        i--;
                    }
                }
                newStopInput = ValidString.ReplaceChar(GUI.TextField(UIUtil.GetRect(new Vector2(viewRect.width - 150, 50), PositionAnchor.LeftUp, viewRect.size, new Vector2(10, 5 + 55 * i)), newStopInput), '#', '_');
                bool enter = false;
                if (Application.platform == RuntimePlatform.Android)
                {
                    if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(120, 50), PositionAnchor.RightUp, viewRect.size, new Vector2(-10, 5 + 55 * i)), Global.Texts("Add")))
                        enter = true;
                    else
                    {
                        //enter = mobileKeyboard.done;
                        newStopInput = ValidString.ReplaceChar(mobileKeyboard.text, '#', '_');
                    }
                }
                else
                {
                    enter |= (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(120, 50), PositionAnchor.RightUp, viewRect.size, new Vector2(-10, 5 + 55 * i)), Global.Texts("Add")) || Input.GetKey(KeyCode.Return));
                }
                if (enter)
                {
                    if (newStopInput != "")
                        if (Global.Allowed(newStopInput))
                        {
                            Global.stopList.Add(newStopInput);
                            scrollStopPosition.y += 55;
                            StopListChanged = true;
                            newStopInput = "";
                            if (mobileKeyboard != null)
                                mobileKeyboard.text = "";
                        }
                        else
                        {
                            labelBackCount = 5;
                        }
                }
            }
            GUI.EndGroup();
			if (labelBackCount > 0)
				UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(150, 20), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -100)), Global.Texts("Contained"));
            //stop list end
            if (StopListChanged)
            {
				if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts("Back")))
                {
                    CurWin = MenuWindow.Admin;
                    Global.CheckWords();
                    //Global.PushStopList();
                    StopListChanged = false;
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
            ServerInfo current = null;
            if (Global.Connected)
            {
                UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(200, 20), PositionAnchor.Center, Windows[windowID].rect.size, new Vector2(-70, 0)), Global.Texts("Connected to"));
                //UIUtil.TextContainerTitle(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 50)), Global.Texts("Connected"));
                current = Global.CurrentConnection;
                UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(200, 20), PositionAnchor.Center, Windows[windowID].rect.size, new Vector2(70, 0)), current.Name);
                if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Center, Windows[windowID].rect.size, new Vector2(0, 50)), Global.Texts("Disconnect")))
                {
                    Global.Disconnect();
                }
            }
            else
            {
                UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(200, 20), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 110)), Global.Texts("Disconnected"));

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
                        if (current != null && Global.Servers[i].Name == current.Name)
                        {
                            UIUtil.TextContainerTitle(UIUtil.GetRect(new Vector2(viewRect.width - 170, 50), PositionAnchor.LeftUp, viewRect.size, new Vector2(10, 20 + 55 * i)), Global.Servers[i].Name);
                            UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(150, 50), PositionAnchor.RightUp, viewRect.size, new Vector2(-10, 5 + 55 * i)), Global.Texts("Current"));
                        }
                        else
                        {
                            UIUtil.TextContainerTitle(UIUtil.GetRect(new Vector2(viewRect.width - 170, 50), PositionAnchor.LeftUp, viewRect.size, new Vector2(10, 20 + 55 * i)), Global.Servers[i].Name);
                            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(150, 50), PositionAnchor.RightUp, viewRect.size, new Vector2(-10, 5 + 55 * i)), Global.Texts("Connect")))
                            {
                                Global.ConnectTo(i);
                                CurWin = MenuWindow.Admin;
                            }
                        }
                    }
                }
                GUI.EndGroup();
                //servers list end
            }
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
            if (!Global.Connected)
				UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(100, 20), PositionAnchor.Center, Windows[windowID].rect.size, new Vector2(0, 45)), Global.Texts("Disconnected"));
			if (labelBackCount > 0)
                UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(50, 20), PositionAnchor.Center, Windows[windowID].rect.size, new Vector2(100, 45)), Global.Texts("Sent"));
            bool enter = false;
            if (Application.platform == RuntimePlatform.Android)
            {
                if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(250, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts("Send")))
                    enter = true;
                else
                {
                    if (mobileKeyboard == null)
                        mobileKeyboard = TouchScreenKeyboard.Open(sendingWord, TouchScreenKeyboardType.Default);
                    //enter = mobileKeyboard.done;
                    sendingWord = ValidString.ReplaceChar(mobileKeyboard.text, '#', '_');
                }
            }
            else
            {
                enter |= (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(250, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts("Send")) || Input.GetKey(KeyCode.Return));
            }

            sendingWord = ValidString.ReplaceChar(GUI.TextField(UIUtil.GetRect(new Vector2(250, 60), PositionAnchor.Center, Windows[windowID].rect.size), sendingWord, 15, local), '#', '_');

            if (enter && sendingWord != "")
            {
                if (sendingWord == ConsolePass)
                {
                    CurWin = MenuWindow.Admin;
                    sendingWord = "";
                    if (mobileKeyboard != null)
                        mobileKeyboard.text = "";
                }
                else
                {
                    if (Global.Connected)
                    {
                        Global.SendWord(sendingWord);
                        labelBackCount = 5;
                        sendingWord = "";
                        if (mobileKeyboard != null)
                            mobileKeyboard.text = "";
                    }
                }
            }
        }
        void DrawOptionsW(int windowID)
        {
			UIUtil.WindowTitle(Windows[windowID], Global.Texts("Options"));
			float fBuffer;
			int iBuffer;
            if (orientation == DeusUtility.UI.ScreenOrientation.Landscape)
            {
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

                UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(200, 20), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 230)), Global.ObjectsInSphere(Global.Radius, Global.ObjectRadius).ToString() + " - objects");

                /*
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
                */
            }
            else
            {
                GUI.BeginGroup(UIUtil.GetRect(new Vector2(300, 55), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 100)));
				UIUtil.Label(new Rect(50, 0, 200, 20), Global.Texts("Duration") + " - " + Global.Duration);
				fBuffer = Convert.ToSingle(Math.Round(GUI.HorizontalSlider(new Rect(0, 40, 300, 13), Global.Duration, 0.2f, 10f), 1));
				if (Global.Duration != fBuffer)
					Global.Duration = fBuffer;
				GUI.EndGroup();

                GUI.BeginGroup(UIUtil.GetRect(new Vector2(300, 55), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 165)));
				UIUtil.Label(new Rect(50, 0, 200, 20), Global.Texts("Delay") + " - " + Global.Delay);
				fBuffer = Convert.ToSingle(Math.Round(GUI.HorizontalSlider(new Rect(0, 40, 300, 13), Global.Delay, 0.0f, 10f), 1));
				if (Global.Delay != fBuffer)
					Global.Delay = fBuffer;
				GUI.EndGroup();

                GUI.BeginGroup(UIUtil.GetRect(new Vector2(300, 55), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 230)));
				UIUtil.Label(new Rect(50, 0, 200, 20), Global.Texts("Sphere radius") + " - " + Global.Radius);
				iBuffer = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(0, 40, 300, 13), Global.Radius, 10f, 1000f));
				if (Global.Radius != iBuffer)
					Global.Radius = iBuffer;
				GUI.EndGroup();

                GUI.BeginGroup(UIUtil.GetRect(new Vector2(300, 55), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 295)));
				UIUtil.Label(new Rect(50, 0, 200, 20), Global.Texts("Object radius") + " - " + Global.ObjectRadius);
				iBuffer = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(0, 40, 300, 13), Global.ObjectRadius, 1f, Global.Radius / 4));
				if (iBuffer > Global.Radius / 4) iBuffer = Global.Radius / 4;
				if (Global.ObjectRadius != iBuffer)
					Global.ObjectRadius = iBuffer;
				GUI.EndGroup();

				UIUtil.TextStyle2(UIUtil.GetRect(new Vector2(200, 20), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 360)), Global.ObjectsInSphere(Global.Radius, Global.ObjectRadius).ToString() + " - objects");
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
