var projectile : Rigidbody;
var speed = 20;
function Update()
{
	 if(Application.loadedLevelName == "2DWorld")
	 {
	 if( Input.GetButtonDown("Fire2") && Input.GetKey(KeyCode.D))
	  {
		  var instantiatedProjectileRight : Rigidbody = Instantiate( 
		   projectile, transform.position, transform.rotation );
		  instantiatedProjectileRight.velocity =
		   transform.TransformDirection( Vector3( 0, 0, speed ) );
		  Physics.IgnoreCollision( instantiatedProjectileRight. collider,
		   transform.root.collider );
	  }
	 if(Input.GetButtonDown("Fire2") && Input.GetKey(KeyCode.A))
	 {
	 	var instantiatedProjectileLeft : Rigidbody = Instantiate( 
		   projectile, transform.position, transform.rotation );
		  instantiatedProjectileLeft.velocity =
		   transform.TransformDirection( Vector3( 0, 0, -speed ) );
		  Physics.IgnoreCollision( instantiatedProjectileLeft. collider,
		   transform.root.collider );
	 }
	 if(Input.GetButtonDown("Fire2") && Input.GetKey(KeyCode.W))
	 {
	 	var instantiatedProjectileUp : Rigidbody = Instantiate( 
		   projectile, transform.position, transform.rotation );
		  instantiatedProjectileUp.velocity =
		   transform.TransformDirection( Vector3( 0, speed, 0 ) );
		  Physics.IgnoreCollision( instantiatedProjectileUp. collider,
		   transform.root.collider );
	 }
	 }
	 else if(Application.loadedLevelName =="3DDungeon" || Application.loadedLevelName =="BossRoom")
	 {
	 	if(Input.GetButtonDown("Fire2"))
	 	{
		 	var instantiatedProjectileForward : Rigidbody = Instantiate( 
			   projectile, transform.position, transform.rotation );
			  instantiatedProjectileForward.velocity =
			   transform.TransformDirection( Vector3( 0, 0, speed ) );
			  Physics.IgnoreCollision( instantiatedProjectileForward. collider,
			   transform.root.collider );
	 	}
	 }
}