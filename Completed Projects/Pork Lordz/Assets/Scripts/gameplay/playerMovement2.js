private var walkingForce = 10;			//Applied force when walking
private var dashForce = 30;				//Applied force when dashing
private var maxSpeed = 10;				//Max movement speed
private var repeatThreshold = 0.25f;		//Threshold for recongnizing double taps
private var jumpForce = 30;
private var jumpTime = 0.4f;

public var currentJumpTime = 0.0f;
private var currentRepeatTime = 0.0f;	//Tracks space inbetween button presses for dashing
private var moveForce = 0;				//Current applied force; changes depending on whether walking or dashing.
private var dashing : boolean = false;	//Flags whether dashing or not
private var ducking : boolean = false;
private var canStand : boolean = false;

function Start ()
{

}

function Update () {

	//Adjust movement force depending on speed;
	if (dashing)
	{
		moveForce = dashForce;
	}
	else if (ducking)
	{
		moveForce = 6;
	}
	else
	{
		moveForce = walkingForce;
	}


	//Add force depending on direction
	if (Input.GetKey(KeyCode.A))
	{
		dashingCheck();
		
		if (rigidbody.velocity.x > -moveForce)
		{
			rigidbody.AddForce(-moveForce,0,0);
		}
	}
	else if (Input.GetKey(KeyCode.D))
	{
		dashingCheck();
		
		if (rigidbody.velocity.x < moveForce)
		{
			rigidbody.AddForce(moveForce,0,0);
		}
	}
	else
	{
		dashing = false;
	
		if (currentRepeatTime < repeatThreshold)
		{
			currentRepeatTime += Time.deltaTime;
		}
	}
	
	//Jumping
	if (Input.GetKey(KeyCode.Space) && currentJumpTime < jumpTime)
	{
		rigidbody.AddForce(0,jumpForce,0);
		currentJumpTime += Time.deltaTime;
	}
	
	if (!Input.GetKey(KeyCode.Space) && currentJumpTime > 0)
	{
		currentJumpTime = jumpTime;
	}
	
	if (Physics.Raycast(transform.position,-transform.up,1))
	{
		currentJumpTime = 0;
	}
	
	//Ducking
	if (Input.GetKey(KeyCode.S))
	{
		dashing = false;
		ducking = true;
		if (transform.localScale.y == 1)
		{
			transform.localScale.y = 0.5f;
			transform.position.y -= 0.5f;
		}
	}
	else if (!Physics.Raycast(transform.position,transform.up,5))
	{
		ducking = false;
		if (transform.localScale.y == 0.5f)
		{
			transform.localScale.y = 1;
			transform.position.y += 0.5f;
		}
	}

}

function dashingCheck()
{
		if (currentRepeatTime > 0 && currentRepeatTime < repeatThreshold)
		{
			dashing = true;
		}
		else
		{
			currentRepeatTime = 0;
		}
}

//Reset Jumping if standing on solid
function OnCollisionEnter(collision : Collision)
{
}