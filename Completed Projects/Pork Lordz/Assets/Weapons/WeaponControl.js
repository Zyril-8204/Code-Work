#pragma strict

public var weapons: GameObject[];
public var lastWeapon: int = -1;
static var attackBonus: boolean = false;
static var successiveHits: int = 0;
static var knockbackPower: int = 500;
private var chanceCap: int = 10;

private var bonusTimer:float = 0.0f;
private var lastHitCount = 0;

function Start () {

	lastWeapon = Inventory.currentWeapon.id;
	ClearGraphic();
    if (lastWeapon != -1)
    {
	    weapons[lastWeapon].SetActiveRecursively(true);
        }
}

function Update () {

	//Resets the counter that tracks your successive hits if it remains the same for too long
	if (lastHitCount != successiveHits)
	{
		bonusTimer = 0;
		lastHitCount = successiveHits;
	}
	else
	{
		bonusTimer += Time.deltaTime;
	}
	
	if (bonusTimer > Inventory.currentWeapon.speed * 5)
	{
		successiveHits = 0;
	}
	
	

	if (lastWeapon != Inventory.currentWeapon.id)
	{
		lastWeapon = Inventory.currentWeapon.id;
		Swinging.lastAttackTime = 0;
		ClearGraphic();
		weapons[lastWeapon].SetActiveRecursively(true);
	}
	
	if (Input.GetMouseButton(0))
	{
		var rando = Random.Range(0, chanceCap);
		//print(successiveHits.ToString());
		//print(successiveHits.ToString() + "/" + chanceCap.ToString());
		if (successiveHits >= rando)
		{
			attackBonus = true;
			//print(rando.ToString() + " = Bonus");
		}
		
		
	}
}

function ClearGraphic()
{
	for (var i = 0; i < 3 ; i++)
	{
		weapons[i].SetActiveRecursively(false);
	}
}

function OnGUI()
{
	var style = new GUIStyle();
	style.normal.textColor = Color.red;
	style.font = gameControl.font;
	style.fontSize = 20;
	style.alignment = TextAnchor.MiddleRight;
	GUI.Label(Rect(Screen.width - 10 - 128, 0 + 16, 128,32),successiveHits.ToString(), style);
}