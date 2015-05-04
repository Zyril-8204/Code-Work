/*
 * Player Fire Class
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
  * This class will create the Fire for the player when they shoot
  * and move them across the screen in an attempt to hit the enemies
  */
namespace orion
{
    public class PlayerFire : GameComponent
    {
        public Vector2 firePosition; // Position on screen
        Texture2D playerTextureFire; // Texture of the Fire

        public Game1 game; // create an instance of Game
        public bool Visible = false; // see if visibile

        // width and height of sprite in texture
        protected const int FIRESPRITEWIDTH = 16;
        protected const int FIRESPRITEHEIGHT = 16;




        public PlayerFire(Game game)
            : base(game)
        {
            this.game = (Game1)game; // Set Game to current game
        }

        public void LoadContent(ContentManager theContentManager)
        {
            playerTextureFire = theContentManager.Load<Texture2D>("Fire"); // Load the texture           
        }

        public override void Update(GameTime gameTime)
        {
            // if it's visible move the position
            if (Visible == true)
            {
                firePosition.Y -= 5;
            }

            // if it's off the screen turn off visible
            if ((firePosition.Y >= Game.Window.ClientBounds.Height) ||
                (firePosition.Y >= Game.Window.ClientBounds.Width) || (firePosition.Y <= 2))
            {
                Visible = false;
            }
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(playerTextureFire, firePosition, Color.White); // draw the shot
        }

        // Put the shot in the right spot, and make it visible
        public void Fire(Vector2 theStartPosition, bool isSpreading)
        {
            firePosition = theStartPosition;
            Visible = true;
        }

        // Check to see if the rectangle around the shot is going to collide 
        // with the rectangle around the enemy.
        public bool CheckCollision(Rectangle rect)
        {
            Rectangle firerect = new Rectangle((int)firePosition.X, (int)firePosition.Y,
                        FIRESPRITEWIDTH, FIRESPRITEHEIGHT);
            if (firerect.Intersects(rect))
            {
                game.audio.soundPlay(game.audio.sfx_hit);
                return true;
            }
            else
                return false;
        }
    }
}