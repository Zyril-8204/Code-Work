/*
 * Enemy Fire Class
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

/*
  * This class will create the Fire for the enemies when they shoot
  * and move them across the screen in an attempt to hit the player
  */
namespace orion
{
    public class EnemyFire : GameComponent
    {
        public Vector2 enemyFirePosition; // Position on screen
        Texture2D enemyTextureFire; // Texture of the Fire
        public Game1 game; // create an instance of Game
        public bool Visible = false; // see if visibile
        bool bossShootLeft, bossShootRight; // see which way the boss will shoot
        public int directionSelect; // select which way the boss will shoot
        private Random random; // the random number generator

        // shoot towards player
        float XDistance, YDistance, rotation;
        public bool Cannon1 = false;
        public bool Cannon2 = false;
        private bool hasShot = false;
        float XDirection;
        float YDirection;
        float XDirection2;
        float YDirection2;

        // width and height of sprite in texture
        protected const int FIRESPRITEWIDTH = 16;
        protected const int FIRESPRITEHEIGHT = 16;


        public EnemyFire(Game game)
            : base(game)
        {
            this.game = (Game1)game; // Set Game to current game
            random = new Random(this.GetHashCode()); // create a random hash to generate random numbers from
        }

        public void LoadContent(ContentManager theContentManager)
        {
            enemyTextureFire = theContentManager.Load<Texture2D>("EnemyFire"); // Load the texture
        }

        public override void Update(GameTime gameTime)
        {
            // If it's not time for the boss shots go straight
            if (game.boss3Active == false && game.boss4Active == false)
            {
                if (Visible == true)
                {
                    enemyFirePosition.Y += 3;
                }

                if ((enemyFirePosition.Y >= Game.Window.ClientBounds.Height) ||
                    (enemyFirePosition.Y >= Game.Window.ClientBounds.Width) || (enemyFirePosition.Y <= 2))
                {
                    Visible = false;
                }
            }


            // if it is time for the boss, we pick a direction then move the 
            // shot a littl to left/right to simulate aim
            if (game.boss3Active == true)
            {
                switch (directionSelect)
                {
                    case 1:
                        {
                            bossShootLeft = true;
                            bossShootRight = false;
                            break;
                        }
                    case 2:
                        {
                            bossShootRight = true;
                            bossShootLeft = false;
                            break;
                        }
                }
                if (Visible == true)
                {
                    if (bossShootLeft == true && game.Boss3.normalShot == false)
                    {
                        enemyFirePosition.Y += 2;
                        enemyFirePosition.X -= 1 + random.Next(1);
                    }
                    if (bossShootRight == true && game.Boss3.normalShot == false)
                    {
                        enemyFirePosition.Y += 2;
                        enemyFirePosition.X += 1 + random.Next(1);
                    }
                    else
                    {
                        enemyFirePosition.Y += 2;
                    }
                }
                // if it is time for the boss, we pick a direction then move the 
                // shot a littl to left/right to simulate aim

                if ((enemyFirePosition.Y >= Game.Window.ClientBounds.Height) ||
                    (enemyFirePosition.Y >= Game.Window.ClientBounds.Width) || (enemyFirePosition.Y <= 2))
                {
                    Visible = false;
                }
            }
            if (game.boss4Active == true)
            {
                if (Cannon1 == true)
                {
                    if (hasShot == false)
                    {
                        XDistance = game.thePlayer.playerPosition.X - game.Boss4.bossGun1Position.X;
                        YDistance = game.thePlayer.playerPosition.Y - game.Boss4.bossGun1Position.Y;
                        hasShot = true;
                    }

                    //Calculate the required rotation by doing a two-variable arc-tan
                    rotation = (float)Math.Atan2(YDistance, XDistance);

                    // move towards the player's direction
                    XDirection = (float)(4 * Math.Cos(rotation));
                    YDirection = (float)(4 * Math.Sin(rotation));

                    enemyFirePosition.X += XDirection;
                    enemyFirePosition.Y += YDirection;
                }
                else if (Cannon2 == true)
                {
                    if (hasShot == false)
                    {
                        XDistance = game.thePlayer.playerPosition.X - game.Boss4.bossGun2Position.X;
                        YDistance = game.thePlayer.playerPosition.Y - game.Boss4.bossGun2Position.Y;
                        hasShot = true;
                    }

                    //Calculate the required rotation by doing a two-variable arc-tan
                    rotation = (float)Math.Atan2(YDistance, XDistance);

                    //Move square towards mouse by closing the gap 3 pixels per update
                    XDirection2 = (float)(4 * Math.Cos(rotation));
                    YDirection2 = (float)(4 * Math.Sin(rotation));


                    enemyFirePosition.X += XDirection2;
                    enemyFirePosition.Y += YDirection2;
                }
                else
                {
                    enemyFirePosition.Y += 5;
                }
                if ((enemyFirePosition.Y >= Game.Window.ClientBounds.Height) ||
                    (enemyFirePosition.Y >= Game.Window.ClientBounds.Width) || (enemyFirePosition.Y <= 2))
                {
                    Visible = false;
                    Cannon1 = false;
                    Cannon2 = false;
                }

            }

        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(enemyTextureFire, enemyFirePosition, Color.White); // draw the shot
        }

        // Put the shot in the right spot, and make it visible
        public void Fire(Vector2 theStartPosition)
        {
            enemyFirePosition = theStartPosition;
            Visible = true;
        }

        // Check to see if the rectangle around the shot is going to collide 
        // with the rectangle around the player.
        public bool CheckCollision(Rectangle rect)
        {
            Rectangle firerect = new Rectangle((int)enemyFirePosition.X, (int)enemyFirePosition.Y,
                        FIRESPRITEWIDTH, FIRESPRITEHEIGHT);
            return firerect.Intersects(rect);
        }
    }
}