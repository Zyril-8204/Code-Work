#pragma strict

function Start () {

}

function Update () {


	if (gameControl.health <= 0)
	{
		Application.LoadLevel("GameOverScene");
	}

}