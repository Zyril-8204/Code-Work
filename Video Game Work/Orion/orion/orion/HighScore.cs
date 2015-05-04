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
    public class HighScore : GameComponent
    {
        Game1 game; // create an instance of Game
        public int[] Score;
        public int[] Level;
        public string[] Difficulty;
        public int count = 5;
        Vector2 score1position; // the position of score 1
        Vector2 score2position; // the position of score 2
        Vector2 score3position; // the position of score 3
        Vector2 score4position; // the position of score 4
        Vector2 score5position; // the position of score 5
        protected SpriteFont Font; // a font
        private bool create = false; // creates the highscore screen data if not already created.
        private bool hasCreate = false;
        public bool scoreStart = false;

        // defualt score string values.
        string score1S;
        string score1L;
        string score1D;
        string score2S;
        string score2L;
        string score2D;
        string score3S;
        string score3L;
        string score3D;
        string score4S;
        string score4L;
        string score4D;
        string score5S;
        string score5L;
        string score5D;

        // player's strings when game over is reached.
        public string tmp1D;
        public int tmp1S, tmp1L;

        // Menu stuff
        // buttons!
        Vector2 buttonPosition = new Vector2(0, 0);
        Texture2D buttonTexture; // Texture for Buttons
        Rectangle buttonRectangle; // used for resizing image
        Vector2 buttonPosition1 = new Vector2(0, 0);
        Texture2D buttonTexture1; // Texture for Buttons
        Rectangle buttonRectangle1;// used for resizing image
        Vector2 buttonPosition2 = new Vector2(0, 0);
        Texture2D buttonTexture2; // Texture for Buttons
        Rectangle buttonRectangle2;// used for resizing image
        Vector2 buttonPosition3 = new Vector2(0, 0);
        Texture2D buttonTexture3; // Texture for Buttons
        Rectangle buttonRectangle3; // used for resizing image

        // Cursor!
        Vector2 cursorPosition = new Vector2(0, 0);
        Texture2D cursorTexture; // Texture for Coursor
        public int cursorSelector = 1;

        //Fading!
        int fadeAlphaValue = 1;
        int fadeIncrement = 45;
        double fadeDelay = .035;
        Int32 color1 = 0;
        Int32 color2 = 0;
        Int32 color3 = 0;
        Int32 colorIncrement = 45;

        //Screen
        private Rectangle screenBounds;

        public HighScore(Game game)
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
            Font = theContentManager.Load<SpriteFont>("OptionsMenuFont"); // load font

            buttonTexture = theContentManager.Load<Texture2D>("ButtonBlank");
            buttonTexture1 = theContentManager.Load<Texture2D>("ButtonBlank");
            buttonTexture2 = theContentManager.Load<Texture2D>("ButtonBlank");
            buttonTexture3 = theContentManager.Load<Texture2D>("ButtonBlankPressed");
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

            // initialize the score and level array
            Score = new int[count];
            Level = new int[count];

            //place the scores
            score1position.X = screenBounds.Width / 2; // the position of score 1
            score1position.Y = (screenBounds.Height / 2) - 60;
            score2position.X = screenBounds.Width / 2; // the position of score 2
            score2position.Y = (screenBounds.Height / 2) - 30;
            score3position.X = screenBounds.Width / 2; // the position of score 3
            score3position.Y = screenBounds.Height / 2;
            score4position.X = screenBounds.Width / 2; // the position of score 4
            score4position.Y = (screenBounds.Height / 2) + 30;
            score5position.X = screenBounds.Width / 2;
            score5position.Y = (screenBounds.Height / 2) + 60;
        }

        public override void Update(GameTime gameTime)
        {
            if (game.gameScore == true)
            {
                scoreStart = true;
            }

            if (create == false) // create the HighScore data
            {
                CreateScoreTable(); // creates the table of scores
                hasCreate = true;
                create = true;
            }

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

            //Menu stuff
            string back = string.Format("Back");
            string next = string.Format("Clear");
            string quit = string.Format("Quit");

            spriteBatch.Draw(buttonTexture, buttonRectangle, Color.White);
            spriteBatch.Draw(buttonTexture1, buttonRectangle1, Color.White);
            spriteBatch.Draw(buttonTexture2, buttonRectangle2, Color.White);
            spriteBatch.Draw(buttonTexture3, buttonRectangle3, Color.White);
            spriteBatch.DrawString(Font, back, new Vector2(buttonPosition.X + 35, buttonPosition.Y + 15),
               Color.Red);
            spriteBatch.DrawString(Font, next, new Vector2(buttonPosition1.X + 35, buttonPosition1.Y + 15),
                Color.Red);
            spriteBatch.DrawString(Font, quit, new Vector2(buttonPosition2.X + 35, buttonPosition2.Y + 15),
                Color.Red);

            spriteBatch.Draw(cursorTexture, new Rectangle((int)cursorPosition.X, (int)cursorPosition.Y, cursorTexture.Width, cursorTexture.Height),
                new Color(color1, color2, color3, (byte)MathHelper.Clamp(fadeAlphaValue, 0, 255)));

            // Score Stuff
            if (create == true || hasCreate == true)
            {
                score1S = string.Format("Score: {0}", Score[0]); // draw score string and score
                score1L = string.Format("Level: {0}", Level[0]); // draw score string and score
                score1D = string.Format(" Difficulty: {0}", Difficulty[0]); // draw score string and score
                score2S = string.Format("Score: {0}", Score[1]); // draw score string and score
                score2L = string.Format("Level: {0}", Level[1]); // draw score string and score
                score2D = string.Format(" Difficulty: {0}", Difficulty[1]); // draw score string and score
                score3S = string.Format("Score: {0}", Score[2]); // draw score string and score
                score3L = string.Format("Level: {0}", Level[2]); // draw score string and score
                score3D = string.Format(" Difficulty: {0}", Difficulty[2]); // draw score string and score
                score4S = string.Format("Score: {0}", Score[3]); // draw score string and score
                score4L = string.Format("Level: {0}", Level[3]); // draw score string and score
                score4D = string.Format(" Difficulty: {0}", Difficulty[3]); // draw score string and score
                score5S = string.Format("Score: {0}", Score[4]); // draw score string and score
                score5L = string.Format("Level: {0}", Level[4]); // draw score string and score
                score5D = string.Format(" Difficulty: {0}", Difficulty[4]); // draw score string and score

                // Draw 1st highScore
                spriteBatch.DrawString(Font, score1S, new Vector2(score1position.X, score1position.Y),
                     Color.White); // draw score in correct place on screen
                spriteBatch.DrawString(Font, score1L, new Vector2(score1position.X - 100, score1position.Y),
                    Color.White); // draw score in correct place on screen
                spriteBatch.DrawString(Font, score1D, new Vector2(score1position.X + 150, score1position.Y),
                    Color.White); // draw score in correct place on screen

                // Draw 2nd highScore
                spriteBatch.DrawString(Font, score2S, new Vector2(score2position.X, score2position.Y),
                     Color.White); // draw score in correct place on screen
                spriteBatch.DrawString(Font, score2L, new Vector2(score2position.X - 100, score2position.Y),
                    Color.White); // draw score in correct place on screen
                spriteBatch.DrawString(Font, score2D, new Vector2(score2position.X + 150, score2position.Y),
                    Color.White); // draw score in correct place on screen

                //Draw 3rd highScore
                spriteBatch.DrawString(Font, score3S, new Vector2(score3position.X, score3position.Y),
                    Color.White); // draw score in correct place on screen
                spriteBatch.DrawString(Font, score3L, new Vector2(score3position.X - 100, score3position.Y),
                    Color.White); // draw score in correct place on screen
                spriteBatch.DrawString(Font, score3D, new Vector2(score3position.X + 150, score3position.Y),
                    Color.White); // draw score in correct place on screen

                //Draw 4th highScore
                spriteBatch.DrawString(Font, score4S, new Vector2(score4position.X, score4position.Y),
                    Color.White); // draw score in correct place on screen
                spriteBatch.DrawString(Font, score4L, new Vector2(score4position.X - 100, score4position.Y),
                    Color.White); // draw score in correct place on screen
                spriteBatch.DrawString(Font, score4D, new Vector2(score4position.X + 150, score4position.Y),
                    Color.White); // draw score in correct place on screen

                //Draw 5ht highScore
                spriteBatch.DrawString(Font, score5S, new Vector2(score5position.X, score5position.Y),
                    Color.White); // draw score in correct place on screen
                spriteBatch.DrawString(Font, score5L, new Vector2(score5position.X - 100, score5position.Y),
                    Color.White); // draw score in correct place on screen
                spriteBatch.DrawString(Font, score5D, new Vector2(score5position.X + 150, score5position.Y),
                    Color.White); // draw score in correct place on screen
            }


        }
        private void CreateScoreTable()
        {
            if (create == true || hasCreate == true)
            {
                create = false;
                hasCreate = false;
            }
            Difficulty = new string[count];
            Score[0] = 185000;
            Level[0] = 4;
            Difficulty[0] = "Easy";
            Score[1] = 122000;
            Level[1] = 4;
            Difficulty[1] = "Normal";
            Score[2] = 95000;
            Level[2] = 3;
            Difficulty[2] = "Hard";
            Score[3] = 42000;
            Level[3] = 2;
            Difficulty[3] = "Hard";
            Score[4] = 15500;
            Level[4] = 1;
            Difficulty[4] = "Insane";
            if (create == false || hasCreate == false)
            {
                create = true;
                hasCreate = true;
            }
            
            
            

           
        }

        public void MoveCursorRight()
        {
            if (scoreStart == true)
            {
                if (cursorSelector <= 2)
                {
                    cursorPosition.X += 300;
                    cursorSelector++;
                }
            }
        }

        public void MoveCursorLeft()
        {
            if (scoreStart == true)
            {
                if (cursorSelector > 1)
                {
                    cursorPosition.X -= 300;
                    cursorSelector--;
                }
            }
        }

        public void MenuSelect()
        {
            if (scoreStart == true)
            {
                switch (cursorSelector)
                {
                    case 1: //Back
                        {
                            scoreStart = false;
                            game.gameMenu = true;
                            game.gameScore = false;
                            game.mainMenu.cursorSelector = 0;
                            game.mainMenu.menuStart = true;
                            break;

                        }
                    case 2: // Clear scores
                        {
                            CreateScoreTable();
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
        public void SetHighScore()
        {
            int theCount = 0;
            if (create == false || hasCreate == false)
            {
                CreateScoreTable();
                create = true;
                hasCreate = true;
            }
            
            if (game.optionsMenu.difficulty == 0)
            {
                tmp1D = "Easy";
            }
            else if (game.optionsMenu.difficulty == 1)
            {
                tmp1D = "Normal";
            }
            else if (game.optionsMenu.difficulty == 2)
            {
                tmp1D = "Hard";
            }
            if (game.optionsMenu.difficulty == 3)
            {
                tmp1D = "Insane";
            }
            tmp1L = game.level;
            tmp1S = game.theScore.score;

            for (int i = 0; i < count; i++)
            {
                if (Score[i] >= tmp1S)
                {
                    if (i == 0)
                    {
                        theCount = i;
                    }
                    else if (i == 4)
                    {
                        theCount = i;
                    }
                    else
                    {
                        theCount = i + 1;
                    }
                }
            }
            Difficulty[theCount] = tmp1D;
            Level[theCount] = tmp1L;
            Score[theCount] = tmp1S;
        }
    }
}

