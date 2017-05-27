using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ITDmProject
{
public class Word : MonoBehaviour {
	public string ID;
	private GlobalControllerDesktop GC;
	private void Start()
	{
		GC = GameObject.FindObjectOfType<GlobalControllerDesktop> ();
		//GC.words.Add (this.ID, this.gameObject.transform);
		//GC.keys.Add (this.ID);
	}
	public void Instant(string id, string text)
	{
		this.ID = id;
		this.transform.GetComponentInChildren<TextMesh> ().text = text;
	}
}
}
