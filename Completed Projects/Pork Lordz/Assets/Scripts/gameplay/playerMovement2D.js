public var walkingForce = 20;			//Applied force when walking
private var dashForce = 20;				//Applied force when dashing
private var crouchingForce = 20;	
private var repeatThreshold = 0.25f;		//Threshold for recongnizing double taps
public var jumpForce = 50;
public var superJumpForce = 100;
private var bloodObject: GameObject;
private var currentHealth: float;		//For detecting when to spawn blood;


public var jumpTime = 0.2f;
private var maxSpeed = 0;
private var maxSpeedWalking = 10;
private var maxSpeedDash = 10;
private var maxSpeedCrouch = 6;
private var startingScale: float;
static var crouchScale: float;

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

var slot : Texture;
var health = 100.0f;
var maxHealth = 100.0f;

private var eatingCoolDown: float = 30.0f;
private var currentEatingCoolDown: float = 0.0f;


function Start ()
{
	if (!gameControl.hasLoaded)
	{
		gameControl.Reset();
	}

	currentHealth = gameControl.health;
	bloodObject = Resources.Load("Blood");
	startingScale = transform.localScale.y;
	crouchScale = startingScale / 2;
	gameControl.crouchScale = crouchScale;
	lastY = transform.position.y;
}

function Update () {

	if (currentHealth != gameControl.health)
	{
		GameObject.Instantiate(bloodObject,transform.position,transform.rotation);
		currentHealth = gameControl.health;
	}

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
		if (ducking)
		{
			rigidbody.AddForce(0,superJumpForce,0);
		}
		else
		{
			rigidbody.AddForce(0,jumpForce,0);
		}
		currentJumpTime += Time.deltaTime;
		
	}
	
	if (!Input.GetKey(KeyCode.Space) && currentJumpTime > 0)
	{
		currentJumpTime = jumpTime;
	}
	
	if (Physics.Raycast(transform.position,-transform.up,transform.localScale.y*1.1))
	{
		currentJumpTime = 0;
	}
	
	//Ducking
	if (Input.GetKey(KeyCode.S))
	{
		dashing = false;
		ducking = true;
		if (transform.localScale.y == startingScale)
		{
			transform.localScale.y = crouchScale;
			transform.position.y -= crouchScale;
		}
	}
	else if (!Physics.Raycast(transform.position + Vector3(crouchScale,0,0),transform.up,transform.localScale.y * 2) &&
			!Physics.Raycast(transform.position - Vector3(crouchScale,0,0),transform.up,transform.localScale.y * 2))
	{
		ducking = false;
		if (transform.localScale.y == crouchScale)
		{
			transform.localScale.y = startingScale;
			transform.position.y += crouchScale;
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
	
	calcEating();
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
}

function calcEating()
{
	    //Eating
        if (Input.GetKey(KeyCode.E))
        {
        	if (Inventory.food > 0 && gameControl.canEat)
        	{
        		Inventory.food -= 1;
        		gameControl.health = 100;
        		gameControl.canEat = false;
        	}
        }
        
        if (!gameControl.canEat && currentEatingCoolDown < eatingCoolDown)
        {
        	currentEatingCoolDown += Time.deltaTime; 
        }
        else
        {
        	currentEatingCoolDown = 0;
        	gameControl.canEat = true;
        }
}