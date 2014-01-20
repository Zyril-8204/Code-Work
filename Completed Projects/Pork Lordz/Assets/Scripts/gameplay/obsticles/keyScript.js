public var door : GameObject;

function Update() {

}

function OnTriggerEnter(theCollision : Collider)
{
    Destroy(door);
    Destroy(this.gameObject);
}