using UnityEngine;
using System.Collections;

public class ScriptStatMenu : MonoBehaviour {
	
	public string menuSceneName;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
		
	{
		if (GUI.Button(new Rect	(10,10,100,64),"Return"))
		{
			Application.LoadLevel(menuSceneName);	
		}
	}
}
