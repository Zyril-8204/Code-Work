/*
 * Player Class
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
  * This is the main player, all functions behind the player should channel through here.
  */
namespace orion
{
    public class Player : GameComponent
    {
        public Vector2 playerPosition = new Vector2(200, 200); // player position
        public Texture2D playerShipTexture; // player texture
        protected Rectangle playerRectangle; // the rectangle around the player
        public bool initialGun = true; // gun for shooting
        public bool rapidGun = false; // faster gun for shooting
        public bool spreadGun = false; // even faster gun for shooting
        public bool missiles = false; // to be implemented later
        public int gunSelector = 1; // which gun do we use
        public Vector2 firePosition = new Vector2(0, 0); // position of the shot
        public List<PlayerFire> thePlayerFire = new List<PlayerFire>(); // list of player shots
        public List<Rockets> thePlayerRocketFire = new List<Rockets>(); // list of player shots
        public Game1 game; // create an instance of Game
        public int playerSpeed = 12; //Player movement speed
        public int nuke = 3; // Nuke ammo
        private bool flash = false; // flash for using a nuke
        private int flashTimer; // counter to check if it's time to stop flash
        private int flashTime = 200; // timer fgor how long to flash
        Texture2D theFlash; // texture to flash
        protected SpriteFont Font; // a font
        protected Vector2 fontPosition = new Vector2(0, 40); // the position of the font
        Random rnd = new Random();
        public bool won = false;

        ContentManager fireContentManager;


        // width and height of sprite in texture
        protected const int PSHIPWIDTH = 128;
        protected const int PSHIPHEIGHT = 128;

        protected Rectangle screenBounds; // keep the player in safe screen area

        public int heat = 0;
        int maxHeat = 3000;
        int hurtSpacer = 0;

