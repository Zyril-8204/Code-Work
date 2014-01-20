public var damage = 20;
public var onceOnly = false;
function Update() 
{
}

function OnTriggerEnter(theCollision : Collider)
{
    if(theCollision.gameObject.tag == "Player")
    {
        //Debug.Log("Player collision");
        gameControl.health -= this.damage * Inventory.currentShield.defense;
    }
    if(onceOnly)
    {
        Destroy(gameObject);
    }
}