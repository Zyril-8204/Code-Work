    public var jumping : boolean = false; // boolean jumping
    public var speed = 8; // int
    public var jumpSpeed = 1.5;// double
    public var timer = 0f; // float value
    public var leftKey: boolean = false; // boolean leftKey
    public var dashLeft : boolean = false; // boolean dashLeft
    public var rightKey: boolean = false; //boolean rightKey
    public var dashRight : boolean = false; // boolean dashRight
    public var dashed : boolean = false; // boolean dashed
    public var dashTimer = 0f; // float value
    public var repeatThreshold = 5f; //Threshold for button double taps

    
    function Start(){ // funtion to void
    jumping = false;
    }
     
    function FixedUpdate () // funtion to void
    {
    	if (dashed == true)
    	{
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
    	if(Input.GetKey(KeyCode.D))
        {
            transform.position.x += Time.deltaTime * speed;
            leftKey = false;
            rightKey = true;
        }
        if(Input.GetKey(KeyCode.A))
        {
            transform.position.x -= Time.deltaTime * speed;
            leftKey = true;
            rightKey = false;
        }
        if(Input.GetKeyDown(KeyCode.Space) && jumping == false)
        {
            jumping = true;
            rigidbody.velocity.y = -Physics.gravity.y * jumpSpeed;
        }
        if (leftKey == true && dashed == false)
        {
        	if (timer <= repeatThreshold)
        	{
        		if (Input.GetKeyDown(KeyCode.A))
        		{
        			dashLeft = true;
        			timer = 0; 
        			leftKey = false;
        		}
        		timer += 0.2f;
        	}
        	else
        	{
        		leftKey = false;
        		timer = 0;
        	}
        }
        
        if (rightKey == true && dashed == false)
        {
        	if (timer <= 10f)
        	{
        		if (Input.GetKeyDown(KeyCode.D))
        		{
        			dashRight = true;
        			timer = 0; 
        			rightKey = false;
        		}
        		timer += 0.2f;
        	}
        	else
        	{
        		rightKey = false;
        		timer = 0;
        	}
        }
        
        if (dashLeft == true)
        {
       		DashLeft();
        }
        if (dashRight == true)
        {
       		DashRight();
        }
    }
    
    function DashLeft() // funtion to void
    {
    	transform.position.x -= Time.deltaTime * (speed*150);
    	dashLeft = false;
    	dashed = true;
    }
    
    function DashRight() // funtion to void
    {
    	transform.position.x += Time.deltaTime * (speed*150);
    	dashRight = false;
    	dashed = true;
    }
    // void OnCollisionEnter(Collision collision)
    function OnCollisionEnter(collision : Collision)
    {
                   jumping = false;
    }
    