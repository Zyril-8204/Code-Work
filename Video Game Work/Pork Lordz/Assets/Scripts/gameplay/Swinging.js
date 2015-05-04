public var weaponScriptRef: WeaponControl;
public var inactive : boolean = true;
static var lastAttackTime: float = 0;
static var attackType: int = 0;


function Start()
{

}

function Update () {

		if (!animation.IsPlaying("Swing") && !animation.IsPlaying("HighSwing") &&  !animation.IsPlaying("Thrust"))
		{
			inactive = true;
		}
	
		if(Application.loadedLevelName == "2DWorld")
		{
		
			if (lastAttackTime < Inventory.currentWeapon.speed)
			{
				lastAttackTime += Time.deltaTime;
			}
			else if(Input.GetMouseButtonDown(0))
			{
				if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
				{
					animation.Play("Thrust");
					attackType = 0;
				}
				else if (Input.GetKey(KeyCode.S))
				{
					animation.Play("HighSwing");
					attackType = 1;
				}
				else
				{
					animation.Play("Swing");
					
					attackType = 2;
				}
				
				if (weaponScriptRef.weapons[weaponScriptRef.lastWeapon] == gameObject)
				{
					inactive = false;
				}
				
				lastAttackTime = 0;
				
			}
		
//			animation.Play("Swing");
//		}
//		if(Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.W))
//		{
//		animation.Play("HighSwing");
//		}
//		if(Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.S))
//		{
//			animation.Play("Swing");
		
		
	}
	else if(Application.loadedLevelName == "3DDungeon" || Application.loadedLevelName == "BossRoom") 
	{
		if (lastAttackTime < Inventory.currentWeapon.speed)
		{
			lastAttackTime += Time.deltaTime;
		}
		else if(Input.GetMouseButtonDown(0))
		{		
			if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
			{
				animation.Play("Thrust");
				attackType = 0;
			}
			else if (Input.GetKey(KeyCode.LeftShift))
			{
				animation.Play("HighSwing");
				attackType = 1;
			}
			else
			{
				animation.Play("Swing");
				
				attackType = 2;
			}	
				
			if (weaponScriptRef.weapons[weaponScriptRef.lastWeapon] == gameObject)
			{
				inactive = false;
			}
			
			lastAttackTime = 0;
		}
	}
	
}