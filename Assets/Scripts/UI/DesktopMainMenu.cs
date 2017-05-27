using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DeusUtility;
using DeusUtility.UI;
namespace ITDmProject
{
    public class DesktopMainMenu : MonoBehaviour
    {
        protected enum MenuWindow { Nothing, Main, Options, About, Quit }
        public GUISkin Skin;
        private MenuWindow CurWin;
        UIWindowInfo[] Windows;
        private Vector2 scrollAboutPosition = Vector2.zero;
        int sphereRad;
        int objRad;
        GlobalControllerDesktop Global;
        bool langChanged;

        public Texture2D menuButtonBox;
        private Vector4 menuButtonBoxBorder;
        private Vector2 menuButtonBoxSize;

        void Start()
        {
            Global = FindObjectOfType<GlobalControllerDesktop>();
            Time.timeScale = 1;

            menuButtonBoxSize.x = 108;
            menuButtonBoxSize.y = 50;
            menuButtonBoxBorder.x = menuButtonBoxSize.x * 4 / 88;
            menuButtonBoxBorder.y = menuButtonBoxSize.y * 8 / 40;
            menuButtonBoxBorder.z = menuButtonBoxSize.x * 4 / 88;
            menuButtonBoxBorder.w = menuButtonBoxSize.y * 8 / 40;
            CurWin = MenuWindow.Nothing;
            Windows = new UIWindowInfo[5];
            //Windows[5] = new SDWindowInfo(new Rect(0, Screen.height-100, 100, 100));//info
            sphereRad = Global.Radius;
            objRad = Global.ObjectRadius;

        }
        private void Update()
        {
            //
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (CurWin == MenuWindow.Nothing)
                    CurWin = MenuWindow.Main;
                else
                    CurWin = MenuWindow.Nothing;
            }
            //
            Vector2 W4x4 = UIUtil.GetWindow(4, 4);
            Vector2 W8x6 = UIUtil.GetWindow(8, 6);
            Vector2 W6x2 = UIUtil.GetWindow(6, 2);
            Windows[0] = new UIWindowInfo(new Rect(new Vector2((Screen.width - W4x4.x) / 2, ((Screen.height - W4x4.y) / 2)), W4x4));//main
            Windows[1] = new UIWindowInfo(new Rect(new Vector2((Screen.width - W6x2.x) / 2, ((Screen.height - W6x2.y) / 2)), W6x2));//question
            Windows[2] = new UIWindowInfo(new Rect(new Vector2((Screen.width - W8x6.x) / 2, ((Screen.height - W8x6.y) / 2)), W8x6));//options
                                                                                                                                    //
        }
        void OnGUI()
        {
            GUI.skin = Skin;
            GUI.BeginGroup(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 100, 100));
            switch (CurWin)
            {
                case MenuWindow.Nothing:
                    {
                        break;
                    }
                case MenuWindow.Main:
                    {
                        GUI.Window(0, Windows[0].rect, DrawMainW, "");
                        break;
                    }
                case MenuWindow.Options:
                    {
                        GUI.Window(2, Windows[2].rect, DrawOptionsW, "");
                        break;
                    }
                case MenuWindow.About:
                    {
                        GUI.Window(2, Windows[2].rect, DrawAboutW, "");
                        break;
                    }
                case MenuWindow.Quit:
                    {
                        GUI.Window(1, Windows[1].rect, DrawQuitW, "");
                        break;
                    }
            }
            GUI.EndGroup();
            if (CurWin != MenuWindow.Nothing)
            {
                UIUtil.Exclamation(new Rect(0, Screen.height - 70, 200, 50), "Jogo Deus");
                UIUtil.Exclamation(new Rect(Screen.width - 200, Screen.height - 70, 200, 50), "v. " + Application.version);
            }
        }
        private void DrawMenuButton()
        {
            GUI.BeginGroup(new Rect(new Vector2(0, 0), menuButtonBoxSize));
            GUI.DrawTexture(new Rect(new Vector2(0, 0), menuButtonBoxSize), menuButtonBox);
            if (UIUtil.Button(new Rect(menuButtonBoxBorder.x, menuButtonBoxBorder.y,
                     menuButtonBoxSize.x - menuButtonBoxBorder.x - menuButtonBoxBorder.z,  //width
                     menuButtonBoxSize.y - menuButtonBoxBorder.y - menuButtonBoxBorder.w), //height
                     Global.Texts["Menu"]))
            {
                if (CurWin == MenuWindow.Nothing)
                    CurWin = MenuWindow.Main;
                else
                    CurWin = MenuWindow.Nothing;
            }
            GUI.EndGroup();
        }
        void DrawMainW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "Desktop");
            //GUI.color.a = window.UIAlpha;
            if (!Global.ServerUp)
            {
                if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 100, 200, 50), Global.Texts["Up"]))
                {
                    Global.RunServer();
                    CurWin = MenuWindow.Nothing;
                }
            }
            else
            {
                if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 100, 200, 50), Global.Texts["Down"]))
                {
                    Global.DownServer();
                }
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 150, 200, 50), Global.Texts["Options"]))
            {
                langChanged = false;
                CurWin = MenuWindow.Options;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 200, 200, 50), Global.Texts["About"]))
            {
                CurWin = MenuWindow.About;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 250, 200, 50), Global.Texts["Quit"]))
            {
                CurWin = MenuWindow.Quit;
            }
        }
        void DrawOptionsW(int windowID)
        {
            float fBuffer;
            int iBuffer;
            UIUtil.WindowTitle(Windows[windowID], Global.Texts["Options"]);

            GUI.BeginGroup(new Rect(Windows[windowID].CenterX - 170, 100, 340, 55));
            UIUtil.Label(new Rect(120, 0, 100, 20), Global.Texts["Duration"] + " - " + Global.Duration);
            fBuffer = Convert.ToSingle(Math.Round(GUI.HorizontalSlider(new Rect(0, 40, 340, 13), Global.Duration, 0.2f, 10f), 1));
            if (Global.Duration != fBuffer)
                Global.Duration = fBuffer;
            GUI.EndGroup();

            GUI.BeginGroup(new Rect(Windows[windowID].CenterX - 170, 165, 340, 55));
            UIUtil.Label(new Rect(120, 0, 100, 20), Global.Texts["Delay"] + " - " + Global.Delay);
            fBuffer = Convert.ToSingle(Math.Round(GUI.HorizontalSlider(new Rect(0, 40, 340, 13), Global.Delay, 0.0f, 10f), 1));
            if (Global.Delay != fBuffer)
                Global.Delay = fBuffer;
            GUI.EndGroup();

            GUI.BeginGroup(new Rect(Windows[windowID].CenterX - 170, 270, 340, 55));
            UIUtil.Label(new Rect(120, 0, 100, 20), Global.Texts["Sphere radius"] + " - " + Global.Radius);
            iBuffer = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(0, 40, 340, 13), Global.Radius, 10f, 1000f));
            if (Global.Radius != iBuffer)
                Global.Radius = iBuffer;
            GUI.EndGroup();

            GUI.BeginGroup(new Rect(Windows[windowID].CenterX - 170, 335, 340, 75));
            UIUtil.Label(new Rect(120, 0, 100, 20), Global.Texts["Object radius"] + " - " + Global.ObjectRadius);
            iBuffer = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(0, 40, 340, 13), Global.ObjectRadius, 1f, Global.Radius / 4));
            if (iBuffer > Global.Radius / 4) iBuffer = Global.Radius / 4;
            if (Global.ObjectRadius != iBuffer)
                Global.ObjectRadius = iBuffer;
            UIUtil.TextStyle1(new Rect(80, 55, 180, 20), Global.ObjectsInSphere(sphereRad, objRad).ToString() + " - objects");
            GUI.EndGroup();
            //GUI.BeginGroup(new Rect(Windows[windowID].CenterX - 100, 230, 200, 90));
            //UIUtil.Label(new Rect(50, 0, 100, 20), Global.Texts["Sphere radius"]);
            //sphereRad = ValidString.FormatIntRange(GUI.TextField(new Rect(0, 30, 200, 56), sphereRad.ToString(), 5), 10, 1000);
            //if (sphereRad != Global.Radius)
            //{
            //    Global.Radius = sphereRad;
            //}
            //GUI.EndGroup();
            //GUI.BeginGroup(new Rect(Windows[windowID].CenterX - 100, 320, 200, 120));
            //UIUtil.Label(new Rect(50, 0, 100, 20), Global.Texts["Object radius"]);
            //objRad = ValidString.FormatIntRange(GUI.TextField(new Rect(0, 30, 200, 56), objRad.ToString(), 5), 0, sphereRad);
            //string maxS;
            //if (Global.ObjectsInSphere(sphereRad, objRad) != -1)
            //{
            //    maxS = Global.ObjectsInSphere(sphereRad, objRad).ToString() + " - objects";
            //    if (objRad != Global.ObjectRadius)
            //    {
            //        Global.ObjectRadius = objRad;
            //    }
            //}
            //else
            //    maxS = "Invalid value";
            //UIUtil.TextStyle1(new Rect(10, 80, 180, 20), maxS);
            //GUI.EndGroup();


            string[] radios = new string[2];
            radios[0] = "English";
            radios[1] = "Русский";
            int radioSelected = (int)Global.Localisation;
            GUI.BeginGroup(new Rect(77, 380, 100, 110));
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
                if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(100), Windows[windowID].Bottom - 100, UIUtil.Scaled(200), 50), Global.Texts["Back"]))
                {
                    CurWin = MenuWindow.Main;
                    if (langChanged)
                        Global.LoadLocalisationTexts();
                }
            }
            else
            {
                if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(100), Windows[windowID].Bottom - 100, UIUtil.Scaled(200), 50), Global.Texts["Save"]))
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

            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX + 130, Windows[windowID].Bottom - 100, 230, 50), Global.Texts["Developer page"]))
            {
                Application.OpenURL("https://www.linkedin.com/in/%D0%B4%D0%B0%D0%BD%D0%B8%D0%B8%D0%BB-%D1%87%D0%B8%D0%BA%D0%B8%D1%88-5809a2108/");
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, Windows[windowID].Bottom - 100, 200, 50), Global.Texts["Back"]))
            {
                CurWin = MenuWindow.Main;
            }
        }
        void DrawQuitW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.Texts["Quit_question"]);
            //UIUtil.Label(new Rect(50, 10, 180, 43), "Running in fear?");
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX + UIUtil.Scaled(10), 100, UIUtil.Scaled(180), 50), Global.Texts["Yes"]))
            {
                Application.Quit();
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(190), 100, UIUtil.Scaled(180), 50), Global.Texts["No"]))
            {
                CurWin = MenuWindow.Main;
            }
        }
        //void DrawInfo(int windowID)
        //{
        //    UIUtil.Exclamation(new Rect(0, Screen.height - 200, 100, 200), "Jogo Deus");
        //}
    }
}