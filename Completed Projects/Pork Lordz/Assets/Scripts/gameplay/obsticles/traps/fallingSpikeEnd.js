
function Update() 
{
    if (this.transform.position.y <= -150)
    {
        Destroy(this.gameObject);
    }
}