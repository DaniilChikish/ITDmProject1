using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ITDmProject;

public class MobileInterfaceEmulator : MonoBehaviour {
    GlobalControllerDesktop Global;
    public float duration;
    public int maxCommand;
    private float backCount;
    private int comandCount;
	// Use this for initialization
	void Start () {
        Global = FindObjectOfType<GlobalControllerDesktop>();
        backCount = 10;
        comandCount = 0;
	}

    // Update is called once per frame
    void Update()
    {
        if (comandCount < maxCommand)
        {
            if (backCount > 0)
                backCount -= Time.deltaTime;
            else
            {
                Global.AddNCaptere("Com" + comandCount);
                backCount = duration / maxCommand;
                comandCount++;
            }
        }
    }
}
