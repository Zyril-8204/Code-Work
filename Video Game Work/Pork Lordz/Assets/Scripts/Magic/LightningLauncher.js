var projectile : Rigidbody;
var shotDelay = .5;
var speed;
static var damage = 5;

function Update()
{
	if(Inventory.currentMagic.objName == "Zeus Tome")
	{
		if(Application.loadedLevelName == "2DWorld")
			 {
			 	speed = 15;
				 if( Input.GetButtonDown("Fire2") && Input.GetKey(KeyCode.D))
				  {
				  audio.Play();
				  	var instantiatedProjectileLightRight : Rigidbody = Instantiate( 
					   projectile, transform.position, transform.rotation );
					instantiatedProjectileLightRight.velocity =
					   transform.TransformDirection( Vector3( 0, 0, speed ) );
					Physics.IgnoreCollision( instantiatedProjectileLightRight. collider,
					   transform.root.collider ); 
				  }
				 else if(Input.GetButtonDown("Fire2") && Input.GetKey(KeyCode.A))
				 {
				 audio.Play();
				 	var instantiatedProjectileLightLeft : Rigidbody = Instantiate( 
					   projectile, transform.position, transform.rotation );
					instantiatedProjectileLightLeft.velocity =
					   transform.TransformDirection( Vector3( 0, 0, -speed ) );
					Physics.IgnoreCollision( instantiatedProjectileLightLeft. collider,
					   transform.root.collider );
				 }
				 else if(Input.GetButtonDown("Fire2") && Input.GetKey(KeyCode.W))
				 {
				 audio.Play();
				 	var instantiatedProjectileLightUp : Rigidbody = Instantiate( 
					   projectile, transform.position, transform.rotation );
					instantiatedProjectileLightUp.velocity =
					   transform.TransformDirection( Vector3( 0, speed, 0 ) );
					Physics.IgnoreCollision( instantiatedProjectileLightUp. collider,
					   transform.root.collider );
				 }
			 }
			 else if(Application.loadedLevelName =="3DDungeon"|| Application.loadedLevelName =="BossRoom")
			 {
			 speed = 35;
			 	if(Input.GetButtonDown("Fire2"))
			 	{
			 		audio.Play();
				 	var instantiatedProjectileLightForward : Rigidbody = Instantiate( 
					   projectile, transform.position, transform.rotation );
					instantiatedProjectileLightForward.velocity =
					   transform.TransformDirection( Vector3( 0, 0, speed ) );
					Physics.IgnoreCollision( instantiatedProjectileLightForward. collider,
					   transform.root.collider );
			 	}
			 }
	}
	
}
