public var fallThrough = false;
public var obstacleType = 0; //fall through platform
private var timer = 0;

function Update () 
{
    //update collider based on boolean
    if (fallThrough && collider.enabled == true)
    {
        //Debug.Log("fallThrough is true and collider is on, turning collider off.");
        this.collider.enabled = false;
    } else if (!fallThrough && collider.enabled == false)
    {
        //Debug.Log("fallThrough is false and collider is off, turning collider on.");
        this.collider.enabled = true;
    }

    if (timer > 0)
    {
        timer -= Time.deltaTime;
        if (timer <= 0) makeFalse();
    }
}

function makeFalse()
{
    fallThrough = false;
    //Debug.Log("makeFalse");
}

function makeTrue()
{
    fallThrough = true;
    timer += 15;
    //Debug.Log("makeTrue");
}