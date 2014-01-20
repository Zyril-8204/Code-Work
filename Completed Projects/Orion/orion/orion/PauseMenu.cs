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
    public class PauseMenu : GameComponent
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
        Rectangle buttonRectangle3;// used for resizing image


        Vector2 cursorPosition = new Vector2(0, 0);
        Texture2D cursorTexture; // Texture for Coursor
        Game1 game;
        public int cursorSelector = 1;
        public bool menuStart = false;
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

        public PauseMenu(Game game)
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
            buttonTexture3 = theContentManager.Load<Texture2D>("ButtonBlankPressed");
            font = theContentManager.Load<SpriteFont>("optionsMenuFont");

            cursorTexture = theContentManager.Load<Texture2D>("selector");



            buttonPosition.X = (screenBounds.Width / 2) - ((buttonTexture.Width - 76) * 2);
            buttonPosition.Y = screenBounds.Height - (buttonTexture.Height - 28);
            buttonRectangle = new Rectangle((int)buttonPosition.X, (int)buttonPosition.Y, 180, 90);

            buttonPosition1.X = (screenBounds.Width / 2);
            buttonPosition1.Y = screenBounds.Height - (buttonTexture.Height - 28);
            buttonRectangle1 = new Rectangle((int)buttonPosition1.X, (int)buttonPosition1.Y, 180, 90);

            buttonPosition2.X = (screenBounds.Width / 2) + ((buttonTexture.Width - 76) * 2);
            buttonPosition2.Y = screenBounds.Height - (buttonTexture.Height - 28);
            buttonRectangle2 = new Rectangle((int)buttonPosition2.X, (int)buttonPosition2.Y, 180, 90);

            cursorPosition.X = buttonPosition.X - 40;
            cursorPosition.Y = (screenBounds.Height - (buttonTexture.Height - 58));

        }

        public override void Update(GameTime gameTime)
        {
            if (cursorSelector == 0)
            {
                cursorSelector = 1;
                cursorPosition.X = buttonPosition.X - 40;
                cursorPosition.Y = (screenBounds.Height - (buttonTexture.Height - 58));
            }

            switch (cursorSelector)
            {
                case 1:
                    {
                        buttonPosition3 = buttonPosition;
                        buttonRectangle3 = buttonRectangle;
                        break;
                    }
                case 2:
                    {
                        buttonPosition3 = buttonPosition1;
                        buttonRectangle3 = buttonRectangle1;
                        break;
                    }
                case 3:
                    {
                        buttonPosition3 = buttonPosition2;
                        buttonRectangle3 = buttonRectangle2;
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
            string back = string.Format("Return");
            string main = string.Format("Main");
            string menu = string.Format("Menu");
            string quit = string.Format("Quit");

            spriteBatch.Draw(buttonTexture, buttonRectangle, Color.White);
            spriteBatch.Draw(buttonTexture1, buttonRectangle1, Color.White);
            spriteBatch.Draw(buttonTexture2, buttonRectangle2, Color.White);
            spriteBatch.Draw(buttonTexture3, buttonRectangle3, Color.White);
            spriteBatch.DrawString(font, back, new Vector2(buttonPosition.X + 40, buttonPosition.Y + 20),
               Color.Red);
            spriteBatch.DrawString(font, main, new Vector2(buttonPosition1.X + 35, buttonPosition1.Y + 15),
                Color.Red);
            spriteBatch.DrawString(font, menu, new Vector2(buttonPosition1.X + 60, buttonPosition1.Y + 40),
                Color.Red);
            spriteBatch.DrawString(font, quit, new Vector2(buttonPosition2.X + 40, buttonPosition2.Y + 20),
                Color.Red);

            spriteBatch.Draw(cursorTexture, new Rectangle((int)cursorPosition.X, (int)cursorPosition.Y, cursorTexture.Width, cursorTexture.Height),
                new Color(color1, color2, color3, (byte)MathHelper.Clamp(fadeAlphaValue, 0, 255)));
        }

        public void MoveCursorRight()
        {
            if (menuStart == true)
            {
                if (cursorSelector <= 2)
                {
                    cursorPosition.X += 360;
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
                    cursorPosition.X -= 360;
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
                    case 1: // return
                        {
                            menuStart = false;
                            game.gamePaused = false;
                            game.gamePlay = true;
                            break;
                        }
                    case 2: // main menu
                        {
                            game.restartTheGame = true;
                            game.restartGame();
                            break;
                        }
                    case 3: // quit
                        {
                            game.Exit();
                            break;
                        }
                }
            }
        }
    }
}