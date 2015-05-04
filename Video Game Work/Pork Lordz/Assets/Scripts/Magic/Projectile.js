var explosion: GameObject;
var fireExplosion: GameObject;
var iceExplosion: GameObject;
var lightningExplosion: GameObject;

function OnCollisionEnter(collision : Collision)
{
	var contact : ContactPoint = collision.contacts[0];
	var rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
	
	if(Inventory.currentMagic.objName == "Hellfire Tome")
	{
		var instantiatedFireExplosion : GameObject = Instantiate(fireExplosion,contact.point,rotation);
		Destroy(fireExplosion);
		
	}
	else if(Inventory.currentMagic.objName == "Blizzard Tome")
	{
		var instantiatedIceExplosion : GameObject = Instantiate(iceExplosion,contact.point,rotation);
		Destroy(iceExplosion);
	}
	else if(Inventory.currentMagic.objName == "Zeus Tome")
	{
		var instantiatedLightningExplosion : GameObject = Instantiate(lightningExplosion,contact.point,rotation);
		Destroy(lightningExplosion);
	}
	
}
function Update () {
}