using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HardDefaultStorage
{
    public static Dictionary<string, string> GetLocalisationDefault()
    {
        Dictionary<string, string> outp = new Dictionary<string, string>();
        string[] values = new string[] {"Yes", "No","Back","Continue", "Pause",
            "Play","Start","Options","Resolution","Sound","Music","Language","Save",
            "About","Quit","Run","Discover","Refresh","Connect","Disconnect","Up",
            "Down","Quit_question","Restart","Next","Menu","Main menu","Development",
            "Develop_content","Developer page","Duration","Delay","Radius","Object radius",
            "Sphere radius","Server name","Send"};
        foreach (string x in values)
            outp.Add(x, x);
        return outp;
    }
}
