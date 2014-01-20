/*
 * Boss Enemy Class
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
 * This class will be used during boss encounters only.  During these encounters
 * the enemy class actually becomes boss enemies with modifed health.
 */
namespace orion
{
    public class BossEnemy : GameComponent
    {
        public Vector2 enemyPosition = new Vector2(100, 100); // Enemy Position on screen
        public Vector2 firePosition = new Vector2(0, 0); // enemy shot position on screen
        public List<EnemyFire> theEnemyFire = new List<EnemyFire>(); // list of enemy fire
        public Game1 game; // create an instance of Game
        public bool checkAttack = false; // see if enemy has been hit
        public bool Visible; // see if the enemy is vissible
        public bool enemyDeath = false; // see if the enemy is dead
        protected int randomDirection; // random number to pick directon
        protected int directionTickCount; // counter to check if it's time to change direction
        protected int directionTimer = 2500; // timer for how ofter direction changes.
        public int shotTimer = 1550; // timer for how ofter enemy fires.
        protected int gameTimer = 0; // counter to see if it's time to fire
        protected Rectangle enemyRectangle; // rectangle around the enemy for collision detection
        protected Random random; // the random number generator
        protected Rectangle screenBounds; // the rectangle around the screen
        public int enemyHealth = 20; // enemies health
        private int speed = 4; // enemies speed on the screen
        public int damage = 10;
        public bool set = false;

        // width and height of sprite in texture
        private int eShipWidth = 64;
        private int eShipHeight = 64;

        Texture2D enemyShipTexture; // texture of the enemy
        bool left, right, up, down, dl, dr, ur, ul; // direction values to find out which way to go
        ContentManager fireContentManager;

        // powerup variables
        public Vector2 powerUpPosition; // position of the power up on the screen
        public bool powerVisible; // see if the powerup is visible
        public Rectangle powerUpRectangle; // rectangle around the powerup for collision detection
        public int powerSelector; // find out what type of powerup to drop
        public bool dropPowerUp; // see if we drop the powerup
        public int dropChance = 6; // a value to check against to see if we in fact drop it
        public int dropRandom; // random number created to check against drop chance.

        // width and height of sprite in texture
        public int PUWidth = 64;
        public int PUHeight = 64;

        public Texture2D powerUpTexture; // texture of the powerup
        ContentManager powerUpContentManager;

        public BossEnemy(Game game)
            : base(game)
        {
            this.game = (Game1)game; // Set Game to current game;
            enemyPosition = new Vector2(); // reset the vector

            // place rectangles on the screen
            enemyRectangle = new Rectangle(0, 0, eShipWidth, eShipHeight);
            powerUpRectangle = new Rectangle(0, 0, PUWidth, PUHeight);
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

            random = new Random(this.GetHashCode()); // create a random hash to generate random numbers from
        }

        public void LoadContent(ContentManager theContentManager)
        {
            // load the content managers and set them to the current content manager
            theContentManager = game.Content;
            fireContentManager = theContentManager;
            powerUpContentManager = theContentManager;

            // load the textures based off of a random number value from Boss1 class
            enemyShipTexture = theContentManager.Load<Texture2D>(game.Boss3.enemyLoad);
            powerUpTexture = theContentManager.Load<Texture2D>(game.powerUpLoad);

            foreach (EnemyFire enemyShot in theEnemyFire)
            {
                enemyShot.LoadContent(theContentManager); // load texture for each shot
            }

            game.addWarpCloud(new Vectors(enemyPosition.X, enemyPosition.Y));
        }

