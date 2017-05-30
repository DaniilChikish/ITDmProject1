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
        public GUISkin Skin;
        private MenuWindow CurWin;
        UIWindowInfo[] Windows;
        private Vector2 scrollAboutPosition = Vector2.zero;
        private Vector2 scrollServersPosition = Vector2.zero;
        GlobalControllerMobile Global;
        bool langChanged;

        void Start()
        {
            Global = FindObjectOfType<GlobalControllerMobile>();

            CurWin = MenuWindow.Main;
            Windows = new UIWindowInfo[2];
            //Windows[5] = new SDWindowInfo(new Rect(0, Screen.height-100, 100, 100));//info
        }
        private void Update()
        {
            //
            //Debug.Log(Screen.orientation);
            if (Screen.orientation == ScreenOrientation.Landscape)
            Windows[0] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(12, 7), PositionAnchor.Center));//main
            else Windows[0] = new UIWindowInfo(UIUtil.GetRect(UIUtil.GetRectSize(7, 12), PositionAnchor.Center));//main
            //
        }
        void OnGUI()
        {
            GUI.skin = Skin;
            GUI.BeginGroup(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 100, 100));
            switch (CurWin)
            {
                case MenuWindow.Main:
                    {
                        GUI.Window(0, Windows[0].rect, DrawMainW, "");
                        break;
                    }
                case MenuWindow.Launch:
                    {
                        GUI.Window(1, Windows[1].rect, DrawConnectW, "");
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
            GUI.EndGroup();
            UIUtil.Exclamation(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.LeftDown), "Jogo Deus");
            UIUtil.Exclamation(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.RightDown), "v. " + Application.version);
        }
        void DrawMainW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "Mobile");
            //GUI.color.a = window.UIAlpha;
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts["Refresh"]))
            {

            }
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.LeftUp, Windows[windowID].rect.size, new Vector2(50, 100)), Global.Texts["Options"]))
            {
                langChanged = false;
                CurWin = MenuWindow.Options;
            }
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.RightUp, Windows[windowID].rect.size, new Vector2(-50, 100)), Global.Texts["About"]))
            {
                CurWin = MenuWindow.About;
            }

            Rect viewRect = new Rect(0, 0, Windows[windowID].rect.size.x - 150, Windows[windowID].rect.size.y - 310);
            UIUtil.Label(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 155)), "Servers");
            scrollAboutPosition = GUI.BeginScrollView(
                UIUtil.GetRect(new Vector2(Windows[windowID].rect.size.x - 150, Windows[windowID].rect.size.y - 310), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 200)),
                scrollAboutPosition, viewRect);
            {
                int i = 0;
                UIUtil.TextContainerTitle(UIUtil.GetRect(new Vector2(viewRect.width - 170, 40), PositionAnchor.LeftUp, viewRect.size, new Vector2(10, 20+55 * i)), "Server 1");
                UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(150, 50), PositionAnchor.RightUp, viewRect.size, new Vector2(-10, 5+55 * i)), Global.Texts["Connect"]);
            }
            GUI.EndGroup();
        }
        void DrawConnectW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "Mobile");
        }
        void DrawLaunchW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "Mobile");
        }
        void DrawOptionsW(int windowID)
        {
            float fBuffer;
            UIUtil.WindowTitle(Windows[windowID], Global.Texts["Options"]);

            string[] radios = new string[2];
            radios[0] = "English";
            radios[1] = "Русский";
            int radioSelected = (int)Global.Localisation;
            GUI.BeginGroup(UIUtil.GetRect(new Vector2(77, 380), PositionAnchor.LeftDown, Windows[windowID].rect.size));
            UIUtil.Label(new Rect(0, 0, 100, 20), Global.Texts["Language"]);
            radioSelected = UIUtil.ToggleList(new Rect(0, 40, 100, 74), radioSelected, radios);
            GUI.EndGroup();
            if (radioSelected != (int)Global.Localisation)
            {
                Global.Localisation = (Languages)radioSelected;
                langChanged = true;
            }

            if (Global.SettingsSaved)
            {
                if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(UIUtil.Scaled(200), 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts["Back"]))
                {
                    CurWin = MenuWindow.Main;
                    if (langChanged)
                        Global.LoadLocalisationTexts();
                }
            }
            else
            {
                if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(UIUtil.Scaled(200), 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts["Save"]))
                {
                    Global.SaveSettings();
                }
            }
        }
        void DrawAboutW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.Texts["About"]);
            GUI.BeginGroup(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(140), 100, UIUtil.Scaled(280), 150), GUI.skin.GetStyle("textContainer"));
            UIUtil.TextContainerTitle(new Rect(27, 10, UIUtil.Scaled(220), 20), Global.Texts["Development"]);
            UIUtil.TextContainerText(new Rect(27, 40, UIUtil.Scaled(220), 60), Global.Texts["Develop_content"]);
            GUI.EndGroup();

            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(UIUtil.Scaled(200), 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(UIUtil.Scaled(200)/ 2 + 30, -50)), Global.Texts["Developer page"]))
            {
                Application.OpenURL("https://www.linkedin.com/in/%D0%B4%D0%B0%D0%BD%D0%B8%D0%B8%D0%BB-%D1%87%D0%B8%D0%BA%D0%B8%D1%88-5809a2108/");
            }
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(UIUtil.Scaled(200), 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts["Back"]))
            {
                CurWin = MenuWindow.Main;
            }
        }
    }
}
