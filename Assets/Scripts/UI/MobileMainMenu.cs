using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeusUtility;
using DeusUtility.UI;
namespace ITDmProject
{
    public class MobileMainMenu : MonoBehaviour
    {
        protected enum MenuWindow { Main, Launch, Options, About }
        public Rect mainRect;
        public GUISkin Skin;
        public Vector2 screenRatio;
        public float scale;
        public DeusUtility.UI.ScreenOrientation orientation;
        private MenuWindow CurWin;
        UIWindowInfo[] Windows;
        private Vector2 scrollAboutPosition = Vector2.zero;
        private Vector2 scrollServersPosition = Vector2.zero;
        GlobalControllerMobile Global;
        bool langChanged;
        string sendingWord;

        void Start()
        {
            Global = FindObjectOfType<GlobalControllerMobile>();

            CurWin = MenuWindow.Main;
            Windows = new UIWindowInfo[2];
            sendingWord = "Input";
            //Windows[5] = new SDWindowInfo(new Rect(0, Screen.height-100, 100, 100));//info
        }
        private void Update()
        {
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
                        scale = Screen.width / (1280f / 1.5f);
                        mainRect = new Rect(0, 0, Screen.width / scale, Screen.height / scale);
                        Windows[0] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(new Vector2(16, 9), screenRatio) / scale * 1f, PositionAnchor.Center, mainRect.size));//main
                        Windows[1] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(new Vector2(8, 5.0f), screenRatio) / scale * 1f, PositionAnchor.Center, mainRect.size));//launch
                        break;
                    }
                case DeusUtility.UI.ScreenOrientation.Portrait:
                    {
                        scale = Screen.width / (720f / 1.5f);
                        mainRect = new Rect(0, 0, Screen.width / scale, Screen.height / scale);
                        Windows[0] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(new Vector2(9, 16), screenRatio) / scale * 1f, PositionAnchor.Center, mainRect.size));//main
                        Windows[1] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(new Vector2(8, 5.0f), screenRatio) / scale * 1f, PositionAnchor.Center, mainRect.size));//launch
                        break;
                    }
            }
        }
        void OnGUI()
        {
            GUI.skin = Skin;
            //Matrix4x4 mat = new Matrix4x4();
            //mat.SetTRS(Vector3.one*scale, Quaternion.identity, Vector3.one * scale);
            GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
            GUI.BeginGroup(mainRect);
            {
                switch (CurWin)
                {
                    case MenuWindow.Main:
                        {
                            GUI.Window(0, Windows[0].rect, DrawMainW, "");
                            break;
                        }
                    case MenuWindow.Launch:
                        {
                            GUI.Window(1, Windows[1].rect, DrawLaunchW, "");
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
        void DrawMainW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "Mobile");
            //GUI.color.a = window.UIAlpha;
            //if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts["Refresh"]))
            //{
            //    Global.RunClient();
            //}
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.LeftUp, Windows[windowID].rect.size, new Vector2(50, 100)), Global.Texts("Options")))
            {
                langChanged = false;
                CurWin = MenuWindow.Options;
            }
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.RightUp, Windows[windowID].rect.size, new Vector2(-50, 100)), Global.Texts("About")))
            {
                CurWin = MenuWindow.About;
            }

            Rect viewRect = new Rect(0, 0, Windows[windowID].rect.size.x - 170, 50 + 55 * Global.Servers.Count);
            UIUtil.Label(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 125)), Global.Servers.Count.ToString()+ " Servers");
            scrollServersPosition = GUI.BeginScrollView(
                UIUtil.GetRect(new Vector2(Windows[windowID].rect.size.x - 150, Windows[windowID].rect.size.y - (175f + 60f)), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 175)),
                scrollServersPosition, viewRect);
            {
                int i;
                for (i = 0; i < Global.Servers.Count; i++)
                {
                    UIUtil.TextContainerTitle(UIUtil.GetRect(new Vector2(viewRect.width - 170, 50), PositionAnchor.LeftUp, viewRect.size, new Vector2(10, 20 + 55 * i)), Global.Servers[i].Name);
                    if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(150, 50), PositionAnchor.RightUp, viewRect.size, new Vector2(-10, 5 + 55 * i)), Global.Texts("Connect")))
                    {
                        Debug.Log("Try to connect");
                        Debug.Log("\t addres:" + Global.Servers[i].Address);
                        Debug.Log("\t port:" + Global.Servers[i].Port);
                        Debug.Log("\t name:" + Global.Servers[i].Name);
						Global.ConnectTo(i);
                        CurWin = MenuWindow.Launch;
                    }
                }
            }
            GUI.EndGroup();
        }
        void DrawLaunchW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "Mobile");
            sendingWord = ValidString.ReplaceChar(GUI.TextField(UIUtil.GetRect(new Vector2(250, 60), PositionAnchor.Center, Windows[windowID].rect.size), sendingWord, 15), '#', '_');
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(250, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts("Send")))
			{
                Global.Send(sendingWord);
                CurWin = MenuWindow.Main;
			}
        }
        void DrawOptionsW(int windowID)
        {
            float fBuffer;
            UIUtil.WindowTitle(Windows[windowID], Global.Texts("Options"));

            string[] radios = new string[2];
            radios[0] = "English";
            radios[1] = "Русский";
            int radioSelected = (int)Global.Localisation;
            GUI.BeginGroup(UIUtil.GetRect(new Vector2(100, 110), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 110)));
            UIUtil.Label(new Rect(0, 0, 100, 20), Global.Texts("Language"));
            radioSelected = UIUtil.ToggleList(new Rect(0, 40, 100, 74), radioSelected, radios);
            GUI.EndGroup();
            if (radioSelected != (int)Global.Localisation)
            {
                Global.Localisation = (Languages)radioSelected;
                langChanged = true;
            }

            if (Global.SettingsSaved)
            {
                if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(UIUtil.Scaled(200), 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts("Back")))
                {
                    CurWin = MenuWindow.Main;
                    if (langChanged)
                        Global.LoadLocalisationTexts();
                }
            }
            else
            {
                if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(UIUtil.Scaled(200), 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts("Save")))
                {
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
                CurWin = MenuWindow.Main;
            }
        }
    }
}
