// Created by micah hawman

// editor settable values
public var Target: GameObject; 		// what object to we chase after
public var MoveSpeed = 0f; 			// how fast does the enemy move
public var RotationSpeed = 0f; 		// how fast does the enemy turn
public var attackDamage = 0f;		// our base damage
public var health = 0f;			// enemy health
public var attackDelayTime = 0f; 	// how long in between attacks
public var fieldOfViewRange = 0f;	// the range the enemy can see
public var SightRange = 0f;			// the distance the enemy can see
public var attackRange = 0f;		// distance enemy can attack
public var proximityDistance = 0f;	// the distance to set off proximity chase
public var LosChaseDistance = 0f;	// the distance to set off LoS chase
public var bonusScore : int = 50;


// our logic chase settables
public var alwaysChase: boolean = true;			// we always chase player
public var lineOfSightChase: boolean = false;	// we chase player if in line of sight
public var proximityChase: boolean = false;		// we chase player if in a certain distance
public var Patrol:boolean = false;

public var searchTag = "Patrol_Node"; // the tag to search for (set this value in the inspector)
public var scanFrequency = 1; // the frequency with which to re-scan for new nearest target in seconds 

// our attack type settables
public var closeRangeAttacker : boolean = false;	// we fight in front of player (hack and slash)
public var farRangeAttacker : boolean = false;		// we shoot things at player (guns/magic)

public var sawPlayer : boolean = false;		// goes true when we can see player
public var inAttackRange :boolean = false;	// goes true when we are in attack range
public var Node : GameObject;  // the nodes we'll see out


private var playerDistance: float = 0f;	// our distance value
private var NodeDistance: float = 0f;	// our distance value
private var layerMask = 8;					// layer player sits on for collision check
private var attackTimer = 0f;				// the timer to count out the attack delay
private var _myTransform: Transform;		// moves the enemy around

public var death:GameObject; // death object

function Awake()
{
    _myTransform = transform; // sets it to object's transform, good when we get to instantiation
}

function Start()
{
    Target = GameObject.FindWithTag("Player"); 	// find plaer

    //layerMask = ~layerMask;						// set layermask
    // set up repeating scan for new Targets:
    if (Patrol == true)
    {
    	InvokeRepeating("ScanForTarget", 0, scanFrequency );
    }
}

function Update()
{
	//Hulk smash fix 2Deeeee
	if (Application.loadedLevelName == "2DWorld")
	{
	 	transform.position.z = -5.25;
	 	//return true;
	}


	if (health <= 0)
	{
		Stats.score += bonusScore;
		Instantiate(death, this.gameObject.transform.position, Quaternion.identity);
		Stats.kills ++;
		Destroy(this.gameObject);
	}
	
	playerDistance = Vector3.Distance (transform.position, Target.transform.position); // find distance between player and object
 	//sawPlayer = CanSeePlayer(); // see if we can see player *BROKEN*
	
	if (Patrol == true)
	{
		// we rotate to look at the Target every frame (if there is one)
	    if (Node != null)
	    {
	    	NodeDistance = Vector3.Distance (transform.position, Node.transform.position);
	    	if (NodeDistance >= 2.5f)
	    	{
	        	transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Node.transform.position - transform.position), RotationSpeed * Time.deltaTime);
		    	transform.position += transform.forward*MoveSpeed*Time.deltaTime;
		    }
		    else
		    {
		    	Node.GetComponent("Node_Controller").visitingNode = true;
		    	//transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Node.transform.position - transform.position), RotationSpeed * Time.deltaTime);
		    	transform.position += transform.forward*MoveSpeed*Time.deltaTime;
		    }
	    }
	    if (Node == null)
	    {
	    	ResetRoute();
	    }
    } 	
 	
 	/*if (sawPlayer && Patrol == true)
 	{
 		Patrol = false;
 	}
 	
 	else if (sawPlayer == false && Patrol == false && proximityChase == false && lineOfSightChase == false && alwaysChase == false) 
 	{
 		Patrol = true;
 	}*/
 	
 	// move towards the player no matter what.
 	if (alwaysChase == true)
 	{
 		_myTransform.rotation = Quaternion.Slerp(_myTransform.rotation, Quaternion.LookRotation(Target.transform.position - _myTransform.position), RotationSpeed * Time.deltaTime);
	    _myTransform.position += _myTransform.forward*MoveSpeed*Time.deltaTime;
 	}
 	
    else if(lineOfSightChase == true)
    {	      
	    if (playerDistance <= LosChaseDistance && sawPlayer == true)
	    {
	    	_myTransform.rotation = Quaternion.Slerp(_myTransform.rotation, Quaternion.LookRotation(Target.transform.position - _myTransform.position), RotationSpeed * Time.deltaTime);
		    _myTransform.position += _myTransform.forward*MoveSpeed*Time.deltaTime;
	    }
	    else
	    {
	    	// uncomment if you want to spawn secondary enemies for player.
	    	//EnemySpawnAI.playerSighted = true;
	    }
	}
	else
	{
		// uncomment if you want to spawn secondary enemies for player.
		//EnemySpawnAI.playerSighted = false;
	}
	
	 	// move towards the player if in set range
 	if (proximityChase == true && playerDistance <= proximityDistance)
 	{
	    _myTransform.rotation = Quaternion.Slerp(_myTransform.rotation, Quaternion.LookRotation(Target.transform.position - _myTransform.position), RotationSpeed * Time.deltaTime);
	    
	    if (playerDistance >=0)
	    {
	    	_myTransform.rigidbody.velocity = transform.forward*MoveSpeed;
	    }
	    else
	    {
	    	_myTransform.rigidbody.velocity = transform.forward*MoveSpeed;
	    }
	    
    }
    else 
    {
    	_myTransform.rigidbody.velocity = Vector3(0,0,0);
    }
	
	if (playerDistance <= attackRange)
	{
		inAttackRange = true;
	}
	else
		inAttackRange = false;
	
	if (inAttackRange == true)
	{
		if (attackTimer <= attackDelayTime)
		{
			attackTimer ++;
		}
		else 
		{
			attackTimer = 0;
			AttackPlayer();
		}
	}
	
	
}

