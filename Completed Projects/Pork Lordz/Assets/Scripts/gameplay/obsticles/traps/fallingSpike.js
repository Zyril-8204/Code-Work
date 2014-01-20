public var fallingSpike:GameObject;


function Update () {

}

function OnTriggerStay(other:Collider)
{
	//Debug.Log(other.name);
	if (other.gameObject.tag == "Player")
	{
    	fallingSpike.rigidbody.useGravity = true;
    	Destroy(this.gameObject);
    }
}