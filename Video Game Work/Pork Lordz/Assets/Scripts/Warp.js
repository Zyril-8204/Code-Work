#pragma strict
private var warp: Transform;

function Start () {

	warp = GameObject.FindWithTag("Warp").GetComponent(Transform) as Transform;
	warp.position = transform.position;
}

function Update () {

	if (Input.GetKey(KeyCode.Alpha5))
	{
		warp.position = transform.position;
	}
	
	if (Input.GetKey(KeyCode.Alpha7))
	{
		transform.position = warp.position;
	}
	
	if (Input.GetKey(KeyCode.Alpha1))
	{
		gameControl.health = 9999999;
	}

}