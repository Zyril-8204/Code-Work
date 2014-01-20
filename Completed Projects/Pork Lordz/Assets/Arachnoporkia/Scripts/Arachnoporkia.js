public var safeSpot: Transform;
public var mouth: Transform;
public var target: Transform;
public var health: float = 100;
public var maxHealth: float = 100;
public var upSpeed: float = 10.0f;
public var shootTimer: float = 0;
public var shootPause: float = 2;
public var shootPauseOnGround: float = 10;
public var speed: float = 5;
public var speedBoost: float = 10;
public var explosion: GameObject;
public var centerObject : Transform;

private var pixel :Texture;
private var retreatAmount:float = .5f;
private var canSpit: boolean = true;
private var spits: int = 0;
private var maxSpits: int = 10;
private var maxRot = 1;
private var rot = 0;
private var startPos: Vector3;

function Start ()
{
	pixel = Resources.Load("HealthBar");
	startPos = transform.position;
}

function Update () 
{
	if (Input.GetKeyDown(KeyCode.O))
	{
		health -= 10;
	}

	if (health <= 0)
	{
		Stats.score += 9001;
		Application.LoadLevel("GameWonScene");
		Destroy(gameObject);
	}
	
	if (health < maxHealth	* retreatAmount && canSpit)
	{
		//transform.Rotate(transform.up, 5);
		transform.LookAt(target.position);
	
		var temp = safeSpot.position.y - transform.position.y;
		if (temp > upSpeed) temp = upSpeed;
		transform.position.y = transform.position.y + temp * Time.deltaTime;
		
		shootTimer += Time.deltaTime;
		if (shootTimer > shootPause)
		{
			shootTimer = 0;
			spits++;
			GameObject.Instantiate(Resources.Load("SpiderBall"), mouth.position, transform.rotation); 
		}
		
		if (spits > maxSpits)
		{
			canSpit = false;
		}
	}
	else if (transform.position.y > startPos.y)
	{
		transform.rotation.x = 0;
		transform.rotation.z = 0;
		var temp2 = startPos.y - transform.position.y;
		if (temp2 > upSpeed) temp2 = upSpeed;
		transform.position.y = transform.position.y + temp2 * Time.deltaTime;
		
		if (transform.position.y - startPos.y < 0.5f) transform.position.y = startPos.y;
	}
	else //Ground movement
	{
		var currentSpeed: float = speed + speedBoost * (1 - health / maxHealth);
		
//		if (Physics.Raycast(transform.position, transform.forward,2))
//		{
//			transform.RotateAround(transform.up, 180);	
//		}
		
		shootTimer += Time.deltaTime;
		if (shootTimer > shootPauseOnGround)
		{
			shootTimer = 0;
			GameObject.Instantiate(Resources.Load("SpiderBall"), mouth.position, transform.rotation); 
		}
		
		rot += Random.Range(-1,2);
		rot = Mathf.Clamp(rot,-maxRot,maxRot);
		transform.Rotate(transform.up, rot);	
		transform.position = transform.position + transform.forward * Time.deltaTime * currentSpeed;
		
	}
}

function OnTriggerStay (other : Collider) {
	if (other.gameObject.tag == "Weapon")
	{
		if (!(other.GetComponent(Swinging)).inactive)
		{
			health -= Inventory.currentWeapon.power;
			(other.GetComponent(Swinging)).inactive = true;	
		}
		
	}
	
	if (other.gameObject.tag == "Player")
	{
		gameControl.health -= 1 * Inventory.currentShield.defense;		
	}
		
}

function OnCollisionStay(other : Collision)
{
	transform.LookAt(centerObject);
	//transform.rotation = Quaternion.Euler(0,Vector3.Angle(transform.position,centerObject.position),0);
}

function OnGUI()
{
	var temp:float = Screen.width * (health / maxHealth);
	GUI.color = Color.red;
	GUI.DrawTexture(Rect(0,0,temp, 10),pixel);
}