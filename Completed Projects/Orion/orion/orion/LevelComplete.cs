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
    public class LevelComplete: GameComponent
    {
        Texture2D gradeAATexture;
        Texture2D gradeATexture;
        Texture2D gradeBTexture;
        Texture2D gradeCTexture;
        Texture2D gradeDTexture;
        Texture2D levelComplete;
        Texture2D rank;
        Vector2 gradeAAPosition;
        Vector2 gradeAPosition;
        Vector2 gradeBPosition;
        Vector2 gradeCPosition;
        Vector2 gradeDPosition;
        Vector2 levelCompletePosition;
        Vector2 rankPosition;
        bool gAA = false;
        bool gA = false;
        bool gB= false;
        bool gC = false;
        bool gD = false;
        public bool drawGrade = false;
        Game1 game;
        private Rectangle screenBounds;

        public LevelComplete(Game game)
            : base(game)
        {
            this.game = (Game1)game; // Set Game to current game;
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
            gradeAATexture = theContentManager.Load<Texture2D>("AA");
            gradeATexture = theContentManager.Load<Texture2D>("A");
            gradeBTexture = theContentManager.Load<Texture2D>("B");
            gradeCTexture = theContentManager.Load<Texture2D>("C");
            gradeDTexture = theContentManager.Load<Texture2D>("D");
            rank = theContentManager.Load<Texture2D>("Rank");
            levelComplete = theContentManager.Load<Texture2D>("LevelComplete");
            gradeAAPosition.X = screenBounds.Width / 2;
            gradeAAPosition.Y = screenBounds.Height / 2;
            gradeAPosition.X = screenBounds.Width / 2;
            gradeAPosition.Y = screenBounds.Height / 2;
            gradeBPosition.X = screenBounds.Width / 2;
            gradeBPosition.Y = screenBounds.Height / 2;
            gradeCPosition.X = screenBounds.Width / 2;
            gradeCPosition.Y = screenBounds.Height / 2;
            gradeDPosition.X = screenBounds.Width / 2;
            gradeDPosition.Y = screenBounds.Height / 2;
            levelCompletePosition.X = screenBounds.Width / 2 - 200;
            levelCompletePosition.Y = (screenBounds.Height / 2) - 100;
            rankPosition.X = (screenBounds.Width / 2) - 370;
            rankPosition.Y = screenBounds.Height / 2;
        }
        
        public override void Update(GameTime gameTime)
        {
            if (drawGrade == true)
            {
                if (game.boss1Defeated == true && game.level == 1)
                {
                    if (game.theScore.score >= 47000)
                    {
                        gAA = true;
                    }
                    else if (game.theScore.score >= 41000)
                    {
                        gA = true;
                    }
                    else if (game.theScore.score >= 37500)
                    {
                        gB = true;
                    }
                    else if (game.theScore.score >= 35000)
                    {
                        gC = true;
                    }
                    else
                    {
                        gD = true;
                    }
                }
                if (game.boss2Defeated == true && game.level == 2)
                {
                    if (game.theScore.score >= 104000)
                    {
                        gAA = true;
                    }
                    else if (game.theScore.score >= 89000)
                    {
                        gA = true;
                    }
                    else if (game.theScore.score >= 79000)
                    {
                        gB = true;
                    }
                    else if (game.theScore.score >= 69000)
                    {
                        gC = true;
                    }
                    else
                    {
                        gD = true;
                    }
                }
                if (game.boss3Defeated == true && game.level == 3) 
                {
                    if (game.theScore.score >= 179000)
                    {
                        gAA = true;
                    }
                    else if (game.theScore.score >= 164000)
                    {
                        gA = true;
                    }
                    else if (game.theScore.score >= 144000)
                    {
                        gB = true;
                    }
                    else if (game.theScore.score >= 124000)
                    {
                        gC = true;
                    }
                    else
                    {
                        gD = true;
                    }
                }
                if (game.boss4Defeated == true && game.level == 4)
                {
                    if (game.theScore.score >= 254000)
                    {
                        gAA = true;
                    }
                    else if (game.theScore.score >= 239000)
                    {
                        gA = true;
                    }
                    else if (game.theScore.score >= 219000)
                    {
                        gB = true;
                    }
                    else if (game.theScore.score >= 204000)
                    {
                        gC = true;
                    }
                    else
                    {
                        gD = true;
                    }
                }
            }
            if (game.nextLevel = false || drawGrade == false)
            {
                gA = false;
                gAA = false;
                gB = false;
                gC = false;
                gD = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (drawGrade == true)
            {
                spriteBatch.Draw(rank, rankPosition, Color.White);
                spriteBatch.Draw(levelComplete, levelCompletePosition, Color.White);
                if (gAA == true)
                {
                    spriteBatch.Draw(gradeAATexture, gradeAAPosition, Color.White);
                }
                if (gA == true)
                {
                    spriteBatch.Draw(gradeATexture, gradeAPosition, Color.White);
                }
                if (gB == true)
                {
                    spriteBatch.Draw(gradeBTexture, gradeBPosition, Color.White);
                }
                if (gC == true)
                {
                    spriteBatch.Draw(gradeCTexture, gradeCPosition, Color.White);
                }
                if (gD == true)
                {
                    spriteBatch.Draw(gradeDTexture, gradeDPosition, Color.White);
                }
            }
        }
    }
}
