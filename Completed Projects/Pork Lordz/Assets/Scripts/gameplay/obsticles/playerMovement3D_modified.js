public var speed = 10.0f;
public var jumpSpeed = 8.0f;
public var gravity = 20.0f;
public var dash: boolean = false;
public var dashKey: boolean = false; // key to see if we dash
public var dashed : boolean = false; // boolean dashed
public var dashTimer = 0f; // float value
public var repeatThreshold = 5f; //Threshold for button double taps
private var moveDirection : Vector3 = Vector3.zero;
private var dashSpeed = 30.0f;
private var walkSpeed = 10.0f;
private var crouchSpeed = 1.0f;
public var currentCrouchingSpeed = 0.0f;
private var crouchDelta = 5.0f;

public var startFall = 0.0f;
public var lastY = 0.0f;
private var fallingThreshold = 30.0f;
public var falling : boolean = false;
private var fallDamage = 5;

private var equalDelta = 0.1f;

private var hit : RaycastHit; //struct used for determining objects hit by raycast (used for passthrough platforms and such)

function Start ()
{
	lastY = transform.position.y;
}

function Update() {

	var controller: CharacterController = GetComponent(CharacterController);
	
	if (controller.isGrounded) 
	{
		moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		moveDirection = transform.TransformDirection(moveDirection);
		moveDirection *= speed;
		if (Input.GetButton("Jump"))
		{
			moveDirection.y = jumpSpeed;
		}
	}
	moveDirection.y -= gravity * Time.deltaTime;
	controller.Move(moveDirection * Time.deltaTime);
	
    //check for falling bridge, uses same tag as the fallthrough since it triggers a different script
	if (Physics.Raycast(transform.position, Vector3(0, -1, 0), hit))
    {
        if (hit.collider.tag == "Fallthrough")
        {
            hit.collider.gameObject.GetComponent("platformFallingBridge").MakeTrue();
        }
    }
} 

public function movement(theSpeed:float)
{
	speed += theSpeed;
}

function FixedUpdate () // funtion to void
    {
    	if (dashed == true)
    	{ 
    		if (dashTimer >= 5f)
    		{
    			speed = walkSpeed;
    		}
    		
    		if (dashTimer <= 30f)
    		{
    		 	dashTimer += 0.2f;
    		}
    		else
    		{
    			dashed = false;
    			dashTimer = 0;	
    		}
    	}
    	if (Input.GetKey(KeyCode.R) && dashed == false)
    	{
    		dashKey = true;
    	}
        
        if (dashKey == true && dashed == false)
        {
            dash = true;
           	dashKey = false;
        }
        
        if (dash == true)
        {
        	Dash();
        }
        
        
		calcCrouch();
		
		calcFalling();
    }
    
function calcFalling()
{
	
	//Check if falling 
	if (lastY >= (transform.position.y + equalDelta))
	{
		if (!falling)
		{
			falling = true;
			startFall = lastY;
			Debug.Log("Meep");
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
    
    
    function Dash() // funtion to void
    {
    	speed = dashSpeed;
    	dash = false;
    	dashed = true;
    }
    
    function calcCrouch()
    {
        //Crouching
        if (Input.GetKey(KeyCode.LeftShift))
        {

        	if (transform.localScale.y == 1)
			{
        		transform.localScale.y = 0.5f;
        		transform.position.y -= 0.5f;
        		currentCrouchingSpeed = speed;
        	}

            //fallthrough platforms
            if (Physics.Raycast(transform.position, Vector3(0, -1, 0), hit))
            {
                if (hit.collider.tag == "Fallthrough")
                {
                    hit.collider.gameObject.GetComponent("platformFallthrough").makeTrue();
                }
            }
        }
        else if (transform.localScale.y == 0.5f)
        {
        	if (!Physics.Raycast(transform.position + Vector3(0.5f,0,0),transform.up,1.0f) &&
        		!Physics.Raycast(transform.position + Vector3(-0.5f,0,0),transform.up,1.0f) &&
        		!Physics.Raycast(transform.position + Vector3(0,0,0.5f),transform.up,1.0f) &&
        		!Physics.Raycast(transform.position + Vector3(0,0,-0.5f),transform.up,1.0f))
			{
				speed = walkSpeed;
        		transform.localScale.y = 1;
        		transform.position.y += 0.5f;
        	}
        }
        
        if (transform.localScale.y == 0.5f)
        {
            if (currentCrouchingSpeed > crouchSpeed) currentCrouchingSpeed -= crouchDelta * Time.deltaTime;
        	speed = currentCrouchingSpeed;
        }

        
    }
    
function OnGUI()
{
	gameControl.drawHealth();
}
        
        