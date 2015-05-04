using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace orion
{
    class gun
    {
        public Vectors gunLocation;
        public int health = 3;
        public float dir = 0;
        int shootPause = 100;
        int wait = 0;

        public gun(Vectors vect1)
        {
            gunLocation = vect1;
        }

        public bool isDead()
        {
            if (health <= 0 || gunLocation.y > 1000)
                return true;
            else
                return false;
        }

        public bool update(Game1 game, Vectors direction)
        {
            //get direction
            Vectors temp = new Vectors(game.thePlayer.playerPosition.X, game.thePlayer.playerPosition.Y);
            dir = Vectors.getDirection(gunLocation, temp);

            //update position
            gunLocation = gunLocation + direction;

            //test to shoot
            if (wait >= shootPause)
            {
                wait = 0;
                return true;    //shoot bullet
            }
            else
            {
                wait++;
                return false;   //don't shoot
            }
        }

        public void draw(SpriteBatch sb, Texture2D texture)
        {
            sb.Draw(texture, new Rectangle((int)gunLocation.x, (int)gunLocation.y, 81, 84), null, Color.White, dir * 0.01745329f, new Vector2(40, 42), SpriteEffects.None, 0.0f);
        }

    }
}