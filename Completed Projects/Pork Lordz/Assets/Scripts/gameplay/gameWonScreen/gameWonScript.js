public var theTexture : Texture2D;

function OnGUI()
{
	GUI.DrawTexture(Rect(0,0,Screen.width, Screen.height), theTexture);
	
	if (GUI.Button(Rect(Screen.width / 2,Screen.height - 40,50,30),"Yes"))
	{
		RoomLink.inPosition = null;
		Application.LoadLevel("SceneMenu");
	}
	
	if (GUI.Button(Rect(Screen.width / 2 + 60,Screen.height - 40,50,30),"No"))
	{
		Application.Quit();
	}
}