        public Player(Game game)
            : base(game)
        {
            this.game = (Game1)game; // Set Game to current game
            playerPosition = new Vector2(); // reset the vector

            // create the source rectangle
            // this represents where the sprite picture is in the surface
            playerRectangle = new Rectangle(0, 0, PSHIPWIDTH, PSHIPHEIGHT);
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

        // Places the player in the staring position in the game (bottom middle)
        public void PutinStartPosition()
        {
            playerPosition.X = screenBounds.Width / 2;
            playerPosition.Y = screenBounds.Height - PSHIPHEIGHT;
        }

        public void LoadContent(ContentManager theContentManager, string theAssetName)
        {
            // load the content managers
            fireContentManager = theContentManager;

            foreach (PlayerFire playerShot in thePlayerFire)
            {
                playerShot.LoadContent(theContentManager); // load the shots
            }
            foreach (Rockets rocket in thePlayerRocketFire)
            {
                
            }

            // load the textures
            playerShipTexture = theContentManager.Load<Texture2D>(theAssetName);
            theFlash = theContentManager.Load<Texture2D>("Flash");
        }

        public override void Update(GameTime gameTime)
        {
            //Over heat system
            if (game.playerInput.isFiring())    //While player is shooting
            {
                if (heat < maxHeat) //Heat weapons depending on your power up
                {
                    heat += 3;
                }
            }
            else if (heat > 0)
                heat -= 10;      //Release to cool weapons

            if (heat >= maxHeat)
            {
                if (hurtSpacer == 30)
                {
                    game.audio.soundPlay(game.audio.sfx_alarm);
                    game.thePlayerHealth.rumble = true;
                    game.thePlayerHealth.checkAttack = true;
                    hurtSpacer = 0;
                }
                else
                    hurtSpacer++;
            }

            if (!won)
            {

                // keep the ship inside the screen
                if (playerPosition.X < screenBounds.Left)
                {
                    playerPosition.X = screenBounds.Left;
                }
                if (playerPosition.X > screenBounds.Width - PSHIPWIDTH)
                {
                    playerPosition.X = screenBounds.Width - PSHIPWIDTH;
                }
                if (playerPosition.Y < (screenBounds.Top + 64))
                {
                    playerPosition.Y = screenBounds.Top + 64;
                }
                if (playerPosition.Y > screenBounds.Height - PSHIPHEIGHT)
                {
                    playerPosition.Y = screenBounds.Height - PSHIPHEIGHT;
                }
            }

            game.thePlayerHealth.Update(gameTime);


            // Update each shot
            foreach (PlayerFire playerShot in thePlayerFire)
            {
                playerShot.Update(gameTime);
            }

            foreach (Rockets rocket in thePlayerRocketFire)
            {
                rocket.Update();
            }

            // Select the correct gun as it may randomly and quickly change
            switch (gunSelector)
            {
                case 1:
                    {
                        initialGun = true;
                        rapidGun = false;
                        spreadGun = false;
                        break;
                    }
                case 2:
                    {
                        rapidGun = true;
                        initialGun = false;
                        spreadGun = false;
                        break;
                    }
                case 3:
                    {
                        spreadGun = true;
                        initialGun = false;
                        rapidGun = false;
                        break;
                    }
            }

            // See if the player's shots are off the screen, if they are remove them
            for (int i = thePlayerFire.Count - 1; i >= 0; i--)
            {
                PlayerFire shot = thePlayerFire[i];
                shot.Update(gameTime);
                thePlayerFire[i] = shot; // you need this is Shot is a value type 

                if ((shot.firePosition.Y >= Game.Window.ClientBounds.Height + 5) ||
                    (shot.firePosition.X >= Game.Window.ClientBounds.Width + 5) ||
                    (shot.firePosition.Y <= screenBounds.Top + 64) || (shot.firePosition.X <= screenBounds.Left + 5))
                {
                    thePlayerFire.Remove(shot);
                }
            }

            // See if the player's shots are off the screen, if they are remove them
            for (int i = thePlayerRocketFire.Count - 1; i >= 0; i--)
            {
                //if (!thePlayerRocketFire[i].Visible)
                //    thePlayerRocketFire.Remove(thePlayerRocketFire[i]);
            }

            // check and see if we should stop the flash after a nuke
            if (flash == true)
            {
                if (flashTimer >= flashTime)
                {
                    flash = false;
                    flashTimer = 0;
                }
                else
                {
                    flashTimer += gameTime.ElapsedGameTime.Milliseconds;
                }
            }

            // Check the collision logics
            DoCollisionLogic(gameTime);
            DoPowerCollisionLogic(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the ship
            int percent = (int)((float)255 - ((float)(heat / (float)maxHeat) * (float)255));
            Color playerColor = new Color(255, percent, percent); //Draw red if overheated
            spriteBatch.Draw(playerShipTexture, playerPosition, playerRectangle, playerColor);

            foreach (PlayerFire playerShot in thePlayerFire)
            {
                playerShot.Draw(spriteBatch); // Draw the shot
            }

            foreach (Rockets rocket in thePlayerRocketFire)
            {
                rocket.Draw(spriteBatch); // Draw the shot
            }

            // draw the flash for the nuke
            if (flash == true)
            {
                spriteBatch.Draw(theFlash, new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height), Color.Yellow);
            }
        }

        // This will find out the current location of the player and return it's rectangle around the player
        public Rectangle GetBounds()
        {
            return new Rectangle((int)playerPosition.X, (int)playerPosition.Y,
                PSHIPWIDTH, PSHIPHEIGHT);
        }

        // player will shoot at the enemies and add it into the current list 
        // of shots.
        public void Shoot()
        {
            bool createNew = true;
            bool spreader = false;
            firePosition = new Vector2(game.thePlayer.playerPosition.X + 55, game.thePlayer.playerPosition.Y - 20);
            foreach (PlayerFire playerShot in thePlayerFire)
            {
                if (playerShot.Visible == false)
                {
                    createNew = false;
                    playerShot.Fire(firePosition, spreader);
                    break;
                }
            }
            if (createNew == true && spreadGun == false)
            {
                PlayerFire playerShot = new PlayerFire(game);
                playerShot.LoadContent(fireContentManager);
                playerShot.Fire(firePosition, spreader);
                thePlayerFire.Add(playerShot);
            }
        }

        // player will shoot at the enemies and add it into the current list 
        // of shots. (shoots 3 shots instead of one)
        public void SpreadShoot()
        {
            bool spreader = true;
            bool createNew = true;
            firePosition = new Vector2(game.thePlayer.playerPosition.X + 55, game.thePlayer.playerPosition.Y - 20);
            foreach (PlayerFire playerShot in thePlayerFire)
            {
                if (playerShot.Visible == false)
                {
                    createNew = false;
                    playerShot.Fire(firePosition, spreader);
                    break;
                }
            }
            if (createNew == true && spreadGun == true)
            {
                PlayerFire playerShot = new PlayerFire(game);
                playerShot.LoadContent(fireContentManager);
                playerShot.Fire(firePosition, spreader);
                thePlayerFire.Add(playerShot);
                firePosition = new Vector2(game.thePlayer.playerPosition.X + 118, game.thePlayer.playerPosition.Y - 20);
                PlayerFire playerShot2 = new PlayerFire(game);
                playerShot2.LoadContent(fireContentManager);
                playerShot2.Fire(firePosition, spreader);
                thePlayerFire.Add(playerShot2);
                firePosition = new Vector2(game.thePlayer.playerPosition.X + -5, game.thePlayer.playerPosition.Y - 20);
                PlayerFire playerShot3 = new PlayerFire(game);
                playerShot3.LoadContent(fireContentManager);
                playerShot3.Fire(firePosition, spreader);
                thePlayerFire.Add(playerShot3);
            }
        }

        // will check for the enemy rectangle and see if shot's position collides with said rectangle.
        // If it colides the enemy will loose health and the shot will be removed from the list of shots.
        private void DoCollisionLogic(GameTime gameTime)
        {
            bool hasCollision = false;
            foreach (Enemy theEnemy in game.theEnemy)
            {
                Rectangle enemyRectangle = theEnemy.GetBounds();
                for (int i = thePlayerFire.Count - 1; i >= 0; i--)
                {
                    PlayerFire shot = thePlayerFire[i];
                    shot.Update(gameTime);
                    thePlayerFire[i] = shot; // you need this is Shot is a value type  
                    hasCollision = shot.CheckCollision(enemyRectangle);
                    if (hasCollision)
                    {
                        theEnemy.checkAttack = true;
                        thePlayerFire.Remove(shot);
                    }
                }
            }

            foreach (Enemy theEnemy in game.theEnemy)
            {
                Rectangle enemyRectangle = theEnemy.GetBounds();
                for (int i = thePlayerRocketFire.Count - 1; i >= 0; i--)
                {
                    Rockets rocket = thePlayerRocketFire[i];
                    thePlayerRocketFire[i] = rocket; // you need this is Shot is a value type  
                    hasCollision = rocket.CheckCollision(enemyRectangle);
                    if (hasCollision)
                    {
                        theEnemy.checkAttack = true;
                        thePlayerRocketFire.Remove(rocket);
                    }
                }
            }
            if (game.Boss3.Visible == true && game.Boss3.bossFadein == false
                && game.Boss3.bossFadeOut == false && game.Boss3.bossFaded == false)
            {

                Rectangle boss1Rectangle = game.Boss3.GetBounds();
                for (int i = thePlayerFire.Count - 1; i >= 0; i--)
                {
                    PlayerFire shot = thePlayerFire[i];
                    shot.Update(gameTime);
                    thePlayerFire[i] = shot; // you need this is Shot is a value type  
                    hasCollision = shot.CheckCollision(boss1Rectangle);
                    if (hasCollision)
                    {
                        game.Boss3.checkAttack = true;
                        thePlayerFire.Remove(shot);
                    }
                }
            }
            if (game.Boss3.Visible == true && game.Boss3.bossFaded == true)
            {
                foreach (BossEnemy theEnemy in game.Boss3.bossEnemies)
                {
                    Rectangle enemyRectangle = theEnemy.GetBounds();
                    for (int i = thePlayerFire.Count - 1; i >= 0; i--)
                    {
                        PlayerFire shot = thePlayerFire[i];
                        shot.Update(gameTime);
                        thePlayerFire[i] = shot; // you need this is Shot is a value type  
                        hasCollision = shot.CheckCollision(enemyRectangle);
                        if (hasCollision)
                        {
                            theEnemy.checkAttack = true;
                            thePlayerFire.Remove(shot);
                        }
                    }
                }
            }
            if (game.Boss4.Visible == true && game.Boss4.bossFadein == false
                && game.Boss4.bossFadeOut == false && game.Boss4.bossFaded == false)
            {

                Rectangle boss2HRectangle = game.Boss4.GetHeadBounds();
                for (int i = thePlayerFire.Count - 1; i >= 0; i--)
                {
                    PlayerFire shot = thePlayerFire[i];
                    shot.Update(gameTime);
                    thePlayerFire[i] = shot; // you need this is Shot is a value type  
                    hasCollision = shot.CheckCollision(boss2HRectangle);
                    if (hasCollision)
                    {
                        game.Boss4.checkAttack = true;
                        thePlayerFire.Remove(shot);
                    }
                }
                Rectangle boss2TRectangle = game.Boss4.GetTailBounds();
                for (int i = thePlayerFire.Count - 1; i >= 0; i--)
                {
                    PlayerFire shot = thePlayerFire[i];
                    shot.Update(gameTime);
                    thePlayerFire[i] = shot; // you need this is Shot is a value type  
                    hasCollision = shot.CheckCollision(boss2TRectangle);
                    if (hasCollision)
                    {
                        game.Boss4.checkAttack = true;
                        thePlayerFire.Remove(shot);
                    }
                }
            }
            if (game.Boss4.Visible == true && game.Boss4.bossFaded == true)
            {
                foreach (BossEnemy theEnemy in game.Boss4.bossEnemies)
                {
                    Rectangle enemyRectangle = theEnemy.GetBounds();
                    for (int i = thePlayerFire.Count - 1; i >= 0; i--)
                    {
                        PlayerFire shot = thePlayerFire[i];
                        shot.Update(gameTime);
                        thePlayerFire[i] = shot; // you need this is Shot is a value type  
                        hasCollision = shot.CheckCollision(enemyRectangle);
                        if (hasCollision)
                        {
                            theEnemy.checkAttack = true;
                            thePlayerFire.Remove(shot);
                        }
                    }
                }
            }
        }

        // will check for the power up rectangle and see if players position 
        // collides with said rectangle.  If it colides the player will gain
        // a powerup and the powerup will be removed from the list of powerups.
        private void DoPowerCollisionLogic(GameTime gameTime)
        {
            bool hasCollision = false;
            for (int i = game.thePowerUp.Count - 1; i >= 0; i--)
            {
                PowerUp thePowerUp = game.thePowerUp[i];
                Rectangle playerRectangle = GetBounds();
                game.thePowerUp[i] = thePowerUp; // you need this is PowerUp is a value type  
                hasCollision = thePowerUp.CheckCollision(playerRectangle);
                if (hasCollision)
                {
                    if (thePowerUp.powerUpType == 1)
                    {
                        game.thePlayerHealth.Heal();
                        thePowerUp.Visible = false;
                        game.powerCount--;
                        game.thePowerUp.Remove(game.thePowerUp[i]);
                        game.theScore.score += 10;
                    }
                    else if (thePowerUp.powerUpType == 2)
                    {
                        game.thePlayer.gunSelector = 2;
                        thePowerUp.Visible = false;
                        game.powerCount--;
                        game.thePowerUp.Remove(game.thePowerUp[i]);
                        game.theScore.score += 10;
                    }
                    else if (thePowerUp.powerUpType == 3)
                    {
                        game.thePlayer.gunSelector = 3;
                        thePowerUp.Visible = false;
                        game.powerCount--;
                        game.thePowerUp.Remove(game.thePowerUp[i]);
                        game.theScore.score += 10;
                    }
                    else if (thePowerUp.powerUpType == 4)
                    {
                        game.thePlayer.missiles = true;
                        thePowerUp.Visible = false;
                        game.powerCount--;
                        game.thePowerUp.Remove(game.thePowerUp[i]);
                        game.theScore.score += 10;
                    }
                    else if (thePowerUp.powerUpType == 5)
                    {
                        game.playerLives++;
                        thePowerUp.Visible = false;
                        game.powerCount--;
                        game.thePowerUp.Remove(game.thePowerUp[i]);
                        game.theScore.score += 10;
                    }
                    else if (thePowerUp.powerUpType == 6)
                    {
                        nuke++;
                        thePowerUp.Visible = false;
                        game.powerCount--;
                        game.thePowerUp.Remove(game.thePowerUp[i]);
                        game.theScore.score += 10;
                    }
                    else if (thePowerUp.powerUpType == 0)
                    {
                        game.thePlayerHealth.Heal();
                        thePowerUp.Visible = false;
                        game.powerCount--;
                        game.thePowerUp.Remove(game.thePowerUp[i]);
                        game.theScore.score += 10;
                    }

                    game.audio.soundPlay(game.audio.sfx_powerUp);
                }
                else if (thePowerUp.decayTimer >= thePowerUp.decayTime)
                {
                    game.thePowerUp.Remove(thePowerUp);
                    thePowerUp.decayTimer -= thePowerUp.decayTime;
                    game.powerCount--;
                }
                else
                {
                    thePowerUp.decayTimer += gameTime.ElapsedGameTime.Milliseconds;
                }

            }
        }

        // Uses a nuke ammo and kills all the enemies on the screen
        // (maybe disables the player's guns for 2 seonds and stops movement for 1 from EMP?)
        public void FireNuke()
        {
            flash = true;
            game.thePlayer.nuke--;
            if (flash == true)
            {
                foreach (Enemy enemy in game.theEnemy)
                {
                    enemy.enemyDeath = true;
                }
                foreach (BossEnemy enemy in game.Boss3.bossEnemies)
                {
                    enemy.enemyDeath = true;
                }
                foreach (BossEnemy enemy in game.Boss4.bossEnemies)
                {
                    enemy.enemyDeath = true;
                }
                for (int i = thePlayerRocketFire.Count - 1; i >= 0; i--)
                {
                    Rockets rocket = thePlayerRocketFire[i];
                    thePlayerRocketFire[i] = rocket; // you need this is Shot is a value type 
                    thePlayerRocketFire.Remove(rocket);
                }
            }
        }

        public void RocketShoot()
        {
            game.audio.soundPlay(game.audio.sfx_rocketLaunch, -(float)rnd.NextDouble() + .5f);
            Rockets newRocket = new Rockets(game);
            newRocket.LoadContent();
            thePlayerRocketFire.Add(newRocket);
        }
    }
}