public var target : GameObject;
public var size = 10f;
public var scrollSpeed = 30f;

var pos : Vector3;
private var cam : Camera;

function Start()
{
    cam = Camera.main.gameObject.GetComponent("Camera");
    cam.isOrthoGraphic = true;
    cam.transform.rotation = Quaternion.Euler(30, 45, 0);

    pos = target.transform.position;
    cam.orthographicSize =25;
    Screen.sleepTimeout = 0.0f;
}

function LateUpdate()
{
    var distance = 30f;

    //transform.position = target.transform.position + new Vector3(-distance, distance, -distance);

    transform.position = Vector3.Lerp(transform.position, target.transform.position + new Vector3(-distance, distance, -distance), 0.5f * Time.deltaTime);
    cam.transform.LookAt(target.transform);
}

function OnGUI()
{
//    GUI.Label(new Rect(10, 100, 200, 50), "" + target.transform.position.x + ", " + target.transform.position.y + ", " + target.transform.position.z);
//    GUI.Label(new Rect(10, 130, 200, 50), "" + cam.transform.position.x + ", " + cam.transform.position.y + ", " + cam.transform.position.z);
}
