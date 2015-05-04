/*
 * This script should be attached to a camera object and will implement the main menu.
 * Will display buttons for starting the game, viewing stats, and quitting.
 */


using UnityEngine;
using System.Collections;

public class ScriptMainMenu : MonoBehaviour {
	
	public string startSceneName;
	public string statSceneName;
	
	public Texture playButton;
	public Texture statsButton;
	public Texture quitButton;
	public Texture cursor;
	public Texture transition;
	public Texture title;
	
	public float rotationSpeed = .05f;			//Speed of rotation for the camera object
	public float buttonWidth =  .25f;			//Width of button (percent of screen width)
	public float buttonHeight = .10f;			//Height of button (percent of screen height)
	public float buttonPadding = .05f;			//Space in between buttons (percent of screen width)
	
	private Rect buttonRect;					//Used to draw buttons
	private Rect cursorRect;					//Used to draw the cursor
	private Rect transitionRect;				//Used to draw the transition
	private Vector2 buttonRowPosition;			//Position that the row of buttons starts
	private int selectionMade = 0;				//Flags when an option has been made. (0 = none, 1 = start, 2 = stats, 3 = quit)
	private float maxStabSpeed = -10;			//Max speed that the cursor moves at when a selection has been made.
	public float transitionAlpha = -2;
	
	
	// Use this for initialization
	void Start () {
		
		gameControl.Reset();
		
		buttonRowPosition = new Vector2(0,Screen.height / 4 + 140);
		transitionRect = new Rect(0,0,Screen.width,Screen.height);
		
		//Convert percentages to real values
		buttonWidth *= Screen.width;
		buttonHeight *= Screen.height;
		buttonPadding *= Screen.width;
		
		//Define starting position of first button - adds an initial padding space.
		buttonRect = new Rect(buttonRowPosition.x + buttonPadding,buttonRowPosition.y,buttonWidth,buttonHeight);
		//Places cursor at bottom of screen 
		cursorRect = new Rect(0,Screen.height - cursor.height / 2 ,cursor.width,cursor.height); 
	}
	
	// Update is called once per frame
	void Update () {
		
		//Rotates Camera
		transform.RotateAround(transform.up, rotationSpeed * Time.deltaTime);
	
	}
	
	 void OnGUI() {
		
		//Calculate position of the cursor
		if (selectionMade != 0)
		{
			float yVel = buttonRowPosition.y - cursorRect.y;
			
			if (yVel < maxStabSpeed)
			{
				yVel = maxStabSpeed;
			}
			else
			{
				transitionAlpha += 1 * Time.deltaTime;
			}
			
			cursorRect.y += yVel; 
		}
		else 
		{
			cursorRect.x = Input.mousePosition.x - cursor.width / 2;
		}
		
		
		//Draw cursor
		//GUI.Label(cursorRect,cursor,"");
		GUI.DrawTexture(cursorRect,cursor);
		
		//Draw Title
		GUI.DrawTexture(new Rect(Screen.width / 2 - title.width / 2,0,title.width,title.height),title);
		
		
		//Reset position
		buttonRect.x = buttonRowPosition.x + buttonPadding;
		
		//Start button
		if (GUI.Button(buttonRect, playButton, ""))
		{
			selectionMade = 1;
		}
		
		buttonRect.x += buttonPadding + buttonWidth;	//Advance to next position 
		
		//Stats button
		if (GUI.Button(buttonRect, statsButton, ""))
		{
			selectionMade = 2;
		}
		
		buttonRect.x += buttonPadding + buttonWidth;
		
		//Quit button
		if (GUI.Button(buttonRect, quitButton, ""))
		{
			selectionMade = 3;
		}
		
		//Draw transition
		GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b, transitionAlpha);
		GUI.DrawTexture(transitionRect,transition);
		//GUI.Label(transitionRect, transition, "");
		GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b, 1);
		
		//Execute action if transition is complete
		if (transitionAlpha	>= 1)
		{
				switch (selectionMade)
				{
				case 1: Application.LoadLevel(startSceneName); break;
				case 2: Application.LoadLevel(statSceneName); break;
				case 3: Application.Quit(); break;
				}	
		}
		
	}
}
