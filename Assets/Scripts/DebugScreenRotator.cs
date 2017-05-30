using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScreenRotator : MonoBehaviour {
    public ScreenOrientation orientation;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Screen.orientation = orientation;
	}
}