function CanSeePlayer() : boolean
{
    var hit : RaycastHit;
 	var rayDirection = Target.transform.position - transform.position; 
    
    // If the Target is close to this object and is in front of it, then return true
    if((Vector3.Angle(rayDirection, transform.forward)) < (fieldOfViewRange*2) && (Vector3.Distance(transform.position, Target.transform.position) <= attackRange))
    {
        inAttackRange = true;
    }
    
	else
	{
		inAttackRange = false;
	}
	
    if((Vector3.Angle(rayDirection, transform.forward)) <= fieldOfViewRange)
    { // Detect if player is within the field of view
        if (Physics.Raycast (transform.position, rayDirection, hit, SightRange)) 
        {
            if (hit.collider.gameObject.tag == "Player") 
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

function ScanForTarget() {
    // this should be called less often, because it could be an expensive
    // process if there are lots of objects to check against
    Node = GetNearestTaggedObject();

}

function GetNearestTaggedObject() : GameObject {
    // and finally the actual process for finding the nearest object:

    var nearestDistanceSqr = Mathf.Infinity;
    var taggedGameObjects = GameObject.FindGameObjectsWithTag(searchTag); 
    var nearestObj : GameObject = null;

    // loop through each tagged object, remembering nearest one found
    for (var obj : GameObject in taggedGameObjects) 
    {
	    if (obj.GetComponent("Node_Controller").visitingNode == true)
	    {
	    	obj.GetComponent("Node_Controller").visitedNode = true;
	    	continue;
	    }
	    if (obj.GetComponent("Node_Controller").visitedNode == true)
	    {
	    	continue;
	    }

        var objectPos = obj.transform.position;
        var distanceSqr = (objectPos - transform.position).sqrMagnitude;

        if (distanceSqr < nearestDistanceSqr) {
            nearestObj = obj;
            nearestDistanceSqr = distanceSqr;
        }
    }
    return nearestObj;
}

function ResetRoute()
{
	var taggedGameObjects = GameObject.FindGameObjectsWithTag(searchTag);
	for (var obj : GameObject in taggedGameObjects) 
    {
    	transform.position += transform.forward*(MoveSpeed / taggedGameObjects.Length)*Time.deltaTime;
    	obj.GetComponent("Node_Controller").resetRoute = true;
    }
}

function AttackPlayer()
{
	gameControl.health -= attackDamage * Inventory.currentShield.defense;
	if (this.gameObject.tag == "bomb")
	{
		health = 0;
	}
}

function ApplyDamage(damage:int)
{
	health -= damage;
}

function OnTriggerStay (other : Collider) {
	if (other.gameObject.tag == "Weapon")
	{
		if (!(other.GetComponent(Swinging)).inactive)
		{		
			(other.GetComponent(Swinging)).inactive = true;
			health -= Inventory.currentWeapon.power;
		}
	}
}
function OnCollisionEnter(other:Collision)
{
	if (other.gameObject.tag == "Fire")
	{
		health -= FireLauncher.damage;
		other.collider.isTrigger = true;
	}
	if (other.gameObject.tag == "Ice")
	{
		health -= IceLauncher.damage;
		other.collider.isTrigger = true;
	}
	if (other.gameObject.tag == "Lightning")
	{
		health -= LightningLauncher.damage;
		other.collider.isTrigger = true;
	}
}
