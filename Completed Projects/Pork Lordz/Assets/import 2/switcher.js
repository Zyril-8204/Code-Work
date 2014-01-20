#pragma strict

function OnCollisionEnter (col: Collision) {

if (col.gameObject.name == "Button")
	{
		gameObject.Find("MovingPlatform").SendMessage("elevatorReact");
	}
if (col.gameObject.name == "bridgePart1")
	{
		gameObject.Find("bridgePart1").SendMessage("bridgeReact");
		yield WaitForSeconds(.5);
		gameObject.Find("bridgePart2").SendMessage("bridgeReact");
		yield WaitForSeconds(.1);
		gameObject.Find("bridgePart3").SendMessage("bridgeReact");
		yield WaitForSeconds(.1);
		gameObject.Find("bridgePart4").SendMessage("bridgeReact");
		yield WaitForSeconds(.1);
		gameObject.Find("bridgePart5").SendMessage("bridgeReact");
		yield WaitForSeconds(.1);
		gameObject.Find("bridgePart6").SendMessage("bridgeReact");
		yield WaitForSeconds(.1);
		gameObject.Find("bridgePart7").SendMessage("bridgeReact");
		yield WaitForSeconds(.1);
		gameObject.Find("bridgePart8").SendMessage("bridgeReact");
		yield WaitForSeconds(.1);
		gameObject.Find("bridgePart9").SendMessage("bridgeReact");
		yield WaitForSeconds(.1);
		gameObject.Find("bridgePart10").SendMessage("bridgeReact");
		yield WaitForSeconds(.1);
		gameObject.Find("bridgePart11").SendMessage("bridgeReact");
		
	}
}
