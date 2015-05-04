private var walkingForce = 20;			//Applied force when walking
private var dashForce = 70;				//Applied force when dashing
private var crouchingForce = 6;	
private var repeatThreshold = 0.25f;		//Threshold for recongnizing double taps
private var jumpForce = 40;
private var jumpTime = 0.4f;
private var maxSpeed = 0;
private var maxSpeedWalking = 10;
private var maxSpeedDash = 20;
private var maxSpeedCrouch = 6;

public var currentJumpTime = 0.0f;
private var currentRepeatTime = 0.0f;	//Tracks space inbetween button presses for dashing
private var moveForce = 0;				//Current applied force; changes depending on whether walking or dashing.
public var dashing : boolean = false;	//Flags whether dashing or not
private var ducking : boolean = false;
private var canStand : boolean = false;

private var startFall = 0.0f;
private var lastY = 0.0f;
private var fallingThreshold = 30.0f;
public var falling : boolean = false;
private var fallDamage = 5;

private var hit : RaycastHit; //struct used for determining objects hit by raycast (used for passthrough platforms and such)

var slot : Texture;
var health = 100.0f;
var maxHealth = 100.0f;


function Start ()
{
	lastY = transform.position.y;
}

function Update () {

	//Adjust movement force depending on speed;
	if (dashing)
	{
		moveForce = dashForce;
		maxSpeed = maxSpeedDash;
	}
	else if (ducking)
	{
		moveForce = crouchingForce;
		maxSpeed = maxSpeedCrouch;
	}
	else
	{
		moveForce = walkingForce;
		maxSpeed = maxSpeedWalking;
	}


	//Add force depending on direction
	if (Input.GetKey(KeyCode.A))
	{
		dashingCheck();
		
		if (rigidbody.velocity.x > -maxSpeed)
		{
			rigidbody.AddForce(-moveForce,0,0);
		}
	}
	else if (Input.GetKey(KeyCode.D))
	{
		dashingCheck();
		
		if (rigidbody.velocity.x < maxSpeed)
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
        if (Physics.Raycast(transform.position, Vector3(0, -1, 0), hit))
        {
            if (hit.collider.tag == "Fallthrough")
            {
                hit.collider.gameObject.GetComponent("platformFallthrough").makeTrue();
            }
        }
	}
	else if (!Physics.Raycast(transform.position + Vector3(0.5f,0,0),transform.up,1) &&
			!Physics.Raycast(transform.position - Vector3(0.5f,0,0),transform.up,1))
	{
		ducking = false;
		if (transform.localScale.y == 0.5f)
		{
			transform.localScale.y = 1;
			transform.position.y += 0.5f;
		}
	}

	//Check if falling 
	if (lastY >= transform.position.y)
	{
		if (!falling)
		{
			falling = true;
			startFall = lastY;
		}
	}
	else
	{
		if (Mathf.Abs(startFall - transform.position.y) > fallingThreshold && falling)
		{
			gameControl.health -= fallDamage;
		} 
		falling = false;
	}
	
	
	lastY = transform.position.y;

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

function OnGUI()
{
	gameControl.drawHealth();
	//GUI.DrawTexture(Rect(10,Screen.height - 32,128,32), pixel);
	//GUI.DrawTexture(Rect(10,Screen.height - 32,128.0f * (gameControl.health / gameControl.maxHealth) ,32), healthBar);
	//GUI.Label(Rect(10,Screen.height - 32,128,32), gameControl.health.ToString());
}