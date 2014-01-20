/*
 * Enemy Class
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

namespace orion
{
    /*
     * This class will create enemies for the play to kill for a score, the enemies
     * values tie into the main game as well as eventually tie into other aspects 
     * such as high score and difficulty
     */
    public class Enemy : GameComponent
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
        protected int directionTimer = 1500; // timer for how ofter direction changes.
        public int shotTimer = 1550; // timer for how ofter enemy fires.
        protected int gameTimer = 0; // counter to see if it's time to fire
        protected Rectangle enemyRectangle; // rectangle around the enemy for collision detection
        protected Random random; // the random number generator
        protected Rectangle screenBounds; // the rectangle around the screen
        public int enemyHealth = 20; // enemies health
        public int damage = 10;
        public bool set = false;


        // width and height of sprite in texture
        private int eShipWidth = 128;
        private int eShipHeight = 128;

        Texture2D enemyShipTexture; // texture of the enemy

        ContentManager fireContentManager;

        // Movement algorithm variables
        public bool leftSideStrike = false;
        private bool leftSideUp = false;
        private bool leftSideDown = true;
        public bool rightSideStrike = false;
        private bool rightSideUp = false;
        private bool rightSideDown = true;
        public bool topDown = false;
        public bool topDownLongLeft = false;
        private bool topDownLongRight = false;
        public bool topDownShortLeft = false;
        private bool topDownShortRight = false;
        public int speed = 0;

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

        public Enemy(Game game)
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
            fireContentManager = theContentManager;
            powerUpContentManager = theContentManager;

            // load the textures based off of a random number value from Game1 class
            enemyShipTexture = theContentManager.Load<Texture2D>(game.enemyLoad);
            powerUpTexture = theContentManager.Load<Texture2D>(game.powerUpLoad);

            foreach (EnemyFire enemyShot in theEnemyFire)
            {
                enemyShot.LoadContent(theContentManager); // load texture for each shot
            }
        }

        public override void Update(GameTime gameTime)
        {

            if (leftSideStrike == false && rightSideStrike == false && topDown == false)
            {
                enemyPosition.Y += speed;
            }
            if (leftSideStrike == true)
            {
                enemyPosition.X += speed;
                if (directionTickCount >= directionTimer)
                {
                    if (leftSideDown == true)
                    {
                        leftSideUp = true;
                        leftSideDown = false;
                        directionTickCount = 0;
                    }
                    else
                    {
                        leftSideDown = true;
                        leftSideUp = false;
                        directionTickCount = 0;
                    }
                }
                else
                {
                    directionTickCount += gameTime.ElapsedGameTime.Milliseconds;
                }

                if (leftSideDown == true)
                {
                    enemyPosition.Y += speed;
                }
                if (leftSideUp == true)
                {
                    enemyPosition.Y -= speed;
                }

            }
            if (rightSideStrike == true)
            {
                enemyPosition.X -= speed;

                if (directionTickCount >= directionTimer)
                {
                    if (rightSideDown == true)
                    {
                        rightSideUp = true;
                        rightSideDown = false;
                        directionTickCount = 0;
                    }
                    else
                    {
                        rightSideDown = true;
                        rightSideUp = false;
                        directionTickCount = 0;
                    }
                }
                else
                {
                    directionTickCount += gameTime.ElapsedGameTime.Milliseconds;
                }

                if (rightSideDown == true)
                {
                    enemyPosition.Y += speed;
                }
                if (rightSideUp == true)
                {
                    enemyPosition.Y -= speed;
                }
            }
            if (topDown == true && (topDownLongLeft == true || topDownLongRight == true))
            {
                enemyPosition.Y += speed;
                if (directionTickCount >= directionTimer)
                {
                    if (topDownLongLeft == true)
                    {
                        topDownLongLeft = false;
                        topDownLongRight = true;
                        directionTickCount = 0;
                    }
                    else
                    {
                        topDownLongRight = false;
                        topDownLongLeft = true;
                        directionTickCount = 0;
                    }
                }
                else
                {
                    directionTickCount += gameTime.ElapsedGameTime.Milliseconds;
                }
            }
            if (topDownLongLeft == true)
            {
                enemyPosition.X -= speed;
            }
            if (topDownLongRight == true)
            {
                enemyPosition.X += speed;
            }

            if (topDown == true && (topDownShortLeft == true || topDownShortRight == true))
            {
                enemyPosition.Y += speed;
                if (directionTickCount >= directionTimer)
                {
                    if (topDownShortLeft == true)
                    {
                        topDownShortLeft = false;
                        topDownShortRight = true;
                        directionTickCount = 0;
                    }
                    else
                    {
                        topDownShortRight = false;
                        topDownShortLeft = true;
                        directionTickCount = 0;
                    }
                }
                else
                {
                    directionTickCount += gameTime.ElapsedGameTime.Milliseconds;
                }
            }
            if (topDownShortLeft == true)
            {
                enemyPosition.X -= speed;
            }
            if (topDownShortRight == true)
            {
                enemyPosition.X += speed;
            }

            // update every shot on the screen currently
            foreach (EnemyFire enemyShot in theEnemyFire)
            {
                enemyShot.Update(gameTime);
            }

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
                    game.addExplosion(new Vectors(enemyPosition.X, enemyPosition.Y));
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
            game.enemytype = 1 + random.Next(3);
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
                                if (game.DropHeal == true)
                                {
                                    game.powerUpLoad = "ShieldPlus";
                                    powerSelector = 1;
                                    game.DropHeal = false;
                                    validChoice = true;
                                    break;
                                }
                                else
                                {

                                    game.powerUpLoad = "ShieldPlus";
                                    validChoice = true;
                                    break;
                                }
                            }
                        case 2: // rapid fire
                            {
                                if (game.DropHeal == true)
                                {
                                    game.powerUpLoad = "ShieldPlus";
                                    powerSelector = 1;
                                    game.DropHeal = false;
                                    validChoice = true;
                                    break;
                                }
                                else
                                {
                                    if (game.thePlayer.initialGun == true && game.thePlayer.gunSelector >= 1
                                        && game.thePlayer.rapidGun == false)
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
                            }
                        case 3: // spread shot
                            {
                                if (game.DropHeal == true)
                                {
                                    game.powerUpLoad = "ShieldPlus";
                                    powerSelector = 1;
                                    game.DropHeal = false;
                                    validChoice = true;
                                    break;
                                }
                                else
                                {
                                    if (game.thePlayer.rapidGun == true && game.thePlayer.gunSelector == 2
                                        && game.thePlayer.gunSelector != 3 && game.thePlayer.spreadGun == false)
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
                            }
                        case 4: // missiles
                            {
                                if (game.DropHeal == true)
                                {
                                    game.powerUpLoad = "ShieldPlus";
                                    powerSelector = 1;
                                    game.DropHeal = false;
                                    validChoice = true;
                                    break;
                                }
                                else
                                {
                                    if (game.thePlayer.spreadGun == true && game.thePlayer.missiles == false)
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
                                            powerSelector = 1 + random.Next(6);
                                            break;
                                        }
                                    }
                                    else if (game.thePlayer.missiles == true)
                                    {
                                        game.powerUpLoad = "ShieldPlus";
                                        powerSelector = 1;
                                        break;
                                    }
                                    else
                                    {
                                        powerSelector = 1 + random.Next(6);
                                        break;
                                    }
                                }
                            }
                        case 5: // Extra Life
                            {
                                if (game.DropHeal == true)
                                {
                                    game.powerUpLoad = "ShieldPlus";
                                    powerSelector = 1;
                                    game.DropHeal = false;
                                    validChoice = true;
                                    break;
                                }
                                else
                                {
                                    if (game.DropLife == true)
                                    {
                                        if (game.playerLives < 8)
                                        {
                                            game.DropLife = false ;
                                            game.powerUpLoad = "ExtraLife";
                                            validChoice = true;
                                            break;
                                        }
                                        else
                                        {
                                            game.powerUpLoad = "ShieldPlus";
                                            powerSelector = 1;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        game.powerUpLoad = "ShieldPlus";
                                        powerSelector = 1;
                                        break;
                                    }
                                }
                            }
                        case 6: // Bomb
                            {
                                if (game.DropHeal == true)
                                {
                                    game.powerUpLoad = "ShieldPlus";
                                    powerSelector = 1;
                                    game.DropHeal = false;
                                    validChoice = true;
                                    break;
                                }
                                else
                                {
                                    if (game.thePlayer.nuke <= 3)
                                    {
                                        game.powerUpLoad = "Nuke";
                                        validChoice = true;
                                        break;
                                    }
                                    else
                                    {
                                        powerSelector = 1 + random.Next(6);
                                        break;
                                    }

                                }
                            }
                    }
                }

                else
                {
                    validChoice = true;
                }
            }

            switch (game.enemytype)
            {
                case 1:
                    {
                        game.enemyLoad = "Enemy";
                        break;
                    }
                case 2:
                    {
                        game.enemyLoad = "Enemy1";
                        break;
                    }
                case 3: // supply ship
                    {
                        game.enemyLoad = "Enemy2";

                        break;
                    }
            }
        }
    }
}