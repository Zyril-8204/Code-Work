//Stats
static var kills: int = 0;
static var score: int = 0;
static var items: int = 0;
static var playerName: String = "Test";

//Highscore Information
private var nameArray = ["A","B","C","D","E","F","G","H","I","J"];
private var scoreArray = [0,0,0,0,0,0,0,0,0,0];
private var killsArray = [0,0,0,0,0,0,0,0,0,0];
private var itemsArray = [0,0,0,0,0,0,0,0,0,0];
public var hSpace = 128;
public var vSpace = 32;
private var maxSlots = 10;
private var scoresLoaded : boolean = false;

//Database variables
private var url :String = "http://zanzindorf.kodingen.com/porklordz.php?type=addscore&table=1&name=";
private var timeLimit: float = 5.0f;
private var timer: float =	0.0f;
private var www: WWW;

//GameOver variables
public var menuSceneName :String = "SceneMenu";

//UI variables
public var showHighScore: boolean = false;
public var showNameInput: boolean = true;
public var highScorePosition: Vector2 = Vector2(32,32);
public var nameInputPosition: Vector2 = Vector2(32,32);
public var highScoreBG : Texture2D;
public var gameOverBG : Texture2D;
private var style;

function Start()
{
	style = Resources.Load("PorkButtons");
}

function OnGUI()
{
	GUI.skin = style;
	
	
	
	if (showHighScore)
	{
		GUI.DrawTexture(Rect(0,0,Screen.width, Screen.height), highScoreBG);
		DrawStats(highScorePosition);
	}
	if (showNameInput)
	{
		GUI.DrawTexture(Rect(0,0,Screen.width, Screen.height), gameOverBG);
		DrawEnterScore(nameInputPosition);
	}
	
	
	if (GUI.Button(Rect(Screen.width - 128,0,128,64),"Return"))
	{
		Application.LoadLevel(menuSceneName);
	}
}

//Sends the stats to be recorded in the database
public function SendStats()
{
	url = url + playerName.ToString() + "&score=" + score.ToString() + 
		"&kills=" + kills.ToString() + "&items=" + items.ToString();
		
	www = new WWW(url);
	StartCoroutine(WaitForSend(www,timeLimit));
}

//Loads highscore information into public static arrays
public function LoadStats()
{
	url = "http://zanzindorf.kodingen.com/porklordz.php?type=getscore";
	www = new WWW(url);
	StartCoroutine(WaitForLoad(www,timeLimit));
}

public function DrawEnterScore(position: Vector2)
{
	//GUI.Label(Rect(0,0,256,32), "Score: " + score.ToString() + " Kills: " + kills.ToString() + "Items: " + items.ToString()); 
	var pos : Vector2 = Vector2(position.x * Screen.width, position.y * Screen.height);
	
	playerName = GUI.TextField(Rect(pos.x,pos.y,128,64),playerName,11);
	
	if (GUI.Button(Rect(pos.x,pos.y + 64,64,64),"Enter"))
	{
		playerName = playerName.Replace(' ','_');
		SendStats();
	}
	if (GUI.Button(Rect(pos.x + 64,pos.y + 64,64,64),"Cancel"))
	{
		Application.LoadLevel(menuSceneName);
	}
}

//Draws stats to the screen
public function DrawStats(position: Vector2)
{		
	if (!scoresLoaded)
	{
		scoresLoaded = true;
		LoadStats();
	}


	var pos: Rect = Rect(position.x * Screen.width, position.y * Screen.height,hSpace,vSpace);
	
	//Draw column lables
	GUI.Label(pos, "Rank");
	pos.x += hSpace;
	GUI.Label(pos, "Name");
	pos.x += hSpace;
	GUI.Label(pos, "Score");
	pos.x += hSpace;
	GUI.Label(pos, "Kills");
	pos.x += hSpace;
	GUI.Label(pos, "Items");
	pos.x = position.x;
		
	pos = Rect(position.x * Screen.width, position.y * Screen.height + vSpace,hSpace,vSpace);
	
	for (var i = 0; i < maxSlots; i++)
	{
		GUI.Label(pos, (i + 1).ToString());
		pos.x += hSpace;
		GUI.Label(pos, nameArray[i]);
		pos.x += hSpace;
		GUI.Label(pos, scoreArray[i].ToString());
		pos.x += hSpace;
		GUI.Label(pos, killsArray[i].ToString());
		pos.x += hSpace;
		GUI.Label(pos, itemsArray[i].ToString());
		pos.x = position.x * Screen.width;
		
		pos.y += vSpace;
		
	}
}

public function WaitForSend(www : WWW, waitTime : float)
{
    timer += Time.deltaTime;
	
	if (timer < timeLimit)
    	yield www;
    else;
    	yield WaitForSeconds(0);

    if (www.error == null)
    {
		Debug.Log(www.text);
    } 
    else 
    {
        Debug.Log("WWW Error: "+ www.error);
    }   
    
    Application.LoadLevel(menuSceneName); 	
}

public function WaitForLoad(www : WWW, waitTime : float)
    {
    	timer += Time.deltaTime;
    	
    	if (timer < timeLimit)
        	yield www;
        else;
        	yield WaitForSeconds(0);

        // check for errors
        if (www.error == null)
        {
			Debug.Log(www.text);
			var temp = www.text.Split(" "[0]);
			
			var index: int = 0;
			for (var i: int = 0; i < maxSlots * 4; i+=4)
			{
				nameArray[index] = temp[i];
				scoreArray[index] = int.Parse(temp[i+1]);
				killsArray[index] = int.Parse(temp[i+2]);
				itemsArray[index] = int.Parse(temp[i+3]);
				index ++;
			}
			
	
        } else {
            Debug.Log("WWW Error: "+ www.error);
        }    	
    }
    
//Test code for sending data
//http://zanzindorf.kodingen.com/porklordz.php?type=addscore&table=1&name=aaron&score=121&kills=5&items=7