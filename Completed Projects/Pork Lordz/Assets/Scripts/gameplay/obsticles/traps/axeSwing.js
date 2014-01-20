public var axe:GameObject;
var swung = false;
var swing = false;
var inc:int = 0;
public var speed = 60;
function FixedUpdate () 
{
    if(swing)
    {
        axe.transform.Rotate(-Vector3.up * speed * Time.deltaTime);
        inc++;
        if(inc >= (9600 / speed))
        {
            swing = !swing;
        }
    }
}

function OnTriggerEnter ()
{
    if (!swung)
    {
        swing = true;
        swung = true;
    }
    
}