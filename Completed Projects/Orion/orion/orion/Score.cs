/*
 * Score Class
 * Created by *Game Company Name*
 * Programmed by: Micah Hawman 
 */
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

// This class creates and draws the score as well as keeps track of the current
// game score (eventually a highscore will be implemented that will also use this
// class, as well as finding out what the highest score possible is.)
namespace orion
{
    public class Score : GameComponent
    {
        Game1 game; // create an instance of Game
        public int score = 0;//100000; // our score variable that updates
        Vector2 position = new Vector2(1125, 3); // the position of the score
        protected SpriteFont Font; // a font

        public Score(Game game)
            : base(game)
        {
            this.game = (Game1)game; // Set Game to current game
        }

        public void LoadContent(ContentManager theContentManager)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}