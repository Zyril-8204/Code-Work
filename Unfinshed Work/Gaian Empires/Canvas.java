package Game;

import java.awt.*;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;
import java.awt.image.BufferedImage;

import javax.swing.*;

public abstract class Canvas extends JPanel implements KeyListener, MouseListener {

	// Keyboard states - Here are stored states for keyboard keys - is it down or not.
	private static boolean[] keyboardState = new boolean[525];

	// Mouse States - here are the stored mouse states for mouse key being down or not
	private static boolean[] mouseState = new boolean[3];
	
	private static boolean noMouse = false; // removes mouse pointer from game.

	public Canvas() {
		// use double buffer to draw the screen
		this.setDoubleBuffered(true);
		this.setFocusable(true);
		//this.setBackground(Color.black);

		// If you will draw your own mouse cursor or if you just want that mouse cursor disappear, 
		// insert "true" into if condition and mouse cursor will be removed.
		if (noMouse) {
			BufferedImage blankCursorImg = new BufferedImage(16, 16, BufferedImage.TYPE_INT_ARGB);
			Cursor blankCursor = Toolkit.getDefaultToolkit().createCustomCursor(blankCursorImg, new Point(0, 0), null);
			this.setCursor(blankCursor);
		}

		// adds keyboard listener to receive events from jpanel
		this.addKeyListener(this);
		// adds mouse listener to receive events from jpanel
		this.addMouseListener(this);
	}

	// This method is Override in Framework.java and is used for drawing to the screen
	public abstract void Draw(Graphics2D g2d);

	@Override	
	public void paintComponent(Graphics g) {
		
		Graphics2D g2d = (Graphics2D)g;
		super.paintComponent(g2d);
		Draw(g2d);
	}

	// Keyboard
	/**
	 * Is keyboard key "key" down?
	 * 
	 * @param key Number of key for which you want to check the state.
	 * @return true if the key is down, false if the key is not down.
	 */
	public static boolean keyboardKeyState(int key) {
		return keyboardState[key];
	}

	// Methods of keyboard listener
	@Override
	public void keyPressed(KeyEvent e) {
		keyboardState[e.getKeyCode()] = true;
	}

	@Override
	public void keyReleased(KeyEvent e) {
		keyboardState[e.getKeyCode()] = false;
		keyReleasedFramework(e);
	}

	@Override
	public void keyTyped(KeyEvent e){
	}

	public abstract void keyReleasedFramework(KeyEvent e);
	// Mouse
	/**
	 * Is mouse button "button" down?
	 * @param button Number of mouse button for which you want to check the state.
	 * @return true if the button is down, false if the button is not down.
	 */
	public static boolean mouseButtonState(int button)
	{
		return mouseState[button - 1];
	}

	// Sets mouse key status.
	private void mouseKeyStatus(MouseEvent e, boolean status)
	{
		if(e.getButton() == MouseEvent.BUTTON1)
			mouseState[0] = status;
		else if(e.getButton() == MouseEvent.BUTTON2)
			mouseState[1] = status;
		else if(e.getButton() == MouseEvent.BUTTON3)
			mouseState[2] = status;
	}

	// Methods of the mouse listener.
	@Override
	public void mousePressed(MouseEvent e)
	{
		mouseKeyStatus(e, true);
	}

	@Override
	public void mouseReleased(MouseEvent e)
	{
		mouseKeyStatus(e, false);
	}

	@Override
	public void mouseClicked(MouseEvent e) { }

	@Override
	public void mouseEntered(MouseEvent e) { }

	@Override
	public void mouseExited(MouseEvent e) { }
}