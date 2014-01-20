var projectile : Rigidbody;
var shotDelay = .5;
var speed = 10;
static var damage = 5;

function Update()
{
	if(Inventory.currentMagic.objName == "Blizzard Tome")
	{
	if(Application.loadedLevelName == "2DWorld")
			 {
			 	speed = 15;
				 if( Input.GetButtonDown("Fire2") && Input.GetKey(KeyCode.D))
				  {
				  audio.Play();
				  	var instantiatedProjectileFireRight : Rigidbody = Instantiate( 
					   projectile, transform.position, transform.rotation );
					instantiatedProjectileFireRight.velocity =
					   transform.TransformDirection( Vector3( 0, 0, speed ) );
					Physics.IgnoreCollision( instantiatedProjectileFireRight. collider,
					   transform.root.collider ); 
				  }
				 else if(Input.GetButtonDown("Fire2") && Input.GetKey(KeyCode.A))
				 {
				 audio.Play();
				 	var instantiatedProjectileFireLeft : Rigidbody = Instantiate( 
					   projectile, transform.position, transform.rotation );
					instantiatedProjectileFireLeft.velocity =
					   transform.TransformDirection( Vector3( 0, 0, -speed ) );
					Physics.IgnoreCollision( instantiatedProjectileFireLeft. collider,
					   transform.root.collider );
				 }
				 else if(Input.GetButtonDown("Fire2") && Input.GetKey(KeyCode.W))
				 {
				 audio.Play();
				 	var instantiatedProjectileFireUp : Rigidbody = Instantiate( 
					   projectile, transform.position, transform.rotation );
					instantiatedProjectileFireUp.velocity =
					   transform.TransformDirection( Vector3( 0, speed, 0 ) );
					Physics.IgnoreCollision( instantiatedProjectileFireUp. collider,
					   transform.root.collider );
				 }
			 }
			 else if(Application.loadedLevelName =="3DDungeon"|| Application.loadedLevelName =="BossRoom")
			 {
			 	speed = 20;
			 	if(Input.GetButtonDown("Fire2"))
			 	{
			 	audio.Play();
				 	var instantiatedProjectileFireForward : Rigidbody = Instantiate( 
					   projectile, transform.position, transform.rotation );
					instantiatedProjectileFireForward.velocity =
					   transform.TransformDirection( Vector3( 0, 0, speed*2 ) );
					Physics.IgnoreCollision( instantiatedProjectileFireForward. collider,
					   transform.root.collider );
			 	}
			 }
			 }
}
			 