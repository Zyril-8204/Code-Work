public var force: float = 1000;
public var damage: int = 5;
private var timer: float = 0;

function Start()
{
	transform.LookAt(GameObject.FindWithTag("Player").transform);
	rigidbody.AddForce(transform.forward * force);
}


function Update () {

timer += Time.deltaTime;
if (timer > 3)
{
	Destroy(gameObject);
}

}

function OnTriggerStay (other : Collider) {

	if (other.gameObject.tag == "Player")
	{
		gameControl.health -=damage;
		print("hitPLAYER");
		Destroy(gameObject);
	}
}