package Game;

import java.awt.Graphics2D;
import java.awt.Image;
import java.awt.Point;
import java.io.IOException;
import java.net.URL;
import java.util.logging.Level;
import java.util.logging.Logger;

import javax.imageio.ImageIO;

import Game.Login;

public class Game {
	// UI lines for the main screen hand drawn in screen ratio.
	 private Image ui_layout;

	public Game()
	{
		Framework.gameState = Framework.GameState.GAME_CONTENT_LOADING;

		Thread threadForInitGame = new Thread() {
			@Override
			public void run(){
				// Sets variables and objects for the game.
				Initialize();
				// Load game files (images, sounds, ...)
				LoadContent();

				Framework.gameState = Framework.GameState.PLAYING;
			}
		};
		threadForInitGame.start();
	}


	/**
	 * Set variables and objects for the game.
	 */
	private void Initialize()
	{
		
	}

	/**
	 * Load game files - images, sounds, ...
	 */
	private void LoadContent()
	{
		try
        {
            URL uiLaoutUrl = this.getClass().getResource("/ui_layout.jpg");
            ui_layout = ImageIO.read(uiLaoutUrl);

        }
        catch (IOException ex) {
            Logger.getLogger(Game.class.getName()).log(Level.SEVERE, null, ex);
        }
		System.out.println(ui_layout.getWidth(null));
        
	}    

	/**
	 * Update game logic.
	 * 
	 * @param gameTime gameTime of the game.
	 * if the game is using the mouse for something. @param mousePosition current mouse position.
	 */
	public void UpdateGame(long gameTime)
	{
	}

	/**
	 * Draw the game to the screen.
	 * 
	 * @param g2d Graphics2D
	 * @param mousePosition current mouse position.
	 */
	public void Draw(Graphics2D g2d, Point mousePosition)
	{
		
		g2d.drawImage(ui_layout, 0, 0, null);
	}


	public void RestartGame() {
		// TODO Auto-generated method stub
		
	}
}