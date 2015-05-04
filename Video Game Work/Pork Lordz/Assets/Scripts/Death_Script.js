#pragma strict

private var time = 42;
private var timer = 0;
function Start () {

}

function Update () {

	if (timer <= time)
	{
		timer++;
	}
	else
	{
		Destroy(this.gameObject);
	}
}