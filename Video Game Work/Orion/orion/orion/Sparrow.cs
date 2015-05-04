
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
    public class Sparrow : GameComponent
    {
        public Vector2 bossPosition = new Vector2(100, 100);
        public Vector2 firePosition = new Vector2(0, 0);
        public List<EnemyFire> theEnemyFire = new List<EnemyFire>();
        public Game1 game;
        public bool checkAttack = false;
        public bool Visible;
        public bool enemyDeath = false;
        protected int randomDirection;
        protected int directionTimer;
        protected int directionTime = 900;
        protected int shotTimer = 1000;//850;
        protected int gameTimer = 0;
        protected int bossTime = 10000;
        protected int bossTimer = 0;
        protected int waitTime = 2000;
        protected int waitTimer = 0;
        protected Rectangle enemyRectangle;
        protected Random random;
        protected Rectangle screenBounds;
        public float health = 1500;
        public float maxHealth = 1500;
        private int speed = 5;
        public bool normalShot = false;
        public bool spawnBossEnemies = false;
        private bool inStartingSpot = false;
        private bool moveLeft, moveRight;
        // width and height of sprite in texture
        private int bShipWidth = 512;
        private int bShipHeight = 255;
        ContentManager fireContentManager;
        Texture2D bossShipTexture;
        public int damage = 10;

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

        //Death sequence variables
        float alpha = 1;
        int deathTimer = 0;
        int deathPause = 300;
        float rValue = 1;

        public Sparrow(Game game)
            : base(game)
        {
            this.game = (Game1)game;
            bossPosition = new Vector2();

            enemyRectangle = new Rectangle(0, 0, bShipWidth, bShipHeight);
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


            bossShipTexture = theContentManager.Load<Texture2D>("Boss1");

            foreach (EnemyFire enemyShot in theEnemyFire)
            {
                enemyShot.LoadContent(theContentManager);
            }

            if (inStartingSpot == false)
            {
                bossPosition.X = screenBounds.Width / 2;
                bossPosition.Y = screenBounds.Top + 64;
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
                if (health <= 0 && deathTimer > deathPause)
                {
                    enemyDeath = true;
                }

                //Death sequence
                 if (health <= 0)
                {
                    alpha -= .01f;
                    deathTimer += 1;
                    if (alpha <= 0)
                        rValue -= .05f;
                    else
                        game.addExplosion(new Vectors(bossPosition.X + random.Next(0, 384), bossPosition.Y + random.Next(0, 128)));
                    return;
                }

                if (bossPosition.X < screenBounds.Left)
                {
                    bossPosition.X = screenBounds.Left;
                }

                if (bossPosition.X > screenBounds.Width - bShipWidth)
                {
                    bossPosition.X = screenBounds.Width - bShipWidth;
                }

                if (bossPosition.Y < screenBounds.Top)
                {
                    bossPosition.Y = screenBounds.Top;
                }

                if (bossPosition.Y > screenBounds.Height - (bShipHeight))
                {
                    bossPosition.Y = screenBounds.Height - (bShipHeight);
                }

                if (directionTimer >= directionTime && spawnBossEnemies == false && bossFadein == false && bossFadeOut == false)
                {
                    randomDirection = 1 + random.Next(2);
                    directionTimer = 0;
                    switch (randomDirection)
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
                    if (spawnBossEnemies == false && bossFadein == false && bossFadeOut == false)
                    {
                        directionTimer += gameTime.ElapsedGameTime.Milliseconds;
                    }
                }
                if (moveLeft == true)
                {
                    bossPosition.X -= speed;
                }
                if (moveRight == true)
                {
                    bossPosition.X += speed;
                }

                if (bossTimer >= bossTime && bossFadein == false && bossFadeOut == false)
                {
                    bossTimer = 0;
                    spawnBossEnemies = true;
                    bossFadeOut = true;
                }
                else
                {
                    if (spawnBossEnemies == false && bossFadein == false && bossFadeOut == false)
                    {
                        bossTimer += gameTime.ElapsedGameTime.Milliseconds;
                    }
                }

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
                        waitTimer += gameTime.ElapsedGameTime.Milliseconds;
                    }

                }

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

                foreach (PowerUp powerUp in game.thePowerUp)
                {
                    powerUp.Update(gameTime);
                }

                if (enemySpawnTimer >= enemySpawnTime && createFirstEnemy == true
                    && enemyCount <= totalEnemyCount)
                {
                    if (spawnBossEnemies == true)
                    {
                        CreateEnemy();
                        enemySpawnTimer = 0;
                    }
                }
                else
                {
                    enemySpawnTimer += gameTime.ElapsedGameTime.Milliseconds;
                }

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


                foreach (EnemyFire enemyShot in theEnemyFire)
                {
                    enemyShot.Update(gameTime);
                }

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

                if (bossFadein == false && bossFaded == false && bossFadeOut == false)
                    DoCollisionLogic(gameTime);

                if (gameTimer >= shotTimer && spawnBossEnemies == false && bossFadein == false)
                {
                    gameTimer = 0;
                    Shoot();
                }

                else
                {
                    gameTimer += gameTime.ElapsedGameTime.Milliseconds;
                }

                if (checkAttack == true)
                {
                    health -= damage;
                    game.audio.soundPlay(game.audio.sfx_hit);
                    checkAttack = false;
                }
            }

            base.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Visible == true)
            {
                

                //DrawDeathsequence
                if (health <= 0 )
                {
                    Color color;
                    byte alphaValue = (byte)(MathHelper.Clamp(alpha, 0, 1) * 255);
                    byte red = (byte)(MathHelper.Clamp(rValue, 0, 1) * 255);
                    color = new Color(red, alphaValue, alphaValue, alphaValue);
                    spriteBatch.Draw(bossShipTexture, bossPosition, color);
                    return;
                }

                if (bossFadein == true)
                {
                    spriteBatch.Draw(bossShipTexture, new Rectangle((int)bossPosition.X, (int)bossPosition.Y, bossShipTexture.Width, bossShipTexture.Height),
                     new Color(color1, color2, color3, (byte)MathHelper.Clamp(bossFadeValue, 0, 255)));
                }

                if (bossFadein == false && bossFadeOut == false && bossFaded == false)
                {
                    spriteBatch.Draw(bossShipTexture, bossPosition, enemyRectangle, Color.White);
                }

                if (bossFadeOut == true)
                {
                    spriteBatch.Draw(bossShipTexture, new Rectangle((int)bossPosition.X, (int)bossPosition.Y, bossShipTexture.Width, bossShipTexture.Height),
                     new Color(color1, color2, color3, (byte)MathHelper.Clamp(bossFadeValue, 0, 255)));
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

        public Rectangle GetBounds()
        {
            return new Rectangle((int)bossPosition.X, (int)bossPosition.Y,
                bShipWidth, bShipHeight);
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
                    EnemyFire enemyShot = new EnemyFire(game);
                    enemyShot.LoadContent(fireContentManager);
                    firePosition = new Vector2(bossPosition.X + 67, bossPosition.Y + 125);
                    enemyShot.Fire(firePosition);
                    enemyShot.directionSelect = 1 + random.Next(2);
                    theEnemyFire.Add(enemyShot);
                    EnemyFire enemyShot2 = new EnemyFire(game);
                    enemyShot2.LoadContent(fireContentManager);
                    firePosition = new Vector2(bossPosition.X + 117, bossPosition.Y + 125);
                    enemyShot2.Fire(firePosition);
                    enemyShot2.directionSelect = 1 + random.Next(2);
                    theEnemyFire.Add(enemyShot2);
                    EnemyFire enemyShot3 = new EnemyFire(game);
                    enemyShot3.LoadContent(fireContentManager);
                    firePosition = new Vector2(bossPosition.X + 377, bossPosition.Y + 125);
                    enemyShot3.Fire(firePosition);
                    enemyShot3.directionSelect = 1 + random.Next(2);
                    theEnemyFire.Add(enemyShot3);
                    EnemyFire enemyShot4 = new EnemyFire(game);
                    enemyShot4.LoadContent(fireContentManager);
                    firePosition = new Vector2(bossPosition.X + 427, bossPosition.Y + 125);
                    enemyShot4.Fire(firePosition);
                    enemyShot4.directionSelect = 1 + random.Next(2);
                    theEnemyFire.Add(enemyShot4);
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
                        //game.thePlayerHealth.theFlash = true;
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