public static var EMPTY 	= 0;
public static var WEAPONTYPE 	= 1;
public static var SHIELDTYPE 	= 2;
public static var MAGICTYPE 	= 3;
public static var FOODTYPE 		= 4;

//Inventory Variables
static public var currentWeapon 	: Item = new Item();
static public var currentShield 	: Item = new Item();
static public var currentMagic 		: Item = new Item();
static public var food = 0;



function Update () {


}

static public function giveItem(item : Item)
{
	switch (item.type)
	{
		case WEAPONTYPE: 	currentWeapon 	= item; break;
		case SHIELDTYPE: 	currentShield 	= item; break;
		case MAGICTYPE: 	currentMagic	= item; break;
		case FOODTYPE: 		food 			+= 1; 	break;
		default: Debug.Log("PickupError: Please set item type to 1-4");
	}
}