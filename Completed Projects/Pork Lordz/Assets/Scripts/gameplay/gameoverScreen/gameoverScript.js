public var theTexture : Texture2D;

function OnGUI()
{
	GUI.DrawTexture(Rect(0,0,Screen.width, Screen.height), theTexture);
	
	if (GUI.Button(Rect(500,400,50,30),"Yes"))
	{
		Application.LoadLevel("SceneMenu");
	}
	
	if (GUI.Button(Rect(800,400,50,30),"No"))
	{
		Application.Quit();
	}
}