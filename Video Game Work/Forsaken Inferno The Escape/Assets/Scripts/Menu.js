public var theTexture : Texture2D;

function OnGUI()
{
	GUI.DrawTexture(Rect(0,0,Screen.width, Screen.height), theTexture);
		
	if (GUI.Button(Rect((Screen.width/2),(Screen.height/2)-50,100,80),"Start"))
	{
		Application.LoadLevel(2);
	}
	if (GUI.Button(Rect((Screen.width/2),(Screen.height/2)+50,100,80),"Quit"))
	{
		Application.Quit();
	}
	
}