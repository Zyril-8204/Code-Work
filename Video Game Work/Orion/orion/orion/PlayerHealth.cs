/*
 * Player Health Class
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
 * This class will keep track of the player's health, when they gain and loose
 * health and flash the screen and rumble the controller when hit by enemy fire
 */
namespace orion
{
    public class PlayerHealth : GameComponent
    {
        Texture2D theHealthBar; // Texture of health bar
        public float theCurrentHealth = 100; // current player health
        public bool checkAttack; // see if player is about to die or loose health
        public Game1 game; // create an instance of Game
        public bool rumble = false; // see if the controller will rumple
        private int timer = 0; // a counter for rumble time
        private int rumbleTime = 500; // the timer for rumble effect
        public float damage = 10;



        public PlayerHealth(Game game)
            : base(game)
        {
            this.game = (Game1)game; // Set Game to current game
        }

        public void LoadContent(ContentManager theContentManager, string theAssetName)
        {
            // load the textures
            theHealthBar = theContentManager.Load<Texture2D>(theAssetName);

        }

        public override void Update(GameTime gameTime)
        {
            // see if player been hit, if player has call Attack
            if (checkAttack == true)
            {
                if (game.hasFlashed == false)
                {
                    game.flashScreen = true;
                }
                if (game.flashScreen == false && game.hasFlashed == true)
                {
                    Attack(gameTime, damage);
                    game.hasFlashed = false;
                }
            }
            if (game.gameOver.continueGame == true)
            {
                theCurrentHealth = 100;
            }
            // see if player hass bene hit and start the flash timer if so



            // see if player has been hit, and if so start rumble and timer
            if (rumble == true)
            {
                if (timer >= rumbleTime)
                {
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                    timer = 0;
                    rumble = false;

                }
                else
                {
                    GamePad.SetVibration(PlayerIndex.One, 1f, 0f);
                    timer += gameTime.ElapsedGameTime.Milliseconds;
                }
            }
            // Force the health to stay between 0 and 100
            theCurrentHealth = (int)MathHelper.Clamp(theCurrentHealth, 0, 100);
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            // To draw the health bar adjust the first ant second rectangle, first 0,0 is X,Y placement,
            // next 0,0 are width and height of bar. Second rectangle belongs to the source.

            // Draw the negative space in the Shield bar
            spriteBatch.Draw(theHealthBar, new Rectangle(10, 30, 50, 10), new Rectangle(0, 10, theHealthBar.Width, 10), Color.Transparent);

            // Draw the current shields level based on the the current shield
            spriteBatch.Draw(theHealthBar, new Rectangle(10, 30, (int)(50 * ((double)theCurrentHealth / 100)), 10),
            new Rectangle(0, 45, theHealthBar.Width, 10), Color.Red);

            // The boarder must be 1 pixel higher than the shield bar.
            // draw the boarder of shield bar
            spriteBatch.Draw(theHealthBar, new Rectangle(10, 29, 50, 11), new Rectangle(0, 0, theHealthBar.Width, 11), Color.White);
        }

        // if the player has been hit, remove health
        public void Attack(GameTime gameTime, float damage)
        {
            theCurrentHealth -= damage;
            game.audio.soundPlay(game.audio.sfx_hit);
            CheckAlive();
        }

        // if the player picked up the healign power up, restore health
        public void Heal()
        {

            if (theCurrentHealth < 100)
            {
                theCurrentHealth += 10;
            }
        }

        // see if the player is still alive after being hit
        private void CheckAlive()
        {
            if (theCurrentHealth < 0)
            {
                if (checkAttack == true)
                {
                    game.audio.soundPlay(game.audio.sfx_playerDie);
                    game.playerDeath = true;
                    theCurrentHealth += 110;
                }
            }
            else checkAttack = false;
        }
    }
}