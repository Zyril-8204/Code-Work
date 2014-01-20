public var theTexture : Texture2D;
private var StartTime : float;

function OnGUI()
{
	// set the color of the GUI
	GUI.color = Color.white;	
	// interpolate the alpha of the GUI from 1(fully visible)
	// to 0(invisible) over time
	GUI.color.a = Mathf.Lerp(1.0, 0.0, (Time.time/6));
	// draw the texture to fill the screen
	GUI.DrawTexture(Rect(0,0,Screen.width, Screen.height), theTexture);
	if (GUI.color.a <= 0)
	{
		Application.LoadLevel(1);
	}
}