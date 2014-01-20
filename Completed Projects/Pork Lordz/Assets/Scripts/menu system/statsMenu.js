public var theTexture : Texture2D;
public var buttonStyle: GUIStyle;

function OnGUI()
{
	GUI.DrawTexture(Rect(0,0,Screen.width, Screen.height), theTexture);
	
	if (GUI.Button(Rect(Screen.width - 128,0,128,64),"Return",buttonStyle))
	{
		Application.LoadLevel("SceneMenu");
	}
	
	if (GUI.Button(Rect(1000,650,50,30),"Reset"))
	{
		Debug.Log("Reset stats!");
	}
}