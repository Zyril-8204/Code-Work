static var health : float;
static var maxHealth : float;
static var slotSize : int; 
static var crouchScale: float;

static var pixelTexture : Texture;
static var healthBarTexture : Texture;
static var slot : Texture;
static var foodTexture : Texture;
static var foodEatenTexture : Texture;
static var font;

static var maxHealthBarLength : int;
static var healthBar : Rect;

//static public var score = 0;
static public var canEat : boolean;
static var hasLoaded = false;

function Update () {

}

static function Reset()
{
	RoomLink.inPosition = null;
	Stats.score = 0;
	Stats.kills = 0;
	Stats.items = 0;
	Inventory.currentWeapon = new Item();
	Inventory.currentShield = new Item();
	Inventory.currentMagic = new Item();
	Inventory.food = 0;
	
	health = 100.0f;
	maxHealth = 100.0f;
	slotSize = 64;

	pixelTexture = Resources.Load("Pixel");
	healthBarTexture = Resources.Load("HealthBar");
	slot = Resources.Load("InventorySlot");
	foodTexture = Resources.Load("Food");
	foodEatenTexture = Resources.Load("FoodEaten");
	font = Resources.Load("GothicFlames");

	maxHealthBarLength = 128;
	healthBar = Rect(5,Screen.height - 37,32,32);

//static public var score = 0;
	canEat = true;
	hasLoaded = true;
	
	
}

static public function drawHealth()
{
	var style = new GUIStyle();
	style.alignment = TextAnchor.MiddleRight;
	style.normal.textColor = Color.green;
	style.font = font;
	style.fontSize = 20;
	style.alignment = TextAnchor.MiddleLeft;
	

	//draw Health Bar
	GUI.DrawTexture(Rect(10,Screen.height - 32,128,32), gameControl.pixelTexture);
	GUI.DrawTexture(Rect(10,Screen.height - 32,128.0f * (gameControl.health / gameControl.maxHealth) ,32), gameControl.healthBarTexture);
	//GUI.Label(Rect(10,Screen.height - 32,128,32), gameControl.health.ToString());
	if (canEat)
		GUI.DrawTexture(Rect(148,Screen.height - 32,32,32),foodTexture);
	else
		GUI.DrawTexture(Rect(148,Screen.height - 32,32,32),foodEatenTexture);
	
	GUI.Label(Rect(188,Screen.height - 32,32,32),Inventory.food.ToString(),style);

	
	//Cooldown area
		//GUI.Label(Rect(10, Screen.height - 64, 128,32),gameControl.eatingCoolDown.ToString());
	
	//Draw Inventory Slots
	var temp = Rect(Screen.width / 2 - slotSize,Screen.height - slotSize,  slotSize, slotSize);
	GUI.DrawTexture(temp, slot); 
	
	if (Inventory.currentWeapon.texture != null)
		GUI.DrawTexture(temp, Inventory.currentWeapon.texture);
	temp.x+=slotSize;
	GUI.DrawTexture(temp, slot); 
	if (Inventory.currentShield.texture != null)
		GUI.DrawTexture(temp, Inventory.currentShield.texture);
	temp.x+=slotSize;
	GUI.DrawTexture(temp, slot);
	if (Inventory.currentMagic.texture != null)
		GUI.DrawTexture(temp, Inventory.currentMagic.texture);
	
	//Draw Score
	
	style.alignment = TextAnchor.MiddleRight;
	
	GUI.Label(Rect(Screen.width - 138, Screen.height - 32, 128,32),Stats.score.ToString(), style);
	

}

static public function startGame() {

	RoomLink.inPosition = null;
	Stats.score = 0;
	canEat = true;
	health = maxHealth;
	Inventory.currentWeapon = new Item();
	Inventory.currentShield = new Item();
	Inventory.currentMagic = new Item();
	Inventory.food = 0;
	
	
}

