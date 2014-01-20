using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace orion
{
    public class MainMenu : GameComponent
    {
        Vector2 buttonPosition = new Vector2(0, 0);
        Texture2D buttonTexture; // Texture for Buttons
        Rectangle buttonRectangle; // used for resizing image
        Vector2 buttonPosition1 = new Vector2(0, 0);
        Texture2D buttonTexture1; // Texture for Buttons
        Rectangle buttonRectangle1; // used for resizing image
        Vector2 buttonPosition2 = new Vector2(0, 0);
        Texture2D buttonTexture2; // Texture for Buttons
        Rectangle buttonRectangle2; // used for resizing image
        Vector2 buttonPosition3 = new Vector2(0, 0);
        Texture2D buttonTexture3; // Texture for Buttons
        Rectangle buttonRectangle3; // used for resizing image
        Vector2 buttonPosition4 = new Vector2(0, 0);
        Texture2D buttonTexture4; // Texture for Buttons
        Rectangle buttonRectangle4;// used for resizing image
        Vector2 buttonPosition5 = new Vector2(0, 0);
        Texture2D buttonTexture5; // Texture for Buttons
        Rectangle buttonRectangle5;// used for resizing image

        Vector2 cursorPosition = new Vector2(0, 0);
        Texture2D cursorTexture; // Texture for Coursor
        Game1 game;
        public int cursorSelector = 1;
        public bool menuStart = true;
        private Rectangle screenBounds;
        SpriteFont font;

        // fading cursor
        int fadeAlphaValue = 1;
        int fadeIncrement = 45;
        double fadeDelay = .035;
        Int32 color1 = 0;
        Int32 color2 = 0;
        Int32 color3 = 0;
        Int32 colorIncrement = 45;

        public MainMenu(Game game)
            : base(game)
        {
            this.game = (Game1)game; // Set Game to current game
#if XBOX360
            //on the 360, we need to be carefule about the TV's "safe" area.
            screenBounds = new Rectangle(
                    (int)(Game.Window.ClientBounds.Width * 0.03f),
                    (int)(Game.Window.ClientBounds.Height * 0.03f),
                    Game.Window.ClientBounds.Width -
                    (int)(Game.Window.ClientBounds.Width * 0.03f),
                    Game.Window.ClientBounds.Height -
                    (int)(Game.Window.ClientBounds.Height * 0.03f));
#else
            screenBounds = new Rectangle(0, 0,
                Game.Window.ClientBounds.Width,
                Game.Window.ClientBounds.Height);
#endif
        }

        public void LoadContent(ContentManager theContentManager)
        {
            buttonTexture = theContentManager.Load<Texture2D>("ButtonBlank");
            buttonTexture1 = theContentManager.Load<Texture2D>("ButtonBlank");
            buttonTexture2 = theContentManager.Load<Texture2D>("ButtonBlank");
            buttonTexture3 = theContentManager.Load<Texture2D>("ButtonBlank");
            buttonTexture4 = theContentManager.Load<Texture2D>("ButtonBlank");
            buttonTexture5 = theContentManager.Load<Texture2D>("ButtonBlankPressed");
            cursorTexture = theContentManager.Load<Texture2D>("selector");
            font = theContentManager.Load<SpriteFont>("MainMenuFont");

            cursorPosition.X = 50;
            cursorPosition.Y = screenBounds.Height - (buttonTexture.Height - 48);

            buttonPosition.X = (cursorTexture.Width) + 50;
            buttonPosition.Y = screenBounds.Height - (buttonTexture.Height - 28);
            buttonRectangle = new Rectangle((int)buttonPosition.X, (int)buttonPosition.Y, 180, 90);

            buttonPosition1.X = buttonPosition.X + (buttonTexture1.Width - 76) + (cursorTexture.Width + 20);
            buttonPosition1.Y = screenBounds.Height - (buttonTexture.Height - 28);
            buttonRectangle1 = new Rectangle((int)buttonPosition1.X, (int)buttonPosition1.Y, 180, 90);

            buttonPosition2.X = buttonPosition1.X + (buttonTexture2.Width - 76) + (cursorTexture.Width + 20);
            buttonPosition2.Y = screenBounds.Height - (buttonTexture.Height - 28);
            buttonRectangle2 = new Rectangle((int)buttonPosition2.X, (int)buttonPosition2.Y, 180, 90);

            buttonPosition3.X = buttonPosition2.X + (buttonTexture3.Width - 76) + (cursorTexture.Width + 20);
            buttonPosition3.Y = screenBounds.Height - (buttonTexture.Height - 28);
            buttonRectangle3 = new Rectangle((int)buttonPosition3.X, (int)buttonPosition3.Y, 180, 90);

            buttonPosition4.X = buttonPosition3.X + (buttonTexture4.Width - 76) + (cursorTexture.Width + 20);
            buttonPosition4.Y = screenBounds.Height - (buttonTexture.Height - 28);
            buttonRectangle4 = new Rectangle((int)buttonPosition4.X, (int)buttonPosition4.Y, 180, 90);
        }

        public override void Update(GameTime gameTime)
        {
            
            if (cursorSelector == 0)
            {
                cursorSelector = 1;
                cursorPosition.X = 50;
                cursorPosition.Y = screenBounds.Height - (buttonTexture.Height - 48);
            }

            switch (cursorSelector)
            {
                case 1:
                    {
                        buttonPosition5 = buttonPosition;
                        buttonRectangle5 = buttonRectangle;
                        break;
                    }
                case 2:
                    {
                        buttonPosition5 = buttonPosition1;
                        buttonRectangle5 = buttonRectangle1;
                        break;
                    }
                case 3:
                    {
                        buttonPosition5 = buttonPosition2;
                        buttonRectangle5 = buttonRectangle2;
                        break;
                    }
                case 4:
                    {
                        buttonPosition5 = buttonPosition3;
                        buttonRectangle5 = buttonRectangle3;
                        break;
                    }
                case 5:
                    {

                        buttonPosition5 = buttonPosition4;
                        buttonRectangle5 = buttonRectangle4;
                        break;
                    }
            }

            // Cause the cursor to fade in and out
            fadeDelay -= gameTime.ElapsedGameTime.TotalSeconds;

            if (fadeDelay <= 0)
            {
                fadeDelay = .035;
                fadeAlphaValue += fadeIncrement;
                color1 += colorIncrement;
                color2 += colorIncrement;
                color3 += colorIncrement;


                if (fadeAlphaValue >= 255 || fadeAlphaValue <= 0)
                {
                    fadeIncrement *= -1;
                    colorIncrement *= -1;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            string start = string.Format("Start");
            string scores = string.Format("Scores");
            string options = string.Format("Options");
            string help = string.Format("Help");
            string quit = string.Format("Quit");


            spriteBatch.Draw(buttonTexture, buttonRectangle, Color.White);
            spriteBatch.Draw(buttonTexture1, buttonRectangle1, Color.White);
            spriteBatch.Draw(buttonTexture2, buttonRectangle2, Color.White);
            spriteBatch.Draw(buttonTexture3, buttonRectangle3, Color.White);
            spriteBatch.Draw(buttonTexture4, buttonRectangle4, Color.White);
            spriteBatch.Draw(buttonTexture5, buttonRectangle5, Color.White);

            spriteBatch.DrawString(font, start, new Vector2(buttonPosition.X + 40, buttonPosition.Y + 20),
                Color.Red);
            spriteBatch.DrawString(font, scores, new Vector2(buttonPosition1.X + 40, buttonPosition1.Y + 20),
                Color.Red);
            spriteBatch.DrawString(font, options, new Vector2(buttonPosition2.X + 40, buttonPosition2.Y + 20),
                Color.Red);
            spriteBatch.DrawString(font, help, new Vector2(buttonPosition3.X + 40, buttonPosition3.Y + 20),
                Color.Red);
            spriteBatch.DrawString(font, quit, new Vector2(buttonPosition4.X + 40, buttonPosition4.Y + 20),
                Color.Red);
            spriteBatch.Draw(cursorTexture, new Rectangle((int)cursorPosition.X, (int)cursorPosition.Y, cursorTexture.Width, cursorTexture.Height),
                new Color(color1, color2, color3, (byte)MathHelper.Clamp(fadeAlphaValue, 0, 255)));
        }

        public void MoveCursorRight()
        {
            if (menuStart == true)
            {
                if (cursorSelector <= 4)
                {
                    cursorPosition.X += 230;
                    cursorSelector++;
                }
            }
        }

        public void MoveCursorLeft()
        {
            if (menuStart == true)
            {
                if (cursorSelector > 1)
                {
                    cursorPosition.X -= 230;
                    cursorSelector--;
                }
            }
        }

        public void MenuSelect()
        {
            if (menuStart == true)
            {
                switch (cursorSelector)
                {
                    case 1: // start game
                        {
                            menuStart = false;
                            game.gameMenu = false;
                            game.gamePlay = true;
                            game.playSong();

                            break;
                        }
                    case 2: // high scores
                        {
                            menuStart = false;
                            game.gameMenu = false;
                            game.gameScore = true;
                            break;
                        }
                    case 3: // options
                        {
                            menuStart = false;
                            game.gameMenu = false;
                            game.gameOptions = true;
                            game.optionsMenu.showOptions = true;
                            break;
                        }
                    case 4: // help
                        {
                            menuStart = false;
                            game.gameMenu = false;
                            game.gameHelp = true;
                            game.helpMenu.menuStart = true;
                            break;
                        }
                    case 5: // quit
                        {

                            game.Exit();
                            break;
                        }
                }
            }
        }
    }
}