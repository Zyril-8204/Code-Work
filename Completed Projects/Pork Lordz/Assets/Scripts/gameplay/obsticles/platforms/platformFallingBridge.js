public var steppedOn = false;
public var timer = 5f;
public var moveVector : Vector3;
private var speed = 3;
private var killTimer = 3f;

function Update () 
{
    if (steppedOn) 
    {
        timer -= Time.deltaTime;
    }

    if (timer <= 0) 
    {
        steppedOn = false;
        moveVector = transform.TransformDirection(moveVector);
        moveVector *= speed;
        moveVector *= Time.deltaTime;
        this.transform.Translate(moveVector);
        moveVector = -Vector3.up;
        killTimer -= Time.deltaTime;
    }

    if (killTimer <= 0)
    {
        Destroy(this.gameObject);
    }
}

function MakeTrue()
{
    steppedOn = true;
    moveVector = -Vector3.up;
}