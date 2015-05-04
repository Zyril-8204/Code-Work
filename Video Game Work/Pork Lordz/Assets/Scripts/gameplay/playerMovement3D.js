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
private var pixelTexture: Texture2D;
private var redAlpha:float = 0.5;

private var collisionBox: Transform;

public var startFall = 0.0f;
public var lastY = 0.0f;
private var fallingThreshold = 30.0f;
public var falling : boolean = false;
private var fallDamage = 5;
private var equalDelta = 0.1f;
static var crouchScale: float;
private var currentHealth: float;		//For detecting when to spawn blood;

private var eatingCoolDown: float = 30.0f;
private var currentEatingCoolDown: float = 0.0f;

private var hit : RaycastHit; //struct used for determining objects hit by raycast (used for passthrough platforms and such)


function Start ()
{
	if (!gameControl.hasLoaded)
	{
		gameControl.Reset();
	}
	
	pixelTexture = new Texture2D(1,1,TextureFormat.ARGB32, false);
	gameControl.crouchScale = transform.localScale.y / 2;
	lastY = transform.position.y;
	collisionBox = gameObject.GetComponent(CharacterController).collider.transform;
}

function Update() {

	if (redAlpha > 0)
	{
		redAlpha -= 1.0f * Time.deltaTime;
	}
	
	if (currentHealth != gameControl.health)
	{
		currentHealth = gameControl.health;
		redAlpha = 1;
	}

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
        

		calcEating();        
		calcCrouch();
		calcFalling();
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

function calcFalling()
{
	
	//Check if falling 
	if (lastY >= (transform.position.y + equalDelta))
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

        	if (collisionBox.localScale.y == 1)
			{
        		collisionBox.localScale.y = 0.5f;
        		collisionBox.position.y -= 0.5f;
        		currentCrouchingSpeed = speed;
        	}
        	
        	       //fallthrough platforms
            if (Physics.Raycast(collisionBox.position, Vector3(0, -1, 0), hit))
            {
                if (hit.collider.tag == "Fallthrough")
                {
                    hit.collider.gameObject.GetComponent("platformFallthrough").makeTrue();
                }
            }
        }
        else if (transform.localScale.y == 0.5f)
        {
        	if (!Physics.Raycast(collisionBox.position + Vector3(0.5f,0,0),collisionBox.up,1.0f) &&
        		!Physics.Raycast(collisionBox.position + Vector3(-0.5f,0,0),collisionBox.up,1.0f) &&
        		!Physics.Raycast(collisionBox.position + Vector3(0,0,0.5f),collisionBox.up,1.0f) &&
        		!Physics.Raycast(collisionBox.position + Vector3(0,0,-0.5f),collisionBox.up,1.0f))
			{
				speed = walkSpeed;
        		collisionBox.localScale.y = 1;
        		collisionBox.position.y += 0.5f;
        	}
        }
        
        if (collisionBox.localScale.y == 0.5f)
        {
            if (currentCrouchingSpeed > crouchSpeed) currentCrouchingSpeed -= crouchDelta * Time.deltaTime;
        	speed = currentCrouchingSpeed;
        }
    }
    
function OnGUI()
{
	gameControl.drawHealth();
	GUI.color = Color.red;
	GUI.color.a = redAlpha;
	GUI.DrawTexture(Rect(0,0,Screen.width,Screen.height),pixelTexture);
}
        
        