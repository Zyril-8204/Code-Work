using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace orion
{
    class Rock
    {
        public int health = 10;     //How many hits it takes to kill rock
        Random rnd = new Random();  //Randomizer
        Vector2 direction;          //Direction rock moves
        public Vector2 position;    //Current Position
        public bool part = false;   //Whether the rock is broken or not
        Texture2D rockSprite;       //Rock's sprite


        //Constructor for adding normal rocks
        public Rock(Game1 game)
        {


            direction = new Vector2(rnd.Next(-3, 3), rnd.Next(2, 5)); //Picks random direction for rock
            position = new Vector2(rnd.Next(0, 1280), -128); //Places rock at top of the screen

            part = false;   //Rock is not broken
            rockSprite = game.Content.Load<Texture2D>("Rock");
        }

        //Constructor for adding broken rocks
        public Rock(Game1 game, Vector2 vect, Random rand)
        {
            rnd = rand; //Uses manager's random value
            position = vect;
            part = true;    //Rock is broken
            rockSprite = game.Content.Load<Texture2D>("RockPart");
            direction = new Vector2(rnd.Next(-5, 5), rnd.Next(-5, 5));  //Randomizes direction with wider range of directions
            if (direction.Y == 0)   //Makes sure nock is not stationary
                direction.Y = 1;
            if (direction.X == 0)   //Makes sure nock is not stationary
                direction.X = 1;


        }

        //This updates the rocks position
        public void update()
        {
            position.X += direction.X;
            position.Y += direction.Y;
        }

        //Drawing function
        public void draw(SpriteBatch sb)
        {
            Rectangle destination = new Rectangle((int)position.X, (int)position.Y, rockSprite.Width, rockSprite.Height);
            sb.Draw(rockSprite, destination, Color.White);
        }
    }
}