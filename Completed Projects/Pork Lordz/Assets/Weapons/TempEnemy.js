#pragma strict
public var hp: int = 10;

function Start () {

}

function Update () {

	if (hp <= 0)
	{
		GameObject.Destroy(gameObject);
	}

}

function OnTriggerStay (other : Collider) {
	if (other.gameObject.tag == "Weapon")
	{
		if (!(other.GetComponent(Swinging)).inactive)
		{		
			(other.GetComponent(Swinging)).inactive = true;
			
			hp -= Inventory.currentWeapon.power;
						
			if (WeaponControl.attackBonus == true)
			{
					if (Swinging.attackType == 0)//Thrust -> knocks back
					{
						WeaponControl.successiveHits += 1;
						rigidbody.AddForce(Vector3(0,WeaponControl.knockbackPower,0) + other.transform.forward * -WeaponControl.knockbackPower);
					}
					else if (Swinging.attackType == 1)//HighSwing -> flings upwards
					{
						WeaponControl.successiveHits += 1;
						rigidbody.AddForce(Vector3(0,WeaponControl.knockbackPower * 2,0));
						Debug.Log("Flung up");
					}
					else if (Swinging.attackType == 2)//Swing -> Increase chances of being knocked back or flung up
					{
						WeaponControl.successiveHits += 1;
						Debug.Log("knock back");
					}
			}
		}
	}
		
}
