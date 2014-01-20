
function Update() 
{
}

function OnTriggerEnter(theCollision : Collider)
{
    if(theCollision.gameObject.tag == "Player")
    {
        gameControl.health -= 10000;
    }
    else if (theCollision.gameObject.tag != "Player")
    {

        //Destroy(theCollision.gameObject);
    }
}