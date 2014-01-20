#pragma strict
var leftAngle: int = 180.0f;
var rightAngle: int = 0.0f;
private var delta: float = 10;
private var turnSpeed: int = 10;
private var direction: int = 0;

function Start () {

}

function Update () {

	if (Input.GetKey(KeyCode.A))
	{
		direction = leftAngle;
	}
	else if (Input.GetKey(KeyCode.D))
	{
		direction = rightAngle;
	}
	
	//print(Mathf.Abs(direction - transform.localEulerAngles.y));//transform.localEulerAngles.y);
	if (Mathf.Abs(direction - transform.localEulerAngles.y) > delta)
	{
		transform.RotateAround(transform.up, turnSpeed * Time.deltaTime);
	}
}