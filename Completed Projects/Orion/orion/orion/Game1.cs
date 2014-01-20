/********************************************
 * Project Orion
 * Created by *Game Company Name*
 * Concept Art: Micah Hawman, Aaron Hoffman
 * Lead Programmer: Micah Hawman
 * Programmer: Aaron Hoffman
 * Lead Artist: Aaron Hoffman
 * Artist: Micah Hawman
 * Lead Sound Engineer:
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

/* Main game Class, will pull in all sub classes, do all the main drawing and 
 * updating of the game.
 */
namespace orion
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // Begin Main Game Variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ContentManager enemyContentManager;
        ContentManager powerUpContentManager;

        //game background
        Texture2D background;
        Rectangle backgroundRect;
        Texture2D flash; //Texture  of the flash
        Texture2D pixel;
        SpriteFont font;

        // Create classes in the game
        public PlayerHealth thePlayerHealth;
        public Player thePlayer;
        public PlayerInput playerInput;
        public List<Enemy> theEnemy = new List<Enemy>();
        public Score theScore;
        public List<PowerUp> thePowerUp = new List<PowerUp>();
        public Macer Boss1;
        public GassBoss Boss2;
        public Sparrow Boss3;
        public Spliter Boss4;

        public MainMenu mainMenu;
        public PauseMenu pauseMenu;
        public HelpMenu helpMenu;
        public GameOver gameOver;
        public OptionMenu optionsMenu;
        public HighScore highScore;
        public LevelComplete levelComplete;

        // Boolean Variables
        public bool gameMenu = true; // will show the gameMenu when built.
        public bool gameHelp = false; // will show the game Help when called from MainMenu.
        public bool gameOptions = false; // will show the game Options when called from Main Menu.
        public bool gameHighScores = false; // will show the high scores on the game.
        public bool gamePlay = false; // starts game play when startgame is called from menu
        public bool gamePaused = false; // Check to see if the game is paused.. if it is don't update
        public bool gameIsOver = false; // checks for game over
        public bool gameStart = true; // make sure we start the game when time to.
        public bool gameScore = false; // show the score shen called.
        public bool playerDeath = false; // checks for player death when playing game
        public bool boss1Active, boss2Active, boss3Active, boss4Active; // checks if it's time to begin a boss battle
        public bool boss1Defeated = false; // lets us know if boss1 has been defeated.
        public bool boss2Defeated = false; // lets us know if boss2 has been defeated.
        public bool boss3Defeated = false; // lets us know if boss3 has been defeated.
        public bool boss4Defeated = false; // lets us know if boss4 has been defeated.
        public bool unflash = false; // Fixes an issue with screen flash when hit.
        public bool flashScreen = false;
        public bool hasFlashed = false;
        public bool restartTheGame = false;
        public bool nextLevel = false;
        private bool changeLevel = false;
        private bool level1, level2, level3, level4;

        public bool DropLife = false;
        private int lifeDropTimer = 0;
        private int lifeDropTime = 22000;
        public bool DropHeal = false;
        private int healDropTimer = 0;
        private int healDropTime = 15000;

        // int Variables
        public int powerCount = 0; // keeps track of how many powerups are on the screen
        private int enemyCount = 0; // keeps track of how many enemies are currently on the screen
        private int trueEnemyCount = 0; // keeps track of how many enemies have been created
        private const int ADDENEMYTIME = 1000; // Timer of how often a enemy is added
        public int playerLives = 2; // Keeps track of how many lives player has, 2 by default
        private int enemyTimer = 0; // timer to see if it's time for a new enemy
        public int enemytype = 0; // random to decide what type of enemy ship texture to load.
        protected Rectangle screenBounds;
        protected Random random; // the random number generator
        public int level = 1; // keep track of what level we are on.
        private int flashTimer = 0; // a counter for flash time
        private int flashTime = 50; // the timer for flash effect
        private int restartTimer = 0;
        private int restartTime = 500;
        private int nextLevelTimer = 0;
        private int nextLevelTime = 2500;
        public Audio audio = new Audio();
        protected Random random2; // used to find a second type of random
        protected int movementMod = 0;
        protected int modTimer = 0;
        protected int modTime = 20000;
        public float volume = .75f;    //Ranges from 0 to 1


        // Default Strings
        public string enemyLoad = "Enemy"; // default string for which enemy texture to use
        public string powerUpLoad = "ShieldPlus"; // default string for which powerup texture to use

        Levels levels;
        ToolBar bar;
        bool set = false; // sets different variables based on difficulty.
        int nukeTest = 0; //For testing when a nuke is fired; for clearing all obsticles in the levels instance

        // debugging section
        private bool debug = false; // turn this off for release!!
        private bool invincible = false; // makes player invincible

        //Ending sequence variables
        float flyingSpeed = 0;
        //int finalFader = 0;
        string[] credits;
        int endStage = 3;
        public bool gameWon = false;
        bool showCredit = false;
        int CreditPause = 500;
        int creditTime = 0;
        Vector2 endPlayerPos = new Vector2(0, 0);

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Ending sequence variables
            credits = new string[4];
            credits[0] = "Thank you for playing!";
            credits[1] = "Music Created by:\nMatthew A McFarland\nmattmcfarland.com";
            credits[2] = "Art     Programming     Game Design     Sound Design\nAaron Hoffman";
            credits[3] = "Programing     Game Design     Ship Design\nMicah Hawman";


            // Set the screen size
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.ApplyChanges();

            // create the new instance of the classes
            thePlayerHealth = new PlayerHealth(this);
            thePlayer = new Player(this);
            playerInput = new PlayerInput(this);
            theScore = new Score(this);
            Boss3 = new Sparrow(this);
            Boss4 = new Spliter(this);
            mainMenu = new MainMenu(this);
            pauseMenu = new PauseMenu(this);
            helpMenu = new HelpMenu(this);
            gameOver = new GameOver(this);
            optionsMenu = new OptionMenu(this);
            highScore = new HighScore(this);
            levelComplete = new LevelComplete(this);

            // add a new enemy for each new enemy and create the instance of them.
            foreach (Enemy enemy in theEnemy)
            {
                Components.Add(new Enemy(this));
            }

            //Creates Level structure
            
            bar = new ToolBar(this);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            //load sound
            audio.loadAudio(this);
            levels = new Levels(this);
            levels.playSong();

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create additional content managers for class creation later
            enemyContentManager = this.Content;
            powerUpContentManager = this.Content;

            // Load the class content
            thePlayer.LoadContent(this.Content, "Player");
            thePlayerHealth.LoadContent(this.Content, "HealthBar");
            theScore.LoadContent(this.Content);
            Boss1 = new Macer(this);
            Boss2 = new GassBoss(this);
            Boss3.LoadContent(this.Content);
            Boss4.LoadContent(this.Content);
            mainMenu.LoadContent(this.Content);
            pauseMenu.LoadContent(this.Content);
            helpMenu.LoadContent(this.Content);
            gameOver.LoadContent(this.Content);
            highScore.LoadContent(this.Content);
            levelComplete.LoadContent(this.Content);

            // Load content for each enemy created
            foreach (Enemy enemy in theEnemy)
            {
                enemy.LoadContent(this.Content);
            }
            // load the background

            background = this.Content.Load<Texture2D>("title");
            flash = this.Content.Load<Texture2D>("Flash");
            pixel = Content.Load<Texture2D>("pixel");
            font = Content.Load<SpriteFont>("MainMenuFont");

            backgroundRect = new Rectangle(0, 0,
                                           graphics.GraphicsDevice.Viewport.Width,
                                           graphics.GraphicsDevice.Viewport.Height);
            screenBounds = new Rectangle(0, 0,
               Window.ClientBounds.Width,
               Window.ClientBounds.Height);
            random = new Random(this.GetHashCode()); // create a random hash to generate random numbers from
            random2 = new Random(this.GetHashCode()); // create a random hash to generate random numbers from



        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (gameWon)
            {
                theEndUpdate();
                return;
            }



            // Check to see if the player is moving via input class
            playerInput.Update(gameTime);
            audio.update();

            if (nukeTest > thePlayer.nuke)
            {
                audio.soundPlay(audio.sfx_nuke);
                levels.clearLevel();

            }
            nukeTest = thePlayer.nuke;

            if (gameMenu == true)
            {
                mainMenu.Update(gameTime);
                optionsMenu.selector = 0;
                helpMenu.cursorSelector = 0;
                pauseMenu.cursorSelector = 0;
                
                if (restartTheGame == true)
                {
                    if (restartTimer >= restartTime)
                    {
                        restartTheGame = false;
                        restartTimer = 0;
                    }
                    else
                    {
                        restartTimer += gameTime.ElapsedGameTime.Milliseconds;
                    }
                }
            }
            if (gameOptions == true)
            {
                optionsMenu.Update();
            }
            if (gameHelp == true)
            {
                helpMenu.Update(gameTime);
            }

            if (gamePaused == true)
            {
                gamePaused = true;
                gamePlay = false;
                pauseMenu.menuStart = true;
                pauseMenu.Update(gameTime);
            }
            if (gameScore == true)
            {
                highScore.Update(gameTime);
            }
            if (gameOver.gameOver == true)
            {
                gameOver.Update(gameTime);
            }

            //if we continue the game, start the level over.
            if (gameOver.continueGame == true)
            {
                switch (level)
                {
                    case 1:
                        {
                            levels.changeLevel(0);
                            levels.update(this);
                            if (boss1Active == true)
                            {
                                boss1Active = false;
                                Boss1.health = Boss1.maxHealth;
                            }
                            break;
                        }
                    case 2:
                        {
                            levels.clearLevel();
                            levels.changeLevel(1);
                            levels.update(this);
                            if (boss2Active == true)
                            {
                                boss2Active = false;
                                Boss2.health = Boss2.maxHealth;
                            }
                            break;
                        }
                    case 3:
                        {
                            levels.clearLevel();
                            levels.changeLevel(2);
                            levels.update(this);
                            if (boss3Active == true)
                            {
                                boss3Active = false;
                                Boss3.health = Boss3.maxHealth;
                            }
                            break;
                        }
                    case 4:
                        {
                            levels.clearLevel();
                            levels.changeLevel(3);
                            levels.update(this);
                            if (boss4Active == true)
                            {
                                boss4Active = false;
                                Boss4.health = Boss4.maxHealth;
                            }
                            break;
                        }
                }
                if (theScore.score <= 10000)
                {
                    theScore.score = 0;
                }
                else
                {
                    theScore.score -= 10000;
                }

                playerLives = 2;
                playerDeath = false;
                gameStart = true;
                thePlayer.gunSelector = 1;
                playerInput.shotCounter = 0;
                gameOver.gameOver = false;
                gameIsOver = false;
                gamePlay = true;
                foreach (PowerUp powerup in thePowerUp)
                {
                    powerup.decayTimer = 5000;
                }
                thePlayer.missiles = false;
                levels.update(this);
                if (gameOver.continueGame == true)
                {
                    thePlayerHealth.theCurrentHealth += 110;
                    gameOver.continueGame = false;
                }
            }
            if (gamePlay == true)
            {
                // check what difficulty we are on and adjust game settings accordingly.
                if (optionsMenu.difficulty == 0) //Easy
                {
                    thePlayerHealth.damage = 5;
                    foreach (BossEnemy enemy in Boss3.bossEnemies)
                    {
                        enemy.damage = 20;
                        enemy.shotTimer = 1900;
                        if (enemy.set == false)
                        {
                            enemy.enemyHealth = 20;
                            enemy.set = true;
                            enemy.damage = 20;
                        }
                    }
                    foreach (BossEnemy enemy in Boss4.bossEnemies)
                    {

                        enemy.shotTimer = 1900;
                        if (enemy.set == false)
                        {
                            enemy.enemyHealth = 20;
                            enemy.set = true;
                            enemy.damage = 20;
                        }
                    }
                    foreach (Enemy enemy in theEnemy)
                    {
                        enemy.shotTimer = 1900;
                        if (enemy.set == false)
                        {
                            enemy.enemyHealth = 20;
                            enemy.set = true;
                            enemy.damage = 20;
                        }
                    }
                    if (set == false)
                    {
                        playerLives = 3;
                        thePlayer.nuke = 3;
                        Boss1.maxHealth = 95;
                        Boss2.maxHealth = 190;
                        Boss3.maxHealth = 760;
                        Boss3.damage = 20;
                        Boss4.maxHealth = 1425;
                        Boss4.damage = 20;
                        Boss1.health = Boss1.maxHealth;
                        Boss2.health = Boss2.maxHealth;
                        Boss3.health = Boss3.maxHealth;
                        Boss4.health = Boss4.maxHealth;

                        set = true;
                    }
                }
                else if (optionsMenu.difficulty == 1)// normal
                {
                    thePlayerHealth.damage = 10;

                    foreach (BossEnemy enemy in Boss3.bossEnemies)
                    {
                        enemy.damage = 15;
                        enemy.shotTimer = 1550;
                    }
                    foreach (BossEnemy enemy in Boss4.bossEnemies)
                    {
                        enemy.damage = 15;
                        enemy.shotTimer = 1550;

                    }
                    if (set == false)
                    {
                        playerLives = 2;
                        thePlayer.nuke = 2;
                        Boss1.maxHealth = 250;
                        Boss2.maxHealth = 300;
                        Boss3.maxHealth = 1600;
                        Boss3.damage = 10;
                        Boss4.maxHealth = 2300;
                        Boss4.damage = 10;
                        Boss1.health = Boss1.maxHealth;
                        Boss2.health = Boss2.maxHealth;
                        Boss3.health = Boss3.maxHealth;
                        Boss4.health = Boss4.maxHealth;

                        set = true;
                    }
                }
                else if (optionsMenu.difficulty == 2) //Hard
                {
                    thePlayerHealth.damage = 15;
                    
                    foreach (BossEnemy enemy in Boss3.bossEnemies)
                    {
                        enemy.damage = 10;
                        enemy.shotTimer = 1200;
                    }
                    foreach (BossEnemy enemy in Boss4.bossEnemies)
                    {
                        enemy.damage = 10;
                        enemy.shotTimer = 1200;
                    }
                    if (set == false)
                    {
                        playerLives = 2;
                        thePlayer.nuke = 1;
                        Boss1.maxHealth = 450;
                        Boss2.maxHealth = 550;
                        Boss3.maxHealth = 2400;
                        Boss3.damage = 10;
                        Boss4.maxHealth = 3000;
                        Boss4.damage = 10;
                        Boss1.health = Boss1.maxHealth;
                        Boss2.health = Boss2.maxHealth;
                        Boss3.health = Boss3.maxHealth;
                        Boss4.health = Boss4.maxHealth;

                        set = true;
                    }
                }
                else if (optionsMenu.difficulty == 3) //Insane
                {
                    thePlayerHealth.damage = 20;
                    foreach (BossEnemy enemy in Boss3.bossEnemies)
                    {
                        enemy.damage = 5;
                        enemy.shotTimer = 1000;
                    }
                    foreach (BossEnemy enemy in Boss4.bossEnemies)
                    {
                        enemy.damage = 5;
                        enemy.shotTimer = 1000;
                    }
                    if (set == false)
                    {
                        playerLives = 1;
                        thePlayer.nuke = 0;
                        Boss1.maxHealth = 1500;
                        Boss2.maxHealth = 2300;
                        Boss3.maxHealth = 3200;
                        Boss3.damage = 5;
                        Boss4.maxHealth = 4000;
                        Boss4.damage = 5;
                        Boss1.health = Boss1.maxHealth;
                        Boss2.health = Boss2.maxHealth;
                        Boss3.health = Boss3.maxHealth;
                        Boss4.health = Boss4.maxHealth;

                        set = true;
                    }
                }
               

                if (debug == true)
                {
                    invincible = true;
                    // modify what we want to debug here!
                    // Make player invicible!
                    if (invincible == true)
                    {

                        if (thePlayerHealth.theCurrentHealth <= 99)
                        {
                            thePlayerHealth.Heal();
                        }
                        if (thePlayer.nuke < 3)
                        {
                            thePlayer.nuke = 3;
                        }
                        if (playerLives < 2)
                        {
                            playerLives = 2;
                        }
                    }
                }
                //Updates Level components
                levels.update(this);

                // keep track of the player's health
                thePlayerHealth.Update(gameTime);

                // Find out if it's time to exit game
                if (gameIsOver == true)
                {
                    gameOver.gameOver = true;
                    MediaPlayer.Play(audio.song_GameOver);
                    gamePlay = false;
                    thePlayer.missiles = false;
                    for (int i = theEnemy.Count - 1; i >= 0; i--)
                    {
                        Enemy enemy = theEnemy[i];
                        enemy.Update(gameTime);
                        theEnemy[i] = enemy; // you need this is Enemy is a value type 
                        theEnemy.Remove(enemy);
                        enemyCount--;
                        enemyCount = 0;
                    }
                    highScore.SetHighScore();
                }

                // Start the game, or reset the player upon death
                if (gameStart == true || playerDeath == true)
                {
                    thePlayer.PutinStartPosition();
                    gameStart = false;
                    if (playerDeath == true)
                    {
                        unflash = true;
                        if (playerLives == 0)
                        {
                            gameIsOver = true;
                        }
                        else
                        {
                            thePlayer.gunSelector = 1;
                            playerInput.shotCounter = 0;
                            thePlayer.missiles = false;
                            playerLives -= 1;
                            playerDeath = false;
                            thePlayer.heat = 0;
                        }
                    }
                }

                // Update the player as they do actions via input
                thePlayer.Update(gameTime);

                // Update the enemy and watch for thier death or moving outside of screen
                for (int i = theEnemy.Count - 1; i >= 0; i--)
                {
                    Enemy enemy = theEnemy[i];
                    enemy.Update(gameTime);
                    theEnemy[i] = enemy; // you need this is Enemy is a value type 
                    if (enemy.enemyDeath == true)
                    {
                        newPowerUpTime(gameTime);
                        theEnemy.Remove(enemy);
                        enemyCount--;
                        if (boss1Active == false && boss2Active == false && boss3Active == false && boss4Active == false)
                        {
                            theScore.score += 100;
                        }
                    }
                    if (enemy.enemyPosition.Y >= screenBounds.Height + 256 && enemy.topDown == false
                        && enemy.leftSideStrike == false && enemy.rightSideStrike == false)
                    {
                        enemyCount--;
                        theEnemy.Remove(enemy);
                        if (enemyCount == 1)
                        {
                            enemyCount = 0;
                        }
                    }
                    if (enemy.leftSideStrike == true && enemy.enemyPosition.X >= screenBounds.Width + 256)
                    {
                        enemyCount--;
                        theEnemy.Remove(enemy);
                        if (enemyCount == 1)
                        {
                            enemyCount = 0;
                        }
                    }
                    if (enemy.rightSideStrike == true && enemy.enemyPosition.X <= screenBounds.Left - 256)
                    {
                        enemyCount--;
                        theEnemy.Remove(enemy);
                        if (enemyCount == 1)
                        {
                            enemyCount = 0;
                        }
                    }
                    if (enemy.topDown == true && enemy.enemyPosition.Y >= screenBounds.Height + 256)
                    {
                        enemyCount--;
                        theEnemy.Remove(enemy);
                        if (enemyCount >= 1)
                        {
                            enemyCount = 0;
                        }
                    }
                }



                // Update any powerups on the screen currently
                foreach (PowerUp powerUp in thePowerUp)
                {
                    powerUp.Update(gameTime);
                }

                // Create a new enemy as long as the boss battle is not going on
                // we will be changing the code to formations in a later version
                if ((theEnemy.Count <= 0) && changeLevel == false && gameWon == false && boss3Active == false && boss4Active == false && boss1Active == false && boss2Active == false & nextLevel == false)
                {
                    enemyCount = 0;
                    for (int i = theEnemy.Count - 1; i >= 0; i--)
                    {
                        Enemy enemy = theEnemy[i];
                        theEnemy[i] = enemy; // you need this is Enemy is a value type 
                        theEnemy.Remove(enemy);
                    }
                    newEnemyTime();
                }
                else
                {
                    enemyTimer += gameTime.ElapsedGameTime.Milliseconds;
                }

                // based on score see if it's time to start the boss battle
                // We will be modifing this code later on to detect end of level for boss battles
                if (levels.isEnd(0) && boss1Active == false & boss1Defeated == false)
                {
                    boss1Active = true;
                    playSong();

                    levels.changeAsteroidSpawnRate(0.02f);

                    for (int i = theEnemy.Count - 1; i >= 0; i--)
                    {
                        Enemy enemy = theEnemy[i];
                        enemy.Update(gameTime);
                        theEnemy[i] = enemy; // you need this is Enemy is a value type 
                        enemy.enemyDeath = true;
                    }
                }

                // based on score see if it's time to start the boss battle
                // We will be modifing this code later on to detect end of level for boss battles
                if (levels.isEnd(1) && boss2Active == false & boss2Defeated == false)
                {
                    boss2Active = true;
                    playSong();
                    levels.changeAsteroidSpawnRate(0.02f);

                    for (int i = theEnemy.Count - 1; i >= 0; i--)
                    {
                        Enemy enemy = theEnemy[i];
                        enemy.Update(gameTime);
                        theEnemy[i] = enemy; // you need this is Enemy is a value type 
                        enemy.enemyDeath = true;
                    }
                }

                // based on score see if it's time to start the boss battle
                // We will be modifing this code later on to detect end of level for boss battles
                if (levels.isEnd(2) && boss3Active == false & boss3Defeated == false)
                {
                    boss3Active = true;
                    playSong();
                    Boss3Time();

                    // Remove the current enemies and set the score to what it should be as current 
                    // enemies may exist
                    for (int i = theEnemy.Count - 1; i >= 0; i--)
                    {
                        Enemy enemy = theEnemy[i];
                        enemy.Update(gameTime);
                        theEnemy[i] = enemy; // you need this is Enemy is a value type 
                        enemy.enemyDeath = true;
                    }
                }

                if (levels.isEnd(3) && boss4Active == false & boss4Defeated == false)
                {
                    boss4Active = true;
                    playSong();
                    Boss4Time();
                    // Remove the current enemies and set the score to what it should be as current 
                    // enemies may exist
                    for (int i = theEnemy.Count - 1; i >= 0; i--)
                    {
                        Enemy enemy = theEnemy[i];
                        enemy.Update(gameTime);
                        theEnemy[i] = enemy; // you need this is Enemy is a value type 
                        enemy.enemyDeath = true;
                    }
                }

                if (boss1Active == true)
                {
                    if (Boss1.isDead())
                    {
                        boss1Active = false;
                        boss1Defeated = true;
                        theScore.score += 30000;
                        boss1Active = false;
                        nextLevel = true;
                        level1 = true;
                        levelComplete.drawGrade = true;
                        levels.Rocks.active = false;
                        levels.clearLevel();
                    }
                    else
                    {
                        Boss1.Update();
                        enemyTimer = 0;
                    }
                }

                if (boss2Active == true)
                {
                    if (Boss2.isDead())
                    {
                        boss2Active = false;
                        boss2Defeated = true;
                        theScore.score += 30000;
                        boss2Active = false;
                        nextLevel = true;
                        level2 = true;
                        levelComplete.drawGrade = true;
                        levels.Rocks.active = false;
                        levels.clearLevel();
                    }
                    else
                    {
                        Boss2.update();
                        enemyTimer = 0;
                    }
                }

                // check to see if we have deafeated the boss.
                if (boss3Active == true)
                {
                    if (Boss3.enemyDeath == true && boss3Active == true)
                    {
                        Boss3.Visible = false;
                        boss3Active = false;
                        boss3Defeated = true;
                        theScore.score += 30000;
                        nextLevel = true;
                        level3 = true;
                        levelComplete.drawGrade = true;
                        levels.Rocks.active = false;
                        levels.clearLevel();
                    }

                    else
                    {
                        Boss3.Update(gameTime);
                        enemyTimer = 0;
                    }
                }
                if (boss4Active == true)
                {
                    if (Boss4.deathTimer > Boss4.deathPause && boss4Active == true)
                    {
                        Boss4.Visible = false;
                        boss4Active = false;
                        boss4Defeated = true;
                        theScore.score += 30000;
                        nextLevel = true;
                        level4 = true;
                        levelComplete.drawGrade = true;
                        levels.Rocks.active = false;
                        levels.clearLevel();
                    }

                    else
                    {
                        Boss4.Update(gameTime);
                        enemyTimer = 0;
                    }
                }

                if (nextLevel == true)
                {
                    if (level1 == true)
                    {
                        if (nextLevelTimer >= nextLevelTime)
                        {
                            changeLevel = true;
                            levelComplete.drawGrade = false;
                            nextLevel = false;
                            nextLevelTimer = 0;
                        }
                        else
                        {
                            levelComplete.Update(gameTime);
                            nextLevelTimer += gameTime.ElapsedGameTime.Milliseconds;
                            nextLevel = true;
                        }
                    }
                    else if (level2 == true)
                    {
                        if (nextLevelTimer >= nextLevelTime)
                        {
                            changeLevel = true;
                            levelComplete.drawGrade = false;
                            nextLevel = false;
                            nextLevelTimer = 0;

                        }
                        else
                        {
                            levelComplete.Update(gameTime);
                            nextLevelTimer += gameTime.ElapsedGameTime.Milliseconds;
                            nextLevel = true;
                        }
                    }
                    else if (level3 == true)
                    {
                        if (nextLevelTimer >= nextLevelTime)
                        {
                            changeLevel = true;
                            levelComplete.drawGrade = false;
                            nextLevel = false;
                            nextLevelTimer = 0;

                        }
                        else
                        {
                            levelComplete.Update(gameTime);
                            nextLevelTimer += gameTime.ElapsedGameTime.Milliseconds;
                            nextLevel = true;
                        }
                    }
                    else if (level4 == true)
                    {
                        if (nextLevelTimer >= nextLevelTime)
                        {
                            levelComplete.drawGrade = false;
                            changeLevel = true;
                            nextLevel = false;
                            nextLevelTimer = 0;
                        }
                        else
                        {
                            levelComplete.Update(gameTime);
                            nextLevelTimer += gameTime.ElapsedGameTime.Milliseconds;
                            nextLevel = true;
                        }
                    }
                    else nextLevel = false;
                }

                if (changeLevel == true && (level1 == true || level2 == true || level3 == true || level4 == true) )
                {
                    if (level1 == true)
                    {
                        level++;
                        levels.changeLevel(1);
                        levels.Rocks.active = true;
                        changeLevel = false;
                        level1 = false;
                    }
                    if (level2 == true)
                    {
                        level++;
                        levels.changeLevel(2);
                        levels.Rocks.active = true;
                        changeLevel = false;
                        level2 = false;
                    }
                    if (level3 == true)
                    {
                        level++;
                        levels.changeLevel(3);
                        levels.Rocks.active = true;
                        changeLevel = false;
                        level3 = false;
                    }
                    if (level4 == true)
                    {
                        
                        gameWon = true;
                        changeLevel = false;
                        level4 = false;
                    }
                    levelComplete.Update(gameTime);
                }
                if (flashScreen == true)
                {
                    if (flashTimer >= flashTime)
                    {
                        flashScreen = false;
                        hasFlashed = true;
                        flashTimer = 0;
                    }
                    else
                    {
                        flashTimer += gameTime.ElapsedGameTime.Milliseconds;
                    }
                }
                if (DropLife == false)
                {
                    if (lifeDropTimer >= lifeDropTime)
                    {
                        lifeDropTimer = 0;
                        DropLife = true;
                    }
                    else
                    {
                        lifeDropTimer += gameTime.ElapsedGameTime.Milliseconds;
                    }
                }
                if (optionsMenu.difficulty == 2 || optionsMenu.difficulty == 3)
                {
                    if (DropHeal == false && thePlayerHealth.theCurrentHealth <= 80)
                    {
                        if (healDropTimer >= healDropTime)
                        {
                            healDropTimer = 0;
                            DropHeal = true;
                        }
                        else
                        {
                            healDropTimer += gameTime.ElapsedGameTime.Milliseconds;
                        }
                    }
                }
                if (modTimer >= modTime)
                {
                    movementMod = 1 + random2.Next(3);
                    modTimer = 0;
                }
                else
                {
                    modTimer += gameTime.ElapsedGameTime.Milliseconds;
                }

            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Clear the current graphics
            GraphicsDevice.Clear(Color.Black);
            // Begin drawing
            spriteBatch.Begin();
            spriteBatch.Draw(background, backgroundRect, Color.White);


            if (gameWon)
            {
                highScore.SetHighScore();
                theEndDraw(spriteBatch);
                spriteBatch.End();
                return;
            }
           

            if (gameMenu == true)
            {
                mainMenu.Draw(this.spriteBatch);
            }

            if (gameOptions == true)
            {
                optionsMenu.Draw(this.spriteBatch);
            }

            if (gameHelp == true)
            {
                helpMenu.Draw(this.spriteBatch);
            }

            if (gameScore == true)
            {
                highScore.Draw(this.spriteBatch);
            }

            if (gameOver.gameOver == true)
            {
                gameOver.Draw(this.spriteBatch);
                theScore.Draw(this.spriteBatch); // Draw the score
            }

            if (gamePlay == true || gamePaused == true)
            {
                levels.drawBackground(spriteBatch, backgroundRect);
                levels.drawLevel(spriteBatch);
                if (flashScreen == true && hasFlashed == false)
                {
                    spriteBatch.Draw(flash, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.Red);
                }
                thePlayerHealth.Draw(this.spriteBatch); // Draw the health
                thePlayer.Draw(this.spriteBatch); // Draw the player
                foreach (Enemy enemy in theEnemy)
                {
                    enemy.Draw(this.spriteBatch); // Draw each enemy
                }

                foreach (PowerUp powerUp in thePowerUp)
                {
                    powerUp.Draw(this.spriteBatch); // Draw each powerup
                }

                theScore.Draw(this.spriteBatch); // Draw the score

                if (boss3Active == true)
                {
                    Boss3.Draw(this.spriteBatch); // Draw the boss if it's time
                }
                if (boss4Active == true)
                {
                    Boss4.Draw(this.spriteBatch); // Draw the boss if it's time
                }
                if (boss1Active == true)
                {
                    Boss1.Draw(spriteBatch); // Draw the boss if it's time
                }

                if (boss2Active == true)
                {
                    Boss2.draw(spriteBatch); // Draw the boss if it's time
                }

                bar.draw(spriteBatch, this);          //<-------------Draw Health bar
                if (levelComplete.drawGrade == true)
                {
                    levelComplete.Draw(this.spriteBatch);
                }

            }

            if (gamePaused == true)
            {
                pauseMenu.Draw(this.spriteBatch);
            }



            // End Drawing
            spriteBatch.End();
        }

        /*
         * newEnemyTime, will be called after a counter has reach X time.
         * This class will check and see if in the list an Enemy currently exists,
         * if it does it will not attempt to create a new one but move on to the
         * next spot in the list.  Once a total of 10 enemies are on the screen.
         * it will not create a new Enemy.
         */
        public void newEnemyTime()
        {
            bool createNew = true;
            int formation = 1+ random.Next(4) ;
            int enemies = 0;
            int randomNum = 1 + random.Next(2);
            enemyCount = 1;
            int lHorizontalPos = 384 + random.Next(512);
            int vHorizontalPos = 512 + random.Next(128);
            int xHorizontalPos = 512 + random.Next(128);
            int lVertPos = 192 + random.Next(312);
            int speed = 2 + random.Next(2);
            
            if (movementMod == 1)
            {
                formation--;
            }
            else if (movementMod == 3)
            {
                formation++;
            }

            if (formation == 0)
            {
                formation = 1;
            }
            if (formation == 6)
            {
                formation = 5;
            }
            

            if (createNew == true)
            {
                switch (formation)
                {
                    case 1:
                        {
                            enemies = 5;
                            randomNum = 1 + random.Next(3);
                            break;
                        }
                    case 2:
                        {
                            enemies = 5;
                            break;
                        }
                    case 3:
                        {
                            enemies = 5;
                            break;
                        }
                    case 4:
                        {
                            enemies = 5;
                            randomNum = 1 + random.Next(2);
                            break;
                        }
                    case 5:
                        {
                            enemies = 9;
                            randomNum = 1 + random.Next(3);
                            break;
                        }
                }
                while (enemyCount <= enemies)
                {
                    switch (formation)
                    {
                        case 1: // v  formation
                            {
                                switch (enemyCount)
                                {
                                    case 1:
                                        {
                                            Enemy enemy = new Enemy(this);

                                            switch (randomNum)
                                            {
                                                case 1:
                                                    {
                                                        enemy.enemyPosition.X = vHorizontalPos - 256;
                                                        enemy.enemyPosition.Y = screenBounds.Top - 128;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        enemy.enemyPosition.X = vHorizontalPos;
                                                        enemy.enemyPosition.Y = screenBounds.Top - 128;
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        enemy.enemyPosition.X = vHorizontalPos + 256;
                                                        enemy.enemyPosition.Y = screenBounds.Top - 128;
                                                        break;
                                                    }
                                            }

                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 2:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            switch (randomNum)
                                            {
                                                case 1:
                                                    {
                                                        enemy.enemyPosition.X = vHorizontalPos - 384;
                                                        enemy.enemyPosition.Y = screenBounds.Top - 256;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        enemy.enemyPosition.X = vHorizontalPos - 128;
                                                        enemy.enemyPosition.Y = screenBounds.Top - 256;
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        enemy.enemyPosition.X = vHorizontalPos + 384;
                                                        enemy.enemyPosition.Y = screenBounds.Top - 256;
                                                        break;
                                                    }
                                            }
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 3:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            switch (randomNum)
                                            {
                                                case 1:
                                                    {
                                                        enemy.enemyPosition.X = vHorizontalPos - 512;
                                                        enemy.enemyPosition.Y = screenBounds.Top - 384;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        enemy.enemyPosition.X = vHorizontalPos - 256;
                                                        enemy.enemyPosition.Y = screenBounds.Top - 384;
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        enemy.enemyPosition.X = vHorizontalPos + 512;
                                                        enemy.enemyPosition.Y = screenBounds.Top - 384;
                                                        break;
                                                    }
                                            }
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 4:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            switch (randomNum)
                                            {
                                                case 1:
                                                    {
                                                        enemy.enemyPosition.X = vHorizontalPos - 128;
                                                        enemy.enemyPosition.Y = screenBounds.Top - 256;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        enemy.enemyPosition.X = vHorizontalPos + 128;
                                                        enemy.enemyPosition.Y = screenBounds.Top - 256;
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        enemy.enemyPosition.X = vHorizontalPos + 128;
                                                        enemy.enemyPosition.Y = screenBounds.Top - 256;
                                                        break;
                                                    }
                                            }
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 5:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            switch (randomNum)
                                            {
                                                case 1:
                                                    {
                                                        enemy.enemyPosition.X = vHorizontalPos;
                                                        enemy.enemyPosition.Y = screenBounds.Top - 384;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        enemy.enemyPosition.X = vHorizontalPos + 256;
                                                        enemy.enemyPosition.Y = screenBounds.Top - 384;
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        enemy.enemyPosition.X = vHorizontalPos;
                                                        enemy.enemyPosition.Y = screenBounds.Top - 384;
                                                        break;
                                                    }
                                            }
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                }
                                break;
                            }
                        case 2: // Left to right formation
                            {
                                switch (enemyCount)
                                {
                                    case 1:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            enemy.enemyPosition.X = (screenBounds.Left - 128);
                                            enemy.enemyPosition.Y = lVertPos - 128;
                                            enemy.leftSideStrike = true;
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 2:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            enemy.enemyPosition.X = (screenBounds.Left - 256);
                                            enemy.enemyPosition.Y = lVertPos - 128;
                                            enemy.leftSideStrike = true;
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 3:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            enemy.enemyPosition.X = (screenBounds.Left - 384);
                                            enemy.enemyPosition.Y = lVertPos - 128;
                                            enemy.leftSideStrike = true;
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 4:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            enemy.enemyPosition.X = (screenBounds.Left - 512);
                                            enemy.enemyPosition.Y = lVertPos - 128;
                                            enemy.leftSideStrike = true;
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 5:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            enemy.enemyPosition.X = (screenBounds.Left - 640);
                                            enemy.enemyPosition.Y = lVertPos - 128;
                                            enemy.leftSideStrike = true;
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                }
                                break;
                            }
                        case 3: // right to left
                            {
                                switch (enemyCount)
                                {
                                    case 1:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            enemy.enemyPosition.X = (screenBounds.Width + 128);
                                            enemy.enemyPosition.Y = lVertPos - 128;
                                            enemy.rightSideStrike = true;
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 2:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            enemy.enemyPosition.X = (screenBounds.Width + 256);
                                            enemy.enemyPosition.Y = lVertPos - 128;
                                            enemy.rightSideStrike = true;
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 3:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            enemy.enemyPosition.X = (screenBounds.Width + 384);
                                            enemy.enemyPosition.Y = lVertPos - 128;
                                            enemy.rightSideStrike = true;
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 4:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            enemy.enemyPosition.X = (screenBounds.Width + 512);
                                            enemy.enemyPosition.Y = lVertPos - 128;
                                            enemy.rightSideStrike = true;
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 5:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            enemy.enemyPosition.X = (screenBounds.Width + 640);
                                            enemy.enemyPosition.Y = lVertPos - 128;
                                            enemy.rightSideStrike = true;
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                }
                                break;
                            }
                        case 4: // top down
                            {
                                switch (enemyCount)
                                {
                                    case 1:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            enemy.enemyPosition.X = lHorizontalPos;
                                            enemy.enemyPosition.Y = (screenBounds.Top) - 128;
                                            enemy.topDown = true;
                                            switch (randomNum)
                                            {
                                                case 1:
                                                    {
                                                        enemy.topDownLongLeft = true;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        enemy.topDownShortLeft = true;
                                                        break;
                                                    }
                                            }
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 2:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            enemy.enemyPosition.X = lHorizontalPos;
                                            enemy.enemyPosition.Y = (screenBounds.Top) - 256;
                                            enemy.topDown = true;
                                            switch (randomNum)
                                            {
                                                case 1:
                                                    {
                                                        enemy.topDownLongLeft = true;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        enemy.topDownShortLeft = true;
                                                        break;
                                                    }
                                            }
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 3:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            enemy.enemyPosition.X = lHorizontalPos;
                                            enemy.enemyPosition.Y = (screenBounds.Top) - 384;
                                            enemy.topDown = true;
                                            switch (randomNum)
                                            {
                                                case 1:
                                                    {
                                                        enemy.topDownLongLeft = true;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        enemy.topDownShortLeft = true;
                                                        break;
                                                    }
                                            }
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 4:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            enemy.enemyPosition.X = lHorizontalPos;
                                            enemy.enemyPosition.Y = (screenBounds.Top) - 512;
                                            enemy.topDown = true;
                                            switch (randomNum)
                                            {
                                                case 1:
                                                    {
                                                        enemy.topDownLongLeft = true;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        enemy.topDownShortLeft = true;
                                                        break;
                                                    }
                                            }
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 5:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            enemy.enemyPosition.X = lHorizontalPos;
                                            enemy.enemyPosition.Y = (screenBounds.Top) - 640;
                                            enemy.topDown = true;
                                            switch (randomNum)
                                            {
                                                case 1:
                                                    {
                                                        enemy.topDownLongLeft = true;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        enemy.topDownShortLeft = true;
                                                        break;
                                                    }
                                            }
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                }
                                break;
                            }
                        case 5: // X Formation
                            {
                                switch (enemyCount)
                                {
                                    case 1:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            switch (randomNum)
                                            {
                                                case 1:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos - 256;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 384;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 384;
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos + 256;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 384;
                                                        break;
                                                    }
                                            }
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 2:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            switch (randomNum)
                                            {
                                                case 1:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos - 384;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 512;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos - 128;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 512;
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos + 128;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 512;
                                                        break;
                                                    }
                                            }
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 3:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            switch (randomNum)
                                            {
                                                case 1:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos - 512;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 640;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos - 256;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 640;
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos + 512;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 640;
                                                        break;
                                                    }
                                            }
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 4:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            switch (randomNum)
                                            {
                                                case 1:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos - 128;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 512;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos + 128;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 512;
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos + 384;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 512;
                                                        break;
                                                    }
                                            }
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 5:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            switch (randomNum)
                                            {
                                                case 1:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 640;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos + 256;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 640;
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 640;
                                                        break;
                                                    }
                                            }
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 6:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            switch (randomNum)
                                            {
                                                case 1:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos - 128;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 256;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos + 128;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 256;
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos + 384;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 256;
                                                        break;
                                                    }
                                            }
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 7:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            switch (randomNum)
                                            {
                                                case 1:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 128;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos + 256;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 128;
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos + 512;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 128;
                                                        break;
                                                    }
                                            }
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 8:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            switch (randomNum)
                                            {
                                                case 1:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos - 384;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 256;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos - 128;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 256;
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos + 128;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 256;
                                                        break;
                                                    }
                                            }
                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                            if (speed == 2 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                            else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if (optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                    case 9:
                                        {
                                            Enemy enemy = new Enemy(this);
                                            switch (randomNum)
                                            {
                                                case 1:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos - 512;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 128;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos - 256;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 128;
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        enemy.enemyPosition.X = xHorizontalPos;
                                                        enemy.enemyPosition.Y = (screenBounds.Top) - 128;
                                                        break;
                                                    }
                                            }

                                            enemy.Create();
                                            enemy.LoadContent(enemyContentManager);
                                             if (speed == 2 && optionsMenu.difficulty >=2)
                                            {
                                                enemy.shotTimer = 900;
                                            }
                                             else if (speed == 3 && optionsMenu.difficulty >= 2)
                                            {
                                                enemy.shotTimer = 1200;
                                            }
                                            else if(optionsMenu.difficulty >=2)
                                            {
                                                enemy.shotTimer = 1500;
                                            }
                                            enemy.speed = speed;
                                            theEnemy.Add(enemy);
                                            enemyCount++; // keep track of how many enemies are out there right now
                                            trueEnemyCount++; // used for seeing what the highest score possible was
                                            break;
                                        }
                                }
                                break;
                            }
                    }


                }
            }
        }

        /*
         * newPowerUpTime, will be called after a enemy has been destroyed.
         * This class will check and see if in the list a powerup currently exists,
         * if it does it will not attempt to create a new one but move on to the
         * next spot in the list.  Once a total of 4 powerups are on the screen.
         * it will not create a new powerup.
         */
        public void newPowerUpTime(GameTime gameTime)
        {
            bool createNew = true;
            foreach (PowerUp powerUp in thePowerUp)
            {
                if (powerUp.dropPowerUp == false && powerUp.Visible == true)
                {
                    createNew = false;
                    break;
                }
            }
            if (createNew == true && powerCount < 4)
            {
                PowerUp thePU = new PowerUp(this);
                thePU.Update(gameTime);
                thePowerUp.Add(thePU);
                powerCount++; // keep track of our powerups
            }
        }

        public void addExplosion(Vectors vect)
        {
            levels.addExplosion(vect, Color.Red);
        }

        public void addWarpCloud(Vectors vect)
        {
            levels.addExplosion(vect, Color.Blue);
        }

        public void addGass(Vectors position, Vectors direction)
        {
            levels.addGass(position, direction);
        }

        /*
        * Boss1Time, will be called after a certain score (We will modify this later to be the end of a level).
        * This class just sets Boss1 visibility to true thus starting all of his update features.
        */
        public void Boss3Time()
        {
            Boss3.Visible = true;
        }
        public void Boss4Time()
        {
            Boss4.Visible = true;
        }

        public void playSong()
        {
            levels.playSong();
        }

        public void theEndUpdate()
        {

            if (showCredit)
            {
                creditTime += 1;
                if (creditTime > CreditPause)
                {
                    creditTime = 0;
                    showCredit = false;
                }
            }
            else
            {
                if (flyingSpeed < 32) flyingSpeed += .1f;
                levels.tileMap.yOffset -= (int)flyingSpeed;
                endPlayerPos.X = 600 + getWaveOffset(flyingSpeed, 5, 20);
                endPlayerPos.Y = (float)900 * ((float)levels.tileMap.yOffset / ((float)-levels.tileMap.height * 64 + 720)) - 128;

                if (levels.tileMap.yOffset < -levels.tileMap.height * 64 + 720)
                {
                    levels.tileMap.yOffset = 0;
                    endStage--;
                    flyingSpeed = 0;
                    showCredit = true;
                    switch (endStage)
                    {
                        case 0:
                            levels.tileMap.loadTiles(levels.level1Map);
                            break;
                        case 1:
                            levels.tileMap.loadTiles(levels.level2Map);
                            break;
                        case 2:
                            levels.tileMap.loadTiles(levels.level3Map);
                            break;
                    }

                }

                if (endStage < 0 && !showCredit)
                    restartGame();


            } //end else

        }

        public void theEndDraw(SpriteBatch sb)
        {


            if (showCredit)
            {
                sb.Draw(pixel, backgroundRect, Color.Black);
                sb.DrawString(font, credits[endStage + 1], new Vector2(100, 200), Color.White);
            }
            else
            {
                if (endStage < 0) return;

                levels.drawBackground(sb, backgroundRect);
                levels.tileMap.drawTiles(sb, levels.tileTexture[endStage]);
                sb.Draw(thePlayer.playerShipTexture, 
                    new Rectangle((int)endPlayerPos.X, (int)endPlayerPos.Y,128,128), 
                    new Rectangle(0,0,128,128), Color.White,0, new Vector2(64,64),SpriteEffects.FlipVertically,0.0f);
            }

        }

        public void restartGame()
        {
            restartTheGame = true;
            mainMenu.cursorSelector = 0;
            theEnemy = new List<Enemy>();
            thePowerUp = new List<PowerUp>();
            gameMenu = true; // will show the gameMenu when built.
            gameHelp = false; // will show the game Help when called from MainMenu.
            gameOptions = false; // will show the game Options when called from Main Menu.
            gameHighScores = false; // will show the high scores on the game.
            gamePlay = false; // starts game play when startgame is called from menu
            gamePaused = false; // Check to see if the game is paused.. if it is don't update
            gameIsOver = false; // checks for game over
            gameStart = true; // make sure we start the game when time to.
            playerDeath = false; // checks for player death when playing game
            boss1Defeated = false; // lets us know if boss1 has been defeated.
            boss2Defeated = false; // lets us know if boss2 has been defeated.
            boss3Defeated = false; // lets us know if boss3 has been defeated.
            boss4Defeated = false; // lets us know if boss4 has been defeated.
            unflash = false; // Fixes an issue with screen flash when hit.
            flashScreen = false;
            hasFlashed = false;
            powerCount = 0; // keeps track of how many powerups are on the screen
            enemyCount = 0; // keeps track of how many enemies are currently on the screen
            trueEnemyCount = 0; // keeps track of how many enemies have been created
            playerLives = 2; // Keeps track of how many lives player has, 2 by default
            enemyTimer = 0; // timer to see if it's time for a new enemy
            enemytype = 0; // random to decide what type of enemy ship texture to load.
            level = 1; // keep track of what level we are on.
            flashTimer = 0; // a counter for flash time
            flashTime = 50; // the timer for flash effect
            audio = new Audio();
            volume = .75f;    //Ranges from 0 to 1
            enemyLoad = "Enemy"; // default string for which enemy texture to use
            powerUpLoad = "ShieldPlus"; // default string for which powerup texture to use
            set = false; // sets different variables based on difficulty.
            nukeTest = 0; //For testing when a nuke is fired; for clearing all obsticles in the levels instance
            debug = false; // turn this off for release!!
            invincible = false; // makes player invincible

            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.ApplyChanges();
            thePlayerHealth = new PlayerHealth(this);
            thePlayer = new Player(this);
            playerInput = new PlayerInput(this);
            theScore = new Score(this);
            Boss3 = new Sparrow(this);
            Boss4 = new Spliter(this);
            mainMenu = new MainMenu(this);
            pauseMenu = new PauseMenu(this);
            helpMenu = new HelpMenu(this);
            gameOver = new GameOver(this);
            optionsMenu = new OptionMenu(this);
            foreach (Enemy enemy in theEnemy) {Components.Add(new Enemy(this));}
            bar = new ToolBar(this);

            audio.loadAudio(this);
            levels = new Levels(this);
            levels.playSong();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            enemyContentManager = this.Content;
            powerUpContentManager = this.Content;
            thePlayer.LoadContent(this.Content, "Player");
            thePlayerHealth.LoadContent(this.Content, "HealthBar");
            theScore.LoadContent(this.Content);
            Boss1 = new Macer(this);
            Boss2 = new GassBoss(this);
            Boss3.LoadContent(this.Content);
            Boss4.LoadContent(this.Content);

            boss1Active = false;
            boss2Active = false;
            boss3Active = false;
            boss4Active = false;

            mainMenu.LoadContent(this.Content);
            pauseMenu.LoadContent(this.Content);
            helpMenu.LoadContent(this.Content);
            gameOver.LoadContent(this.Content);
            foreach (Enemy enemy in theEnemy) {enemy.LoadContent(this.Content);}
            background = this.Content.Load<Texture2D>("title");
            flash = this.Content.Load<Texture2D>("Flash");
            backgroundRect = new Rectangle(0, 0,
                                           graphics.GraphicsDevice.Viewport.Width,
                                           graphics.GraphicsDevice.Viewport.Height);
            screenBounds = new Rectangle(0, 0,
               Window.ClientBounds.Width,
               Window.ClientBounds.Height);
            random = new Random(this.GetHashCode()); // create a random hash to generate random numbers from

            nextLevel = false;
            changeLevel = false;

            flyingSpeed = 0;
            //finalFader = 0;
            endStage = 3;
            gameWon = false;
            showCredit = false;
            CreditPause = 500;
            creditTime = 0;
            endPlayerPos = new Vector2(0, 0);
            levelComplete = new LevelComplete(this);
            levelComplete.LoadContent(this.Content);
        }

        public static float getWaveOffset(float currentPosition, float speed, float maxOffset)
        {
            return maxOffset * (float)Math.Sin(currentPosition + speed);
        }

        public bool isDebugging()
        {
            return debug;
        }

        public void gotoNextLevel()
        {
            levels.changeLevel(levels.currentLevel + 1);
        }

        public void endLevel()
        {
            levels.tileMap.yOffset = 0;
        }

        public void toggleDebug()
        {
            debug = !debug;
        }
    }
}