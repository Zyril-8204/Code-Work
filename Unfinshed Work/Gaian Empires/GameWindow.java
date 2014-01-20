package Game;
import java.awt.Frame;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;

import javax.swing.*;

import Game.Login;


//our window class that will set up the framework of the game
public class GameWindow {
	public static boolean stillLoading = true;
	public static GridBagConstraints gBC = new GridBagConstraints();
	public static JFrame frame = new JFrame();
	private static boolean useFullScreen = false;
	
	public GameWindow() {



		//Set game Title
		frame.setTitle("Gaian Empires");
		frame.setLayout(new GridBagLayout());

		//set size of the frame
		if(useFullScreen) { // full screen

			// Disable decorations for the frame.
			frame.setUndecorated(true);
			//put frame to full screen.
			frame.setExtendedState(Frame.MAXIMIZED_BOTH);
		} else { // windowed mode
			// set size of the frame
			frame.setSize(1024,768);
			// put frame to center of screen
			frame.setLocationRelativeTo(null);
			// make it so frame can not be resized
			frame.setResizable(false);
		}

		//Exit the application when user closes frame
		frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);

		// create instance of the framework so that it extends the canvas 
		// class and puts it to frame
		Login.CreateLogin();	
		frame.add(new Framework());
		frame.setVisible(true);

		stillLoading = false;
	}

	public static void main(String[] args) {
		// Use the event dispatch to build UI for safety.
		SwingUtilities.invokeLater(new Runnable() {
			@Override
			public void run() {
				new GameWindow();
			}
		});
	}
}
