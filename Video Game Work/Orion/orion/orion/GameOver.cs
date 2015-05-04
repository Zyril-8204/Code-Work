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
    public class GameOver : GameComponent
    {
        Texture2D gameOverTexture;
        Vector2 gameOverPosition = new Vector2(0, 0);
        Texture2D continueTexture;
        Vector2 continuePosition = new Vector2(0, 0);
        Texture2D cursorTexture;
        Vector2 cursorPosition = new Vector2(0, 0);
        Vector2 buttonPosition = new Vector2(0, 0);
        Texture2D buttonTexture; // Texture for Buttons
        Rectangle buttonRectangle; // used for resizing image
        Vector2 buttonPosition1 = new Vector2(0, 0);
        Texture2D buttonTexture1; // Texture for Buttons
        Rectangle buttonRectangle1; // used for resizing image
        Vector2 buttonPosition2 = new Vector2(0, 0);
        Texture2D buttonTexture2; // Texture for Buttons
        Rectangle buttonRectangle2; // used for resizing image
        int selector = 1;
        Game1 game;
        private Rectangle screenBounds;
        public bool gameOver = false;
        public bool continueGame = false;
        SpriteFont font;

        // fading cursor
        int fadeAlphaValue = 1;
        int fadeIncrement = 45;
        double fadeDelay = .035;
        Int32 color1 = 0;
        Int32 color2 = 0;
        Int32 color3 = 0;
        Int32 colorIncrement = 45;

        public GameOver(Game game)
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
            buttonTexture2 = theContentManager.Load<Texture2D>("ButtonBlankPressed");
            cursorTexture = theContentManager.Load<Texture2D>("selector");
            gameOverTexture = theContentManager.Load<Texture2D>("GameOver");
            continueTexture = theContentManager.Load<Texture2D>("GameOver");
            font = theContentManager.Load<SpriteFont>("MainmenuFont");

            buttonPosition.X = (screenBounds.Width / 2) - ((buttonTexture.Width - 76) * 2);
            buttonPosition.Y = screenBounds.Height - (buttonTexture.Height - 28);
            buttonRectangle = new Rectangle((int)buttonPosition.X, (int)buttonPosition.Y, 180, 90);

            buttonPosition1.X = (screenBounds.Width / 2);
            buttonPosition1.Y = screenBounds.Height - (buttonTexture.Height - 28);
            buttonRectangle1 = new Rectangle((int)buttonPosition1.X, (int)buttonPosition1.Y, 180, 90);

            cursorPosition.X = buttonPosition.X - 40;
            cursorPosition.Y = (screenBounds.Height - (buttonTexture.Height - 58));

            continuePosition.X = (screenBounds.Width / 2) - (continueTexture.Width / 2);
            continuePosition.Y = (screenBounds.Height / 2) + (continueTexture.Height) + 30;
            gameOverPosition.X = (screenBounds.Width / 2) - (continueTexture.Width / 2);
            gameOverPosition.Y = (screenBounds.Height / 2) - 100;
        }

        public override void Update(GameTime gameTime)
        {
            if (selector == 0)
            {
                selector = 1;
                cursorPosition.X = buttonPosition.X - 40;
                cursorPosition.Y = (screenBounds.Height - (buttonTexture.Height - 58));
            }

            switch (selector)
            {
                case 1:
                    {
                        buttonPosition2 = buttonPosition;
                        buttonRectangle2 = buttonRectangle;
                        break;
                    }
                case 2:
                    {
                        buttonPosition2 = buttonPosition1;
                        buttonRectangle2 = buttonRectangle1;
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
            string Continue = string.Format("Continue");
            string quit = string.Format("Quit");
            spriteBatch.Draw(buttonTexture, buttonRectangle, Color.White);
            spriteBatch.Draw(buttonTexture1, buttonRectangle1, Color.White);
            spriteBatch.Draw(buttonTexture2, buttonRectangle2, Color.White);

            spriteBatch.DrawString(font, Continue, new Vector2(buttonPosition.X + 40, buttonPosition.Y + 20),
               Color.Red);

            spriteBatch.DrawString(font, quit, new Vector2(buttonPosition1.X + 40, buttonPosition2.Y + 20),
                Color.Red);

            spriteBatch.Draw(cursorTexture, new Rectangle((int)cursorPosition.X, (int)cursorPosition.Y, cursorTexture.Width, cursorTexture.Height),
                new Color(color1, color2, color3, (byte)MathHelper.Clamp(fadeAlphaValue, 0, 255)));
            spriteBatch.Draw(gameOverTexture, gameOverPosition, Color.White);
            //spriteBatch.Draw(continueTexture, continuePosition, Color.White);

        }


        public void MoveCursorRight()
        {
            if (game.gameIsOver == true)
            {
                if (selector <= 1)
                {
                    cursorPosition.X += 300;
                    selector++;
                }
            }
        }

        public void MoveCursorLeft()
        {
            if (game.gameIsOver == true)
            {
                if (selector > 1)
                {
                    cursorPosition.X -= 300;
                    selector--;
                }
            }
        }
        public void MenuSelect()
        {
            switch (selector)
            {
                case 1: //Continue
                    {
                        continueGame = true;
                        selector = 0;
                        break;
                    }


                case 2: //Quit
                    {
                        game.Exit();
                        break;
                    }
            }
        }
    }
}