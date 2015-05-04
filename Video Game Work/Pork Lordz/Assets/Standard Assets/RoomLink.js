public var room : String = "roomName";
public var outPosition : Transform;
public var player : Transform;
static var inPosition = null;

function Start(){

	if (inPosition != null)
	{
		player.position = inPosition.position;
	}
}

function Update () {
}

function OnTriggerEnter (other : Collider) {
	if (other.gameObject.tag == "Player")
	{
		inPosition = outPosition;
    	Application.LoadLevel(room);
    }
}