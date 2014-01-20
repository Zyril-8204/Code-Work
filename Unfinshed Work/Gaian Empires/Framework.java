package Game;
import java.awt.Graphics2D;
import java.awt.*;
import java.awt.Point;
import java.awt.event.KeyEvent;
import java.awt.event.MouseEvent;

public class Framework extends Canvas {

	// frame width and height
	public static int frameWidth;
	public static int frameHeight;

	//time variables
	public static final long secInNanosec = 1000000000L;
	public static final long milisecInNanosec = 1000000L;

	// FPS
	private final int GAME_FPS = 60;
	
	// pause between update cycles
	private final long GAME_UPDATE_PERIOD = secInNanosec / GAME_FPS;

	// states of game
	public static enum GameState{STARTING, VISUALIZING, GAME_CONTENT_LOADING, MAIN_MENU, OPTIONS, PLAYING, GAMEOVER, DESTROYED}
	
	// current game state
	public static GameState gameState;

	// game time
	private long gameTime;
	
	// help calc game time
	private long lastTime = 0;

	// The actual game
	private Game game = new Game();


	public Framework ()
	{
		super();

		gameState = GameState.VISUALIZING;

		//start game in new thread
		Thread gameThread = new Thread() {
			@Override
			public void run(){
				GameLoop();
			}
		};
		gameThread.start();
	}


	/**
	 * Set variables and objects.
	 */
	private void Initialize()
	{

	}

	/**
	 * Load files - images, sounds, ...
	 */
	private void LoadContent()
	{

	}


	/**
	 * In specific intervals of time (GAME_UPDATE_PERIOD) the game/logic is updated and then the game is drawn on the screen.
	 */
	private void GameLoop()
	{
		// wait some time so that we get correct frame/window resolution.
		long visualizingTime = 0, lastVisualizingTime = System.nanoTime();

		// calculate the time for how long we should put threat to sleep to meet FPS.
		long beginTime, timeTaken, timeLeft;

		while(true)
		{
			beginTime = System.nanoTime();

			switch (gameState)
			{
			case PLAYING:                	
				gameTime += System.nanoTime() - lastTime;                    

				game.UpdateGame(gameTime);
				
				lastTime = System.nanoTime();
				break;
			case GAMEOVER:
				//...
				break;
			case MAIN_MENU:
				//...
				break;
			case OPTIONS:
				//...
				break;
			case GAME_CONTENT_LOADING:
				//...
				break;
			case STARTING:
				// Sets variables and objects.
				Initialize();
				// Load files - images, sounds, ...
				LoadContent();

				// When all things that are called above finished, we change game status to playing or main menu
				gameState = GameState.PLAYING;
				break;
			case VISUALIZING:
				// this.getWidth() method doesn't return the correct value immediately 
				// So we wait one second for the window/frame to be set to its correct size. Just in case we
				// also insert 'this.getWidth() > 1' condition in case when the window/frame size wasn't set in time,
				// so that we get approximately size.
				if(this.getWidth() > 1 && visualizingTime > secInNanosec)
				{
					frameWidth = this.getWidth();
					frameHeight = this.getHeight();

					// When we get size of frame we change status.
					gameState = GameState.STARTING;
				}
				else
				{
					visualizingTime += System.nanoTime() - lastVisualizingTime;
					lastVisualizingTime = System.nanoTime();
				}
				break;
			case DESTROYED:
				break;
			default:
				break;
			}

			// Repaint the screen.
			repaint();

			// calculate the time for how long we should put threat to sleep to meet FPS.
			timeTaken = System.nanoTime() - beginTime;
			timeLeft = (GAME_UPDATE_PERIOD - timeTaken) / milisecInNanosec; // In milliseconds
			// If the time is less than 10 milliseconds, then we will put thread to sleep for 10 millisecond so that some other thread can do some work.
			if (timeLeft < 10) 
				timeLeft = 10; //set a minimum
			try {
				//Provides the necessary delay and also yields control so that other thread can do work.
				Thread.sleep(timeLeft);
			} catch (InterruptedException ex) { }
		}
	}

	/**
	 * Draw the game to the screen. It is called through repaint() method in GameLoop() method.
	 */
	@Override
	public void Draw(Graphics2D g2d)
	{
		switch (gameState)
		{
		case PLAYING:
			game.Draw(g2d, mousePosition());
			break;
		case GAMEOVER:
			//...
			break;
		case MAIN_MENU:
			//...
			break;
		case OPTIONS:
			//...
			break;
		case GAME_CONTENT_LOADING:
			//...
			break;
		case DESTROYED:
			break;
		case STARTING:
			break;
		case VISUALIZING:
			break;
		default:
			break;
		}
	}


	/**
	 * Starts new game.
	 */
	@SuppressWarnings("unused")
	private void newGame()
	{
		// We set gameTime to zero and lastTime to current time for later calculations.
		gameTime = 0;
		lastTime = System.nanoTime();

		game = new Game();
	}

	/**
	 *  Restart game - reset game time and call RestartGame() method of game object so that reset some variables.
	 */
	@SuppressWarnings("unused")
	private void restartGame()
	{
		// We set gameTime to zero and lastTime to current time for later calculations.
		gameTime = 0;
		lastTime = System.nanoTime();

		game.RestartGame();

		// We change game status so that the game can start.
		gameState = GameState.PLAYING;
	}


	/**
	 * Returns the position of the mouse pointer in game frame/window.
	 * If mouse position is null than this method return 0,0 coordinate.
	 * 
	 * @return Point of mouse coordinates.
	 */
	private Point mousePosition()
	{
		try
		{
			Point mp = this.getMousePosition();

			if(mp != null)
				return this.getMousePosition();
			else
				return new Point(0, 0);
		}
		catch (Exception e)
		{
			return new Point(0, 0);
		}
	}


	/**
	 * This method is called when keyboard key is released.
	 * 
	 * @param e KeyEvent
	 */
	@Override
	public void keyReleasedFramework(KeyEvent e)
	{

	}

	/**
	 * This method is called when mouse button is clicked.
	 * 
	 * @param e MouseEvent
	 */
	@Override
	public void mouseClicked(MouseEvent e)
	{

	}
}
