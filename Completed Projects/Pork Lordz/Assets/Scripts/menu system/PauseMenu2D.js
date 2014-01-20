public var paused:boolean = false;
public var drawGUI:boolean = false;
public var theTexture : Texture2D;
var inventoryTexture: Texture;
var pixelTexture: Texture;

var pauseRect: Rect = new Rect((Screen.width * 0.5f) - (Screen.width * 0.1f), (Screen.height * 0.5f) * 0.5f,
			                 Screen.width * 0.2f, Screen.height * 0.5f);	
			                 
var buttonStyle;
var style = new GUIStyle();
			  
function Start()
{
	inventoryTexture = Resources.Load("InventorySlot");
	buttonStyle = Resources.Load("PorkButtons");
	pixelTexture = Resources.Load("Pixel");
	style.alignment = TextAnchor.UpperLeft;
	style.normal.textColor = Color.green;
	style.font = Resources.Load("GothicFlames");
	style.fontSize = 24;
}
			                 
function Update () 
{

	if(Input.GetKeyUp(KeyCode.Escape))
	{
		paused = togglePauseMenu();
		AudioListener.pause = paused;			
	}
	if(paused)
	{
		drawGUI = true;
	}
	else
	{
        drawGUI= false;
	}
}

function togglePauseMenu() : boolean
{
	if(Time.timeScale == 1.0F)
	{
		Time.timeScale = 0.0F;
		return(true);
	}
	else
	{
		Time.timeScale = 1.0F;
		return(false);
	}
}

function OnGUI()
{
	GUI.depth = 100;
	if (drawGUI == true)
	{
		GUI.skin = buttonStyle;
		//Draw Background
		GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b, .5f);
		GUI.DrawTexture(Rect(0,0,Screen.width + 5,Screen.height + 1),pixelTexture);
		GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b, 1.0f); 
		
		//Draw TextBox
		GUI.DrawTexture(Rect(0,0,Screen.width - inventoryTexture.width, Screen.height), theTexture);
		
		//Draw Lables
		var tempRect: Rect = new Rect(128,64,Screen.width - inventoryTexture.width - 128,96);
		
		if (Inventory.currentWeapon.texture != null)
			GUI.Label(tempRect,Inventory.currentWeapon.objName + ":\n" + Inventory.currentWeapon.description ,style);
		tempRect.y+= inventoryTexture.height;
		if (Inventory.currentShield.texture != null)
			GUI.Label(tempRect,Inventory.currentShield.objName + ":\n" + Inventory.currentShield.description ,style);
		tempRect.y+= inventoryTexture.height;
		if (Inventory.currentMagic.texture != null)
			GUI.Label(tempRect,Inventory.currentMagic.objName + ":\n" + Inventory.currentMagic.description ,style);
			
		
		//Draw Inventory Slots
		tempRect = new Rect(Screen.width - inventoryTexture.width,64,inventoryTexture.width,inventoryTexture.height);
	
		GUI.DrawTexture(tempRect,inventoryTexture);
		if (Inventory.currentWeapon.texture != null)
			GUI.DrawTexture(tempRect, Inventory.currentWeapon.texture);
		tempRect.y += inventoryTexture.height;
			
		GUI.DrawTexture(tempRect,inventoryTexture);
		if (Inventory.currentShield.texture != null)
			GUI.DrawTexture(tempRect, Inventory.currentShield.texture);
		tempRect.y += inventoryTexture.height;
			
		GUI.DrawTexture(tempRect,inventoryTexture);
		if (Inventory.currentMagic.texture != null)
			GUI.DrawTexture(tempRect, Inventory.currentMagic.texture);
		tempRect.y += inventoryTexture.height;
		
		if (GUI.Button(Rect(0,Screen.height-64,128,64),"BACK")) // retrurn
		{
        	drawGUI= false;
        	paused = false;
        	Time.timeScale = 1.0F;
		}
		if (GUI.Button(Rect(Screen.width - inventoryTexture.width - 128,Screen.height-64,128,64),"QUIT")) // quit
		{
			RoomLink.inPosition = null;
        	drawGUI= false;
        	paused = false;
        	Time.timeScale = 1.0F;
        	Application.LoadLevel("SceneMenu");
		}
		GUI.contentColor = Color.black;
	}
	GUI.depth = 0;
}