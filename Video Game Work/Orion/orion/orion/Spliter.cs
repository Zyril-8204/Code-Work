
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
    public class Spliter : GameComponent
    {
        public Vector2 bossTailPosition;
        public Vector2 bossHeadPosition;
        public Vector2 bossTailPositionRotated;
        public Vector2 bossHeadPositionRotated;
        public Vector2 bossGun1Position;
        public Vector2 bossGun2Position;

        public Vector2 firePosition = new Vector2(0, 0);
        public List<EnemyFire> theEnemyFire = new List<EnemyFire>();
        public Game1 game;
        public bool checkAttack = false;
        public bool Visible;
        public bool enemyDeath = false;
        protected int randomDirection;
        protected int directionTimer;
        protected int directionTime = 900;
        protected int shotTimer = 900;
        protected int gameTimer = 0;
        protected int bossTime = 7000;
        protected int bossTimer = 0;
        protected int waitTime = 2000;
        protected int waitTimer = 0;
        protected float XDistance, YDistance, rotation;
        protected float XDistance2, YDistance2, rotation2;
        protected Rectangle enemyRectangleHead;
        protected Rectangle enemyRectangleTail;
        protected Rectangle enemyRectangleHeadRotated;
        protected Rectangle enemyRectangleTailRotated;
        protected Random random;
        protected Rectangle screenBounds;
        public float health = 2200;
        public float maxHealth = 2200;
        private int speed = 5;
        private Vector2 gun1Origin, gun2Origin;
        public bool normalShot = false;
        public bool spawnBossEnemies = false;
        private bool inStartingSpot = false;
        private bool moveLeft, moveRight;
        public int damage = 10;
        private float splitDamage = 0;

        // width and height of sprite in texture
        private int bHShipWidth = 512;
        private int bHShipHeight = 256;
        private int bTShipWidth = 256;
        private int bTShipHeight = 256;
        private int bHRShipWidth = 128;
        private int bHRShipHeight = 512;
        private int bTRShipWidth = 256;
        private int bTRShipHeight = 256;
        ContentManager fireContentManager;
        Texture2D bossTailShipTexture;
        Texture2D bossHeadShipTexture;
        Texture2D bossTailShipRotatedTexture;
        Texture2D bossHeadShipRotatedTexture;
        Texture2D bossGun1ShipTexture;
        Texture2D bossGun2ShipTexture;

        // Boss fading stuff
        public bool bossFadein = true;
        public bool bossFadeOut = false;
        public bool bossFaded = false;
        int bossFadeValue = 1;
        int bossFadeIncrement = 3;
        double bossFadeDelay = .035;
        Int32 color1 = 0;
        Int32 color2 = 0;
        Int32 color3 = 0;

        // support enemy stuff

        public List<BossEnemy> bossEnemies = new List<BossEnemy>();
        public string enemyLoad = "BossEnemy1";
        public int enemyType;
        private int enemyCount = 0;
        private int enemyDeathCount = 0;
        private int totalEnemyCount = 25;
        private int enemySpawnTime = 400;
        private int enemySpawnTimer = 0;
        bool createFirstEnemy = false;
        ContentManager enemyContentManager;

        // boss split
        private bool split = false;
        public bool hasSplit = false;
        private double angle = 0;
        private float shipDistance = 0;
        bool inSeparatingPosition = false;
        bool inPosition;
        private Vector2 bossHeadOrigin, bossTailOrigin;
        bool notSplit = true;
        bool sTMoveLeft, sTMoveRight, sHMoveLeft, sHMoveRight, sHMoveUp, sHMoveDown;
        int sTDirection = 0;
        int sHDirection = 0;
        bool tailInPos = false;
        bool angleSpin;
        bool setBools = false;

        //Death sequence variables
        public float alpha = 1;
        public int deathTimer = 0;
        public int deathPause = 300;
        float rValue = 1;

        public Spliter(Game game)
            : base(game)
        {
            this.game = (Game1)game;
            enemyRectangleHead = new Rectangle(0, 0, bHShipWidth, bHShipHeight);
            enemyRectangleTail = new Rectangle(0, 0, bTShipWidth, bTShipHeight);
            enemyRectangleHeadRotated = new Rectangle(0, 0, bHRShipWidth, bHRShipHeight);
            enemyRectangleTailRotated = new Rectangle(0, 0, bTRShipWidth, bTRShipHeight);
            bossHeadPosition = new Vector2();
            bossTailPosition = new Vector2();
            bossGun1Position = new Vector2();
            bossGun2Position = new Vector2();
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

            random = new Random(this.GetHashCode());
        }

        public void LoadContent(ContentManager theContentManager)
        {
            fireContentManager = theContentManager;
            enemyContentManager = theContentManager;


            bossHeadShipTexture = theContentManager.Load<Texture2D>("Boss2Head");
            bossTailShipTexture = theContentManager.Load<Texture2D>("Boss2Tail");
            bossHeadShipRotatedTexture = theContentManager.Load<Texture2D>("Boss2HeadRotated");
            bossTailShipRotatedTexture = theContentManager.Load<Texture2D>("Boss2TailRotated");
            bossGun1ShipTexture = theContentManager.Load<Texture2D>("Boss2Gun");
            bossGun2ShipTexture = theContentManager.Load<Texture2D>("Boss2Gun");
            gun1Origin.X = bossGun1ShipTexture.Width / 2;
            gun1Origin.Y = bossGun1ShipTexture.Height / 2;
            gun2Origin.X = bossGun2ShipTexture.Width / 2;
            gun2Origin.Y = bossGun2ShipTexture.Height / 2;
            bossHeadOrigin.X = bossHeadShipTexture.Width / 2;
            bossHeadOrigin.Y = bossHeadShipTexture.Height / 2;
            bossTailOrigin.X = bossTailShipTexture.Width / 2;
            bossTailOrigin.Y = bossTailShipTexture.Height / 2;

            foreach (EnemyFire enemyShot in theEnemyFire)
            {
                enemyShot.LoadContent(theContentManager);
            }

            if (inStartingSpot == false)
            {
                bossTailPosition.X = screenBounds.Width / 2 - (bTShipWidth / 2);
                bossTailPosition.Y = screenBounds.Top + 64;
                bossHeadPosition.X = bossTailPosition.X + 213;
                bossHeadPosition.Y = bossTailPosition.Y + 65;
                bossGun1Position.X = bossHeadPosition.X + 253;
                bossGun1Position.Y = bossHeadPosition.Y + 16;
                bossGun2Position.X = bossHeadPosition.X + 349;
                bossGun2Position.Y = bossHeadPosition.Y + 18;
                splitDamage = health / 2;
                inStartingSpot = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (game.gameIsOver == true)
            {
                for (int i = bossEnemies.Count - 1; i >= 0; i--)
                {
                    BossEnemy enemy = bossEnemies[i];
                    enemy.Update(gameTime);
                    bossEnemies[i] = enemy; // you need this is Enemy is a value type 
                    bossEnemies.Remove(enemy);
                    enemyCount = 0;
                }
            }
            if (Visible == true)
            {


                //Death sequence
                if (health <= 0)
                {
                    game.thePlayer.won = true;
                    game.thePlayer.playerPosition.Y -= 10;

                    //empty enemy shot and enemy lists
                    theEnemyFire = new List<EnemyFire>();
                    bossEnemies = new List<BossEnemy>();

                    alpha -= .01f;
                    deathTimer += 1;
                    if (alpha <= 0) 
                        rValue -= .05f;
                    else
                    {
                        game.addExplosion(new Vectors(bossTailPositionRotated.X + random.Next(0, 256), bossTailPositionRotated.Y + random.Next(0, 256)));
                        game.addExplosion(new Vectors(bossHeadPositionRotated.X + random.Next(0, 128), bossHeadPositionRotated.Y + random.Next(0, 500)));
                    }
                    return;
                }

                // Keep the ship inside the screen
                // Ships Left Side
                if (bossTailPosition.X < screenBounds.Left && hasSplit == false)
                {
                    bossTailPosition.X = screenBounds.Left;
                    bossHeadPosition.X = bossTailPosition.X + 213;
                    bossGun1Position.X = bossHeadPosition.X + 253;
                    bossGun2Position.X = bossHeadPosition.X + 349;

                }

                // Ships Right side
                if (bossHeadPosition.X > (screenBounds.Width - bHShipWidth) && hasSplit == false)
                {
                    bossHeadPosition.X = screenBounds.Width - bHShipWidth;
                    bossTailPosition.X = bossHeadPosition.X - 213;
                    bossGun1Position.X = bossHeadPosition.X + 253;
                    bossGun2Position.X = bossHeadPosition.X + 349;
                }
                // Ships Top Side
                if (bossHeadPosition.Y < screenBounds.Top && hasSplit == false)
                {
                    bossHeadPosition.Y = screenBounds.Top;
                }

                // Ships Bottom Side
                if (bossHeadPosition.Y > (screenBounds.Height - bHShipHeight) && hasSplit == false)
                {
                    bossHeadPosition.Y = screenBounds.Height - (bHShipHeight);
                }

                // keep ships in screen after split
                // Tail Ship left side
                if (bossTailPositionRotated.X < screenBounds.Left && hasSplit == true)
                {
                    bossTailPositionRotated.X = screenBounds.Left;
                }

                // Tail Ship Right Side
                if (bossTailPositionRotated.X > (screenBounds.Width - bTShipWidth) && hasSplit == true)
                {
                    bossTailPositionRotated.X = screenBounds.Width - bTShipWidth;

                }

                // Head Ship Top Side
                if (bossHeadPositionRotated.Y < (screenBounds.Top + 64) && hasSplit == true)
                {
                    bossHeadPositionRotated.Y = screenBounds.Top + 64;
                    bossGun1Position.X = bossHeadPositionRotated.X + 70;
                    bossGun1Position.Y = bossHeadPositionRotated.Y + 290;
                    bossGun2Position.X = bossHeadPositionRotated.X + 70;
                    bossGun2Position.Y = bossHeadPositionRotated.Y + 390;
                }

                // Head Ship Bottom Side
                if (bossHeadPositionRotated.Y > (screenBounds.Height - bHRShipHeight) && hasSplit == true)
                {
                    bossHeadPositionRotated.Y = screenBounds.Height - (bHRShipHeight);
                    bossGun1Position.X = bossHeadPositionRotated.X + 70;
                    bossGun1Position.Y = bossHeadPositionRotated.Y + 290;
                    bossGun2Position.X = bossHeadPositionRotated.X + 70;
                    bossGun2Position.Y = bossHeadPositionRotated.Y + 390;
                }

                // Head Ship Left Side                
                if (bossHeadPositionRotated.X < screenBounds.Left && hasSplit == true)
                {
                    bossHeadPositionRotated.X = screenBounds.Left;
                    bossGun1Position.X = bossHeadPositionRotated.X + 70;
                    bossGun1Position.Y = bossHeadPositionRotated.Y + 290;
                    bossGun2Position.X = bossHeadPositionRotated.X + 70;
                    bossGun2Position.Y = bossHeadPositionRotated.Y + 390;
                }

                // Head Ship Right Side
                if (bossHeadPositionRotated.X > (screenBounds.Width - bHRShipWidth) && hasSplit == true)
                {
                    bossHeadPositionRotated.X = screenBounds.Width - bHRShipWidth;
                    bossGun1Position.X = bossHeadPositionRotated.X + 70;
                    bossGun1Position.Y = bossHeadPositionRotated.Y + 290;
                    bossGun2Position.X = bossHeadPositionRotated.X + 70;
                    bossGun2Position.Y = bossHeadPositionRotated.Y + 390;

                }

                // Fixes ship getting stuck

                // bottom
                if (bossHeadPositionRotated.Y >= (screenBounds.Height - (bHRShipHeight + 8)))
                {
                    sHMoveRight = false;
                    sHMoveLeft = false;
                    sHMoveDown = false;
                    sHMoveUp = true;
                    if (sHDirection == 4)
                    {
                        sHDirection = 3;
                    }
                }

                //top 
                if (bossHeadPositionRotated.Y <= (screenBounds.Top + 69))
                {
                    sHMoveRight = false;
                    sHMoveLeft = false;
                    sHMoveDown = true;
                    sHMoveUp = false;
                    if (sHDirection == 3)
                    {
                        sHDirection = 4;
                    }
                }

                // left
                if (bossHeadPositionRotated.X <= (screenBounds.Left + (bHRShipWidth + 5)))
                {
                    sHMoveRight = true;
                    sHMoveLeft = false;
                    sHMoveDown = false;
                    sHMoveUp = false;
                    if (sHDirection == 1)
                    {
                        sHDirection = 2;
                    }
                }

                // Right
                if (bossHeadPositionRotated.X >= (screenBounds.Width - (bHRShipWidth + 5)))
                {
                    sHMoveRight = false;
                    sHMoveLeft = true;
                    sHMoveDown = false;
                    sHMoveUp = false;
                    if (sHDirection == 2)
                    {
                        sHDirection = 1;
                    }
                }

                // see if it's time to change the ships direction
                if (directionTimer >= directionTime && spawnBossEnemies == false && bossFadein == false
                    && bossFadeOut == false && hasSplit == false && split == false)
                {
                    randomDirection = 1 + random.Next(2); // find a new direction
                    directionTimer = 0; // reset our timer
                    switch (randomDirection) // find out which way we go
                    {
                        case 1: // move left
                            {
                                moveLeft = true;
                                moveRight = false;
                                break;
                            }
                        case 2: // move right
                            {
                                moveRight = true;
                                moveLeft = false;
                                break;
                            }
                    }
                }
                else
                {
                    if (spawnBossEnemies == false && bossFadein == false && bossFadeOut == false && hasSplit == false && split == false)
                    {
                        directionTimer += gameTime.ElapsedGameTime.Milliseconds; // add time to our timer
                    }
                }

                // After Split
                if (directionTimer >= directionTime && hasSplit == true)
                {
                    sTDirection = 1 + random.Next(2); // find the tail ship direction
                    sHDirection = 1 + random.Next(4); // find the head ship direction
                    directionTimer = 0; // reset the timer
                    switch (sTDirection) // find out which way the tail goes
                    {
                        case 1: // move left
                            {
                                sTMoveLeft = true;
                                sTMoveRight = false;
                                break;
                            }
                        case 2: // move right
                            {
                                sTMoveRight = true;
                                sTMoveLeft = false;
                                break;
                            }
                    }
                    switch (sHDirection) // find out which way the head goes
                    {
                        case 1: // move left
                            {
                                sHMoveLeft = true;
                                sHMoveRight = false;
                                sHMoveDown = false;
                                sHMoveUp = false;
                                break;
                            }
                        case 2: // move right
                            {
                                sHMoveLeft = false;
                                sHMoveRight = true;
                                sHMoveDown = false;
                                sHMoveUp = false;
                                break;
                            }
                        case 3: // move up
                            {

                                sHMoveLeft = false;
                                sHMoveRight = false;
                                sHMoveDown = false;
                                sHMoveUp = true;
                                break;
                            }
                        case 4: // move down
                            {
                                sHMoveLeft = false;
                                sHMoveRight = false;
                                sHMoveDown = true;
                                sHMoveUp = false;
                                break;
                            }
                    }
                }
                else if (hasSplit == true && directionTimer <= directionTime)
                {
                    directionTimer += gameTime.ElapsedGameTime.Milliseconds; // add time to the timer
                }

                // Move the ship left
                if (moveLeft == true && hasSplit == false)
                {
                    bossHeadPosition.X -= speed;
                    bossTailPosition.X -= speed;
                    bossGun1Position.X -= speed;
                    bossGun2Position.X -= speed;
                }
                // move the ship right
                if (moveRight == true && hasSplit == false)
                {
                    bossHeadPosition.X += speed;
                    bossTailPosition.X += speed;
                    bossGun1Position.X += speed;
                    bossGun2Position.X += speed;
                }

                // move Tail Left
                if (sTMoveLeft == true && hasSplit == true)
                {
                    bossTailPositionRotated.X -= speed;
                }
                // move tail right
                if (sTMoveRight == true && hasSplit == true)
                {
                    bossTailPositionRotated.X += speed;
                }

                // move Head left
                if (sHMoveLeft == true && hasSplit == true)
                {
                    bossHeadPositionRotated.X -= speed;
                    bossGun1Position.X -= speed;
                    bossGun2Position.X -= speed;
                }
                // move head right
                if (sHMoveRight == true && hasSplit == true)
                {
                    bossHeadPositionRotated.X += speed;
                    bossGun1Position.X += speed;
                    bossGun2Position.X += speed;
                }
                // move head up
                if (sHMoveUp == true && hasSplit == true)
                {
                    bossHeadPositionRotated.Y -= speed;
                    bossGun1Position.Y -= speed;
                    bossGun2Position.Y -= speed;
                }
                //move head down
                if (sHMoveDown == true && hasSplit == true)
                {
                    bossHeadPositionRotated.Y += speed;
                    bossGun1Position.Y += speed;
                    bossGun2Position.Y += speed;
                }

                // see if it' time to fade and spawn enemies
                if (bossTimer >= bossTime && bossFadein == false && bossFadeOut == false && hasSplit == false && split == false && health >= (health / 2))
                {
                    bossTimer = 0; // reset time
                    spawnBossEnemies = true; // set spawning for enemies
                    bossFadeOut = true; // fade boss out
                }
                else
                {
                    if (spawnBossEnemies == false && bossFadein == false && bossFadeOut == false)
                    {
                        bossTimer += gameTime.ElapsedGameTime.Milliseconds; // add time to timer
                    }
                }

                // Gun 1 Rotation
                //Calculate the distance from the player to the gun X and Y position
                XDistance = game.thePlayer.playerPosition.X - bossGun1Position.X;
                YDistance = game.thePlayer.playerPosition.Y - bossGun1Position.Y;

                //Calculate the required rotation by doing a two-variable arc-tan
                rotation = (float)Math.Atan2(YDistance, XDistance);

                // Gun 2 Rotation
                //Calculate the distance from the player to the gun X and Y position
                XDistance2 = game.thePlayer.playerPosition.X - bossGun2Position.X;
                YDistance2 = game.thePlayer.playerPosition.Y - bossGun2Position.Y;

                //Calculate the required rotation by doing a two-variable arc-tan
                rotation2 = (float)Math.Atan2(YDistance2, XDistance2);

                // fade the boss in
                if (bossFadein == true)
                {
                    bossFadeDelay -= gameTime.ElapsedGameTime.TotalSeconds;

                    if (bossFadeDelay <= 0)
                    {
                        bossFadeDelay = .035;

                        bossFadeValue += bossFadeIncrement;
                        color1 += 3;
                        color2 += 3;
                        color3 += 3;

                        if (bossFadeValue >= 255)
                        {
                            bossFadein = false;
                        }

                    }
                }

                // fade the boss out
                if (bossFadeOut == true)
                {
                    bossFadeDelay -= gameTime.ElapsedGameTime.TotalSeconds;

                    if (bossFadeDelay <= 0)
                    {
                        bossFadeDelay = .035;

                        bossFadeValue -= bossFadeIncrement;
                        color1 -= 3;
                        color2 -= 3;
                        color3 -= 3;

                        if (bossFadeValue <= 0)
                        {
                            bossFadeOut = false;
                            bossFaded = true;
                        }

                    }
                }

                // once boss is faded spawn enemies
                if (bossFaded == true && spawnBossEnemies == true)
                {
                    if (waitTimer >= waitTime)
                    {
                        waitTimer = 0;
                        if (createFirstEnemy == false)
                        {
                            CreateEnemy();
                            createFirstEnemy = true;
                        }
                    }
                    else
                    {
                        waitTimer += gameTime.ElapsedGameTime.Milliseconds; // add time to the timer
                    }

                }

                // check what the enemies are doing and keep track of them
                for (int i = bossEnemies.Count - 1; i >= 0; i--)
                {
                    BossEnemy enemy = bossEnemies[i];
                    enemy.Update(gameTime);
                    bossEnemies[i] = enemy; // you need this is Enemy is a value type 
                    if (enemy.enemyDeath == true)
                    {
                        newPowerUpTime(gameTime);
                        bossEnemies.Remove(enemy);
                        enemyDeathCount++;
                        game.theScore.score += 100;
                    }
                }

                // update all the powerups
                foreach (PowerUp powerUp in game.thePowerUp)
                {
                    powerUp.Update(gameTime);
                }

                // keep spawning enemies until we hit max on timer
                if (enemySpawnTimer >= enemySpawnTime && createFirstEnemy == true
                    && enemyCount <= totalEnemyCount)
                {
                    if (spawnBossEnemies == true)
                    {
                        CreateEnemy();
                        enemySpawnTimer = 0; // reset timer
                    }
                }
                else
                {
                    enemySpawnTimer += gameTime.ElapsedGameTime.Milliseconds; // add time to timer
                }

                // see if we killed all the enemies and fade boss back in
                if (enemyDeathCount == totalEnemyCount + 1)
                {
                    bossFaded = false;
                    bossFadein = true;
                    spawnBossEnemies = false;
                    enemyDeathCount = 0;
                    enemySpawnTimer = 0;
                    waitTimer = 0;
                    bossTimer = 0;
                    enemyCount = 0;
                    createFirstEnemy = false;
                }

                // update each shot
                foreach (EnemyFire enemyShot in theEnemyFire)
                {
                    enemyShot.Update(gameTime);
                }

                // remove shots once out of screen
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

                // if the boss is there, make sure we can hit boss
                if (bossFadein == false && bossFaded == false && bossFadeOut == false && (split == false
                    || hasSplit == true || notSplit == true))
                    DoCollisionLogic(gameTime);

                // if time, boss shoots at player
                if (gameTimer >= shotTimer && spawnBossEnemies == false && bossFadein == false && (split == false
                    || hasSplit == true || notSplit == true))
                {
                    gameTimer = 0; // reset time
                    Shoot();
                }

                else
                {
                    gameTimer += gameTime.ElapsedGameTime.Milliseconds; // add time
                }

                // if we hit the boss remove health
                if (checkAttack == true && split == false)
                {
                    health -= damage;
                    if (health <= 0)
                    {
                        enemyDeath = true;
                        checkAttack = false;
                    }
                    else checkAttack = false;
                }

                // Split the boss
                if (health <= splitDamage)
                {
                    // time to seperate
                    if (setBools == false)
                    {
                        split = true;
                        notSplit = false;
                        inSeparatingPosition = false;
                        inPosition = false;
                        angleSpin = false;
                        setBools = true;
                        moveLeft = false;
                        moveRight = false;
                        bossTailPositionRotated = bossTailPosition;
                        bossHeadPositionRotated = bossHeadPosition;
                        bossHeadPosition = bossHeadPositionRotated;
                        bossTailPosition = bossTailPositionRotated;
                    }


                    if (split == true && hasSplit == false && notSplit == false)
                    {
                        if (inSeparatingPosition == false)
                        {
                            bossTailPosition.X -= 3;
                            bossTailPositionRotated.X -= 3;
                            bossHeadPosition.Y += 2;
                            bossHeadPositionRotated.Y += 2;
                            bossGun1Position.Y += 2;
                            bossGun2Position.Y += 2;



                            if (bossTailPosition.X <= 300)
                            {
                                inSeparatingPosition = true;
                            }
                        }

                        if (inSeparatingPosition == true)
                        {
                            if (inPosition == false)
                            {
                                bossHeadPosition.X += 3;
                                bossHeadPositionRotated.X += 3;
                                bossGun1Position.X += 3;
                                bossGun2Position.X += 3;
                                shipDistance += 3;

                                if (shipDistance >= 120)
                                {
                                    inPosition = true;
                                }
                            }
                        }
                        if (inPosition == true)
                        {
                            if (angleSpin == false)
                            {
                                if (angle < (90 * 0.0174532925))
                                {
                                    angle += (.5 * 0.0174532925);
                                }
                                else
                                {
                                    hasSplit = true;
                                    split = false;
                                    angleSpin = true;
                                }
                            }

                            if (hasSplit == true)
                            {
                                bossGun1Position.X = bossHeadPositionRotated.X + 70;
                                bossGun1Position.Y = bossHeadPositionRotated.Y + 290;
                                bossGun2Position.X = bossHeadPositionRotated.X + 70;
                                bossGun2Position.Y = bossHeadPositionRotated.Y + 390;
                            }
                        }
                    }
                }

                if (tailInPos == false && hasSplit == true)
                {
                    if (bossTailPositionRotated.Y <= 5)
                    {
                        bossTailPositionRotated.Y = screenBounds.Top;
                        tailInPos = true;
                    }
                    else
                    {
                        bossTailPositionRotated.Y -= 3;
                        bossTailPosition.Y -= 3;
                    }
                }
            }

            base.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Visible == true)
            {
                //DrawDeathsequence
                Color color;
                if (health <= 0)
                {
                    byte alphaValue = (byte)(MathHelper.Clamp(alpha, 0, 1) * 255);
                    byte red = (byte)(MathHelper.Clamp(rValue, 0, 1) * 255);
                    color = new Color(red, alphaValue, alphaValue, alphaValue);
                }
                else
                    color = Color.White;

                if (bossFadein == true && hasSplit == false && notSplit == true)
                {
                    spriteBatch.Draw(bossTailShipTexture, new Rectangle((int)bossTailPosition.X, (int)bossTailPosition.Y, bossTailShipTexture.Width, bossTailShipTexture.Height),
                     new Color(color1, color2, color3, (byte)MathHelper.Clamp(bossFadeValue, 0, 255)));
                    spriteBatch.Draw(bossHeadShipTexture, new Rectangle((int)bossHeadPosition.X, (int)bossHeadPosition.Y, bossHeadShipTexture.Width, bossHeadShipTexture.Height),
                     new Color(color1, color2, color3, (byte)MathHelper.Clamp(bossFadeValue, 0, 255)));
                    spriteBatch.Draw(bossGun1ShipTexture, new Rectangle((int)bossGun1Position.X, (int)bossGun1Position.Y, bossGun1ShipTexture.Width, bossGun1ShipTexture.Height),
                     new Color(color1, color2, color3, (byte)MathHelper.Clamp(bossFadeValue, 0, 255)));
                    spriteBatch.Draw(bossGun2ShipTexture, new Rectangle((int)bossGun2Position.X, (int)bossGun2Position.Y, bossGun2ShipTexture.Width, bossGun2ShipTexture.Height),
                     new Color(color1, color2, color3, (byte)MathHelper.Clamp(bossFadeValue, 0, 255)));
                }

                if (bossFadein == false && bossFadeOut == false && bossFaded == false && notSplit == true)
                {
                    spriteBatch.Draw(bossTailShipTexture, bossTailPosition, enemyRectangleTail, Color.White);
                    spriteBatch.Draw(bossHeadShipTexture, bossHeadPosition, enemyRectangleHead, Color.White);
                    spriteBatch.Draw(bossGun1ShipTexture, new Vector2((int)bossHeadPosition.X + 293, (int)bossHeadPosition.Y + 56), null, Color.White, rotation, gun1Origin, 1.0f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(bossGun1ShipTexture, new Vector2((int)bossHeadPosition.X + 393, (int)bossHeadPosition.Y + 60), null, Color.White, rotation2, gun2Origin, 1.0f, SpriteEffects.None, 0f);

                }


                if (bossFadeOut == true && notSplit == true)
                {
                    spriteBatch.Draw(bossTailShipTexture, new Rectangle((int)bossTailPosition.X, (int)bossTailPosition.Y, bossTailShipTexture.Width, bossTailShipTexture.Height),
                     new Color(color1, color2, color3, (byte)MathHelper.Clamp(bossFadeValue, 0, 255)));
                    spriteBatch.Draw(bossHeadShipTexture, new Rectangle((int)bossHeadPosition.X, (int)bossHeadPosition.Y, bossHeadShipTexture.Width, bossHeadShipTexture.Height),
                     new Color(color1, color2, color3, (byte)MathHelper.Clamp(bossFadeValue, 0, 255)));
                    spriteBatch.Draw(bossGun1ShipTexture, new Rectangle((int)bossGun1Position.X, (int)bossGun1Position.Y, bossGun1ShipTexture.Width, bossGun1ShipTexture.Height),
                     new Color(color1, color2, color3, (byte)MathHelper.Clamp(bossFadeValue, 0, 255)));
                    spriteBatch.Draw(bossGun2ShipTexture, new Rectangle((int)bossGun2Position.X, (int)bossGun2Position.Y, bossGun2ShipTexture.Width, bossGun2ShipTexture.Height),
                     new Color(color1, color2, color3, (byte)MathHelper.Clamp(bossFadeValue, 0, 255)));
                }

                if (split == true && hasSplit == false)
                {
                    spriteBatch.Draw(bossTailShipTexture, bossTailPositionRotated, null, color, (float)angle, bossTailOrigin, 1.0f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(bossHeadShipTexture, bossHeadPositionRotated, null, color, (float)angle, bossHeadOrigin, 1.0f, SpriteEffects.None, 0f);
                    //spriteBatch.Draw(bossGun1ShipTexture, new Vector2((int)bossGun1Position.X - 220, (int)bossGun1Position.Y - 22), null, Color.White, rotation, gun1Origin, 1.0f, SpriteEffects.None, 0f);
                    //spriteBatch.Draw(bossGun1ShipTexture, new Vector2((int)bossGun2Position.X - 210, (int)bossGun2Position.Y - 20), null, Color.White, rotation2, gun2Origin, 1.0f, SpriteEffects.None, 0f);
                }
                if (hasSplit == true)
                {
                    spriteBatch.Draw(bossTailShipRotatedTexture, bossTailPositionRotated, color);
                    spriteBatch.Draw(bossHeadShipRotatedTexture, bossHeadPositionRotated, color);
                    spriteBatch.Draw(bossGun1ShipTexture, new Vector2((int)bossGun1Position.X, (int)bossGun1Position.Y), null, color, rotation, gun1Origin, 1.0f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(bossGun1ShipTexture, new Vector2((int)bossGun2Position.X, (int)bossGun2Position.Y), null, color, rotation2, gun2Origin, 1.0f, SpriteEffects.None, 0f);
                }


                foreach (EnemyFire enemyShot in theEnemyFire)
                {
                    enemyShot.Draw(spriteBatch);
                }

                foreach (BossEnemy enemy in bossEnemies)
                {
                    enemy.Draw(spriteBatch);
                }
            }
        }

        public Rectangle GetHeadBounds()
        {
            if (hasSplit == true)
            {
                return new Rectangle((int)bossHeadPositionRotated.X, (int)bossHeadPositionRotated.Y,
                 bHRShipWidth, bHRShipHeight);
            }
            else
            {
                return new Rectangle((int)bossHeadPosition.X, (int)bossHeadPosition.Y,
                    bHShipWidth, bHShipHeight);
            }
        }
        public Rectangle GetTailBounds()
        {
            if (hasSplit == true)
            {
                return new Rectangle((int)bossTailPositionRotated.X, (int)bossTailPositionRotated.Y,
                bTRShipWidth, bTRShipHeight);
            }
            else
            {
                return new Rectangle((int)bossTailPosition.X, (int)bossTailPosition.Y,
                bTShipWidth, bTShipHeight);
            }

        }

        public void Shoot()
        {
            if (Visible == true)
            {
                bool createNew = true;

                foreach (EnemyFire enemyShot in theEnemyFire)
                {
                    if (enemyShot.Visible == false)
                    {
                        createNew = false;
                        //enemyShot.Fire(firePosition);
                        break;
                    }
                }

                if (createNew == true)
                {
                    if (hasSplit == false)
                    {
                        EnemyFire enemyShot = new EnemyFire(game);
                        enemyShot.LoadContent(fireContentManager);
                        firePosition = new Vector2(bossGun1Position.X + 20, bossGun1Position.Y + 40);
                        enemyShot.Fire(firePosition);
                        enemyShot.Cannon1 = true;
                        theEnemyFire.Add(enemyShot);
                        EnemyFire enemyShot2 = new EnemyFire(game);
                        enemyShot2.LoadContent(fireContentManager);
                        firePosition = new Vector2(bossGun2Position.X + 20, bossGun2Position.Y + 40);
                        enemyShot2.Fire(firePosition);
                        enemyShot2.Cannon2 = true;
                        theEnemyFire.Add(enemyShot2);
                    }
                    if (hasSplit == true)
                    {
                        EnemyFire enemyShot = new EnemyFire(game);
                        enemyShot.LoadContent(fireContentManager);
                        firePosition = new Vector2(bossGun1Position.X, bossGun1Position.Y);
                        enemyShot.Fire(firePosition);
                        enemyShot.Cannon1 = true;
                        theEnemyFire.Add(enemyShot);
                        EnemyFire enemyShot2 = new EnemyFire(game);
                        enemyShot2.LoadContent(fireContentManager);
                        firePosition = new Vector2(bossGun2Position.X, bossGun2Position.Y);
                        enemyShot2.Fire(firePosition);
                        enemyShot2.Cannon2 = true;
                        theEnemyFire.Add(enemyShot2);
                        EnemyFire enemyShot3 = new EnemyFire(game);
                        enemyShot3.LoadContent(fireContentManager);
                        firePosition = new Vector2(bossTailPositionRotated.X + 60, bossTailPositionRotated.Y + 230);
                        enemyShot3.Fire(firePosition);
                        theEnemyFire.Add(enemyShot3);
                        EnemyFire enemyShot4 = new EnemyFire(game);
                        enemyShot4.LoadContent(fireContentManager);
                        firePosition = new Vector2(bossTailPositionRotated.X + 190, bossTailPositionRotated.Y + 230);
                        enemyShot4.Fire(firePosition);
                        theEnemyFire.Add(enemyShot4);
                    }
                }
            }
        }
        private void DoCollisionLogic(GameTime gameTime)
        {
            if (Visible == true)
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
                        theEnemyFire.Remove(shot);
                    }
                }
            }
        }
        private void CreateEnemy()
        {
            bool createNew = true;
            foreach (BossEnemy enemy in bossEnemies)
            {
                if (enemy.Visible == false)
                {
                    createNew = false;
                    break;
                }
            }
            if (createNew == true && enemyCount <= totalEnemyCount)
            {
                BossEnemy enemy = new BossEnemy(game);
                enemy.Create();
                enemy.LoadContent(enemyContentManager);
                bossEnemies.Add(enemy);
                enemyCount++;
            }
        }
        public void newPowerUpTime(GameTime gameTime)
        {
            bool createNew = true;
            foreach (PowerUp powerUp in game.thePowerUp)
            {
                if (powerUp.dropPowerUp == false && powerUp.Visible == true)
                {
                    createNew = false;
                    break;
                }
            }
            if (createNew == true && game.powerCount < 4)
            {
                PowerUp thePU = new PowerUp(game);
                thePU.Update(gameTime);
                game.thePowerUp.Add(thePU);
                game.powerCount++;
            }
        }
    }
}