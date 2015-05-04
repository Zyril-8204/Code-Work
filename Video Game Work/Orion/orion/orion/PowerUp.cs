/*
 * Power Up Class
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

// This class copies over the information from the enemy as it dies so it can 
// create a powerup and display it to the screen
namespace orion
{
    public class PowerUp : GameComponent
    {
        public Game1 game; // create an instance of Game
        public Vector2 powerUpPosition; // Position of powerup on screen
        public bool Visible; // is powerup visible
        public int powerUpType; // type of powerup
        public bool dropPowerUp; // is a powerup going to drop
        public int decayTime = 5500; // how long the powerup will remain the screen
        public int decayTimer = 0; // counter to find out if powerup should be removed
        public Texture2D powerUpTexture; // texture of powerup
        protected Rectangle powerUpRectangle; // rectangle around powerup

        // width and height of sprite in texture
        public int PUWidth = 64;
        public int PUHeight = 64;

        public PowerUp(Game game)
            : base(game)
        {
            this.game = (Game1)game; // Set Game to current game

            // create the source rectangle
            // this represents where the sprite picture is in the surface
            powerUpRectangle = new Rectangle(0, 0, PUWidth, PUHeight);
        }

        public override void Update(GameTime gameTime)
        {
            powerUpPosition.Y += 1;
            // copy over the data from the enemy class
            foreach (Enemy theEnemy in game.theEnemy)
            {
                if (theEnemy.enemyDeath == true && theEnemy.dropPowerUp == true)
                {
                    powerUpType = theEnemy.powerSelector;
                    Visible = theEnemy.powerVisible;
                    dropPowerUp = theEnemy.dropPowerUp;
                    powerUpPosition = theEnemy.enemyPosition;
                    powerUpTexture = theEnemy.powerUpTexture;
                }
            }

            // copy over the data from the boss enemy class
            if (game.boss3Active == true)
            {
                foreach (BossEnemy theEnemy in game.Boss3.bossEnemies)
                {
                    if (theEnemy.enemyDeath == true && theEnemy.dropPowerUp == true)
                    {
                        powerUpType = theEnemy.powerSelector;
                        Visible = theEnemy.powerVisible;
                        dropPowerUp = theEnemy.dropPowerUp;
                        powerUpPosition = theEnemy.enemyPosition;
                        powerUpTexture = theEnemy.powerUpTexture;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // draw the powerup
            if (Visible == true)
            {
                spriteBatch.Draw(powerUpTexture, powerUpPosition, powerUpRectangle, Color.White);
            }
        }

        // Check to see if the rectangle around the powerup is going to collide 
        // with the rectangle around the player.
        public bool CheckCollision(Rectangle rect)
        {
            Rectangle powerrect = new Rectangle((int)powerUpPosition.X, (int)powerUpPosition.Y,
                        PUWidth, PUHeight);
            return powerrect.Intersects(rect);
        }
    }
}