        public override void Update(GameTime gameTime)
        {
            if (game.gameIsOver == true)
            {
                for (int i = theEnemyFire.Count - 1; i >= 0; i--)
                {
                    EnemyFire shot = theEnemyFire[i];
                    shot.Update(gameTime);
                    theEnemyFire[i] = shot; // you need this is Shot is a value type 
                    theEnemyFire.Remove(shot);
                }
            }

            // see if it's time to pick a new driection on the screen.
            foreach (BossEnemy theEnemy in game.Boss3.bossEnemies)
            {
                if (theEnemy.directionTickCount > theEnemy.directionTimer)
                {
                    theEnemy.directionTickCount = 0;
                    theEnemy.randomDirection = 1 + random.Next(8);
                }
                else
                {
                    theEnemy.directionTickCount += gameTime.ElapsedGameTime.Milliseconds;
                }
            }

            foreach (BossEnemy theEnemy in game.Boss4.bossEnemies)
            {
                if (theEnemy.directionTickCount > theEnemy.directionTimer)
                {
                    theEnemy.directionTickCount = 0;
                    theEnemy.randomDirection = 1 + random.Next(8);
                }
                else
                {
                    theEnemy.directionTickCount += gameTime.ElapsedGameTime.Milliseconds;
                }
            }

            // based on number picked, we make moving that direction true
            switch (randomDirection)
            {
                case 1: //up
                    {
                        up = true;
                        down = false;
                        left = false;
                        right = false;
                        ul = false;
                        ur = false;
                        dl = false;
                        dr = false;
                        break;
                    }
                case 2: // down
                    {
                        up = false;
                        down = true;
                        left = false;
                        right = false;
                        ul = false;
                        ur = false;
                        dl = false;
                        dr = false;
                        break;
                    }
                case 3: // left
                    {
                        up = false;
                        down = false;
                        left = true;
                        right = false;
                        ul = false;
                        ur = false;
                        dl = false;
                        dr = false;
                        break;
                    }
                case 4: // right
                    {
                        up = false;
                        down = false;
                        left = false;
                        right = true;
                        ul = false;
                        ur = false;
                        dl = false;
                        dr = false;
                        break;
                    }
                case 5: // up left
                    {
                        up = false;
                        down = false;
                        left = false;
                        right = false;
                        ul = true;
                        ur = false;
                        dl = false;
                        dr = false;
                        break;
                    }
                case 6: // up right
                    {
                        up = false;
                        down = false;
                        left = false;
                        right = false;
                        ul = false;
                        ur = true;
                        dl = false;
                        dr = false;
                        break;
                    }
                case 7: // down left
                    {
                        up = false;
                        down = false;
                        left = false;
                        right = false;
                        ul = false;
                        ur = false;
                        dl = true;
                        dr = false;
                        break;
                    }
                case 8: // down right
                    {
                        up = false;
                        down = false;
                        left = false;
                        right = false;
                        ul = false;
                        ur = false;
                        dl = false;
                        dr = true;
                        break;
                    }
                default:
                    {
                        up = false;
                        down = false;
                        left = false;
                        right = false;
                        ul = false;
                        ur = false;
                        dl = false;
                        dr = false;
                        break;
                    }
            }


            // Now we move based on the pervious two movement updates.
            if (up == true)
            {
                enemyPosition.Y -= speed;
            }

            if (down == true)
            {
                enemyPosition.Y += speed;
            }

            if (left == true)
            {
                enemyPosition.X -= speed;
            }

            if (right == true)
            {
                enemyPosition.X += speed;
            }

            if (ul == true)
            {
                enemyPosition.Y -= speed;
                enemyPosition.X -= speed;
            }

            if (ur == true)
            {
                enemyPosition.Y -= speed;
                enemyPosition.X += speed;
            }

            if (dl == true)
            {
                enemyPosition.Y += speed;
                enemyPosition.X -= speed;
            }

            if (dr == true)
            {
                enemyPosition.Y += speed;
                enemyPosition.X += speed;
            }

            // keeps enemy off of the bottom where they can't be hit.
            if (enemyPosition.Y >= screenBounds.Height - (eShipHeight + 1))
            {
                randomDirection = 6;
            }

            // keeps the enemy inside the viewport.
            foreach (BossEnemy theEnemy in game.Boss3.bossEnemies)
            {
                if (theEnemy.enemyPosition.X < screenBounds.Left)
                {
                    theEnemy.enemyPosition.X = screenBounds.Left;
                }

                if (theEnemy.enemyPosition.X > screenBounds.Width - theEnemy.eShipWidth)
                {
                    theEnemy.enemyPosition.X = screenBounds.Width - theEnemy.eShipWidth;
                }

                if (theEnemy.enemyPosition.Y < (screenBounds.Top + 64))
                {
                    theEnemy.enemyPosition.Y = screenBounds.Top + 64;
                }

                if (theEnemy.enemyPosition.Y > screenBounds.Height - (theEnemy.eShipHeight))
                {
                    theEnemy.enemyPosition.Y = screenBounds.Height - (theEnemy.eShipHeight);
                }
            }

            foreach (BossEnemy theEnemy in game.Boss4.bossEnemies)
            {
                if (theEnemy.enemyPosition.X < screenBounds.Left)
                {
                    theEnemy.enemyPosition.X = screenBounds.Left;
                }

                if (theEnemy.enemyPosition.X > screenBounds.Width - theEnemy.eShipWidth)
                {
                    theEnemy.enemyPosition.X = screenBounds.Width - theEnemy.eShipWidth;
                }

                if (theEnemy.enemyPosition.Y < (screenBounds.Top + 64))
                {
                    theEnemy.enemyPosition.Y = screenBounds.Top + 64;
                }

                if (theEnemy.enemyPosition.Y > screenBounds.Height - (theEnemy.eShipHeight))
                {
                    theEnemy.enemyPosition.Y = screenBounds.Height - (theEnemy.eShipHeight);
                }
            }
            // update every shot on the screen currently
            foreach (EnemyFire enemyShot in theEnemyFire)
            {
                enemyShot.Update(gameTime);
            }

            // if the shot is outside the screen view, we remove it
            for (int i = theEnemyFire.Count - 1; i >= 0; i--)
            {
                EnemyFire shot = theEnemyFire[i];
                shot.Update(gameTime);
                theEnemyFire[i] = shot; // you need this is Shot is a value type 

                if ((shot.enemyFirePosition.Y >= Game.Window.ClientBounds.Height) ||
                (shot.enemyFirePosition.Y >= Game.Window.ClientBounds.Width) || (shot.enemyFirePosition.Y < 4))
                {
                    theEnemyFire.Remove(shot);
                }
            }

            // see if the shot hit the player
            DoCollisionLogic(gameTime);

            // see if it's time for a new shot to happen
            if (gameTimer >= shotTimer)
            {
                gameTimer = 0; // reset timer
                Shoot(); // call the shoot method
            }

            else
            {
                gameTimer += gameTime.ElapsedGameTime.Milliseconds;
            }

            // see if the player has hit the enemy, if it has deduct life.
            if (checkAttack == true)
            {
                enemyHealth -= damage;
                if (enemyHealth < 0)
                {
                    enemyDeath = true;
                    checkAttack = false;
                }
                else checkAttack = false;
            }
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(enemyShipTexture, enemyPosition, enemyRectangle, Color.White); // draw the enemy

            foreach (EnemyFire enemyShot in theEnemyFire)
            {
                enemyShot.Draw(theSpriteBatch); // draw each shot
            }
        }

        // This will find out the current location of the enemy and return it's rectangle around the enemy
        public Rectangle GetBounds()
        {
            return new Rectangle((int)enemyPosition.X, (int)enemyPosition.Y,
                eShipWidth, eShipHeight);
        }

        // Enemy will shoot at the player and add it into the current list of shots.
        public void Shoot()
        {
            bool createNew = true;
            firePosition = new Vector2(enemyPosition.X + 55, enemyPosition.Y + 125);

            foreach (EnemyFire enemyShot in theEnemyFire)
            {
                if (enemyShot.Visible == false)
                {
                    createNew = false;
                    break;
                }
            }

            if (createNew == true)
            {
                EnemyFire enemyShot = new EnemyFire(game);
                enemyShot.LoadContent(fireContentManager);
                enemyShot.Fire(firePosition);
                theEnemyFire.Add(enemyShot);
            }
        }

        // will check for the players rectangle and see if shot's position collides with said rectangle.
        // If it colides the player will loose health and the shot will be removed from the list of shots.
        private void DoCollisionLogic(GameTime gameTime)
        {
            bool hasCollision = false;
            Rectangle playerRectangle = game.thePlayer.GetBounds();

            for (int i = theEnemyFire.Count - 1; i >= 0; i--)
            {
                EnemyFire shot = theEnemyFire[i];
                shot.Update(gameTime);
                theEnemyFire[i] = shot; // you need this is Shot is a value type 
                hasCollision = shot.CheckCollision(playerRectangle);
                if (hasCollision)
                {
                    game.thePlayerHealth.checkAttack = true;
                    game.thePlayerHealth.rumble = true;
                    //game.thePlayerHealth.theFlash = true;
                    theEnemyFire.Remove(shot);
                }
            }
        }

        /*
         * In this function we have the enemy decide via random number if it will drop a powerup
         * if it does drop a power up it will randomly select a powerup it can drop.
         * It will then create the enemy with those parameters and add them to the list,
         * in a random spot inside of the viewing area.
         */
        // we will have them appear in formations in later code.
        public void Create()
        {
            Visible = true;
            bool validChoice = false;
            int dropMissile;
            game.Boss3.enemyType = 1 + random.Next(2);
            powerSelector = 1 + random.Next(6);
            dropRandom = 1 + random.Next(6);

            if (dropRandom > (dropChance - 2)) // we'll drop a random powerup
            {
                dropPowerUp = true;
                powerVisible = true;
            }
            else // we don't drop a powerup
            {
                dropPowerUp = false;
            }

            while (validChoice == false)
            {
                if (dropPowerUp == true)
                {
                    switch (powerSelector)
                    {
                        case 1: // shield power
                            {
                                game.powerUpLoad = "ShieldPlus";
                                validChoice = true;
                                break;
                            }
                        case 2: // rapid fire
                            {
                                if (game.thePlayer.spreadGun == false && game.thePlayer.gunSelector == 1)
                                {
                                    game.powerUpLoad = "RapidPU";
                                    validChoice = true;
                                    break;
                                }
                                else
                                {
                                    powerSelector = 1 + random.Next(6);
                                    break;
                                }
                            }
                        case 3: // spread shot
                            {
                                if (game.thePlayer.rapidGun == true && game.thePlayer.gunSelector == 2)
                                {
                                    game.powerUpLoad = "SpreadShot";
                                    validChoice = true;
                                    break;
                                }
                                else
                                {
                                    powerSelector = 1 + random.Next(6);
                                    break;
                                }
                            }
                        case 4: // missiles
                            {
                                if (game.thePlayer.spreadGun == true)
                                {
                                    dropMissile = 1 + random.Next(7);
                                    if (dropMissile >= 6)
                                    {
                                        game.powerUpLoad = "Rockets";
                                        validChoice = true;
                                        break;
                                    }
                                    else
                                    {
                                        powerSelector = 1;
                                        break;
                                    }
                                }
                                else
                                {
                                    powerSelector = 1 + random.Next(6);
                                    break;
                                }
                            }
                        case 5: // Extra Life
                            {
                                game.powerUpLoad = "ExtraLife";
                                validChoice = true;
                                break;
                            }
                        case 6: // Bomb
                            {
                                if (game.thePlayer.nuke <= 3)
                                {
                                    game.powerUpLoad = "Nuke";
                                    validChoice = true;
                                    break;
                                }
                                else
                                {
                                    powerSelector = 1;
                                    break;
                                }
                            }
                    }
                }
                else
                {
                    validChoice = true;
                }
            }

            switch (game.Boss3.enemyType)
            {
                case 1:
                    {
                        game.Boss3.enemyLoad = "BossEnemy1";
                        break;
                    }
                case 2:
                    {
                        game.Boss3.enemyLoad = "BossEnemy2";
                        break;
                    }
            }
            switch (game.Boss4.enemyType)
            {
                case 1:
                    {
                        game.Boss4.enemyLoad = "BossEnemy1";
                        break;
                    }
                case 2:
                    {
                        game.Boss4.enemyLoad = "BossEnemy2";
                        break;
                    }
            }

            enemyPosition.X = random.Next(Game.Window.ClientBounds.Width - eShipWidth);
            enemyPosition.Y = random.Next(Game.Window.ClientBounds.Height - eShipHeight);
            randomDirection = 1 + random.Next(8);
        }

    }
}