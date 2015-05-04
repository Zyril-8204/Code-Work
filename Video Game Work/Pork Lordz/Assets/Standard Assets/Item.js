public var texture: Texture;
public var type: int		 	= 5;
public var objName 				= "Item";
public var description			= "Description";


//Weapon Stats
public var id: int = -1;
public var power: int = 0;
public var speed: float = 0;
public var defense: float = 1;

private var showGUI: boolean = false;
static var style = new GUIStyle();

function Start()
{
	style.alignment = TextAnchor.MiddleCenter;
	style.normal.textColor = Color.green;
	style.font = Resources.Load("GothicFlames");
	style.fontSize = 24;
}

function Item()
{
	texture = null;
	type = 0;
	objName = "Item";
	id = -1;
	defense = 1;
}

function OnTriggerStay (other : Collider) {

	if (other.gameObject.tag == "Player")
	{
		showGUI = true;
		
		if (other.transform.localScale.y == gameControl.crouchScale)//(Input.GetKey(KeyCode.LeftShift))
		{
			Stats.score += 2;
			Inventory.giveItem(this as Item);
			gameObject.Destroy(gameObject);
			Stats.items ++;
		}
	}

}

function OnTriggerExit (other : Collider) {
	if (other.gameObject.tag == "Player")
	{
    	showGUI = false;
    }
}

function OnGUI()
{
	if (showGUI) 
	{
		GUI.DrawTexture(Rect(Screen.width/2 - 50,Screen.height/2 - 50,100,100),texture);
		GUI.Label(Rect(Screen.width/2 - 100,Screen.height/2 + 50,200,100),objName,style);
	}
}