public var speed = 1.5f;
public var moveY = false;
public var moveYneg = false;
public var moveX = false;
public var moveXneg = false;
public var moveZ = false;
public var moveZneg = false;
public var timer = 0f;
public var timeReset = 20f;
public var moveVector = new Vector3(0,0,0);

function FixedUpdate () {

    moveVector = Start();
    moveVector = transform.TransformDirection(moveVector);
    moveVector *= speed;
    moveVector *= Time.deltaTime;
    this.transform.Translate(moveVector);
    timer -= Time.deltaTime;
    if(timer <= 0) reverse();
}

function Start() : Vector3
{
    var tV : Vector3 = Vector3.zero;
    if (moveZ) 
    {
        tV += Vector3.forward;
    }
    if (moveZneg) 
    {
        tV += -Vector3.forward;
    }
    if (moveY) 
    {
        tV += Vector3.up;
    }
    if (moveYneg) 
    {
        tV += -Vector3.up;
    }
    if (moveX) 
    {
        tV += Vector3.right;
    }
    if (moveXneg) 
    {
        tV += -Vector3.right;
    }
    return tV;
}

function reverse()
{
    moveY = !moveY;
    moveYneg = !moveYneg;
    moveX = !moveX;
    moveXneg = !moveXneg;
    moveZ = !moveZ;
    moveZneg = !moveZneg;
    timer = timeReset;
}