/*
 * Player Input Class
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
 * This class will bring in keyboard and gamepad input to the game.
 * Based on input this will call out to other areas of the game and cause 
 * actions to occur such as movement and shooting.
 */
namespace orion
{

    public class PlayerInput : GameComponent
    {
        Game1 game; // create an instance of Game
        KeyboardState mKeys; // create the current keyboard
        KeyboardState oKeys; // create the old keyboard
        GamePadState mPad; // create the current gamepad
        GamePadState oPad; // create the old gamepad
        public int gunTick = 1700; // timer for shooting
        public int rapidTick = 700; // timer for shooting
        public int spreadTick = 500; // timer for shooting
        public int rocketTick = 1600; // timer for shooting
        public int shotCounter; // counter for shooting
        public int rocketCounter; // coutner for shooting rockets

        public PlayerInput(Game game)
            : base(game)
        {
            this.game = (Game1)game; // Set Game to current game;
        }

        public bool isFiring()
        {
            if (mKeys.IsKeyDown(Keys.Space) || mPad.IsButtonDown(Buttons.RightTrigger))
                return true;
            else
                return false;
        }

        public override void Update(GameTime gameTime)
        {
            // Get the keyboad and gamepad state
            mKeys = Keyboard.GetState();
            mPad = GamePad.GetState(PlayerIndex.One);

            if (game.gameOptions == true)
            {
                if (mKeys.IsKeyDown(Keys.Left) == true && oKeys.IsKeyUp(Keys.Left) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.optionsMenu.MoveCursorLeft();
                }
                if (mKeys.IsKeyDown(Keys.Right) == true && oKeys.IsKeyUp(Keys.Right) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.optionsMenu.MoveCursorRight();
                }
                if (mKeys.IsKeyDown(Keys.Space) == true && oKeys.IsKeyUp(Keys.Space) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_select);
                    game.optionsMenu.MenuSelect();
                }

                if (mPad.ThumbSticks.Left.X > -0.2F && oPad.ThumbSticks.Left.X < -0.2F)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.optionsMenu.MoveCursorLeft();
                }
                if (mPad.ThumbSticks.Left.X < 0.2F && oPad.ThumbSticks.Left.X > 0.2F)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.optionsMenu.MoveCursorRight();
                }
                if (mPad.IsButtonDown(Buttons.A) == true && oPad.IsButtonUp(Buttons.A) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_select);
                    game.optionsMenu.MenuSelect();
                }
            }
            else if (game.gamePaused == true)
            {
                if (mKeys.IsKeyDown(Keys.Left) == true && oKeys.IsKeyUp(Keys.Left) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.pauseMenu.MoveCursorLeft();
                }
                if (mKeys.IsKeyDown(Keys.Right) == true && oKeys.IsKeyUp(Keys.Right) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.pauseMenu.MoveCursorRight();
                }
                if (mKeys.IsKeyDown(Keys.Space) == true && oKeys.IsKeyUp(Keys.Space) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_select);
                    game.pauseMenu.MenuSelect();
                }

                if (mPad.ThumbSticks.Left.X > -0.2F && oPad.ThumbSticks.Left.X < -0.2F)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.pauseMenu.MoveCursorLeft();
                }
                if (mPad.ThumbSticks.Left.X < 0.2F && oPad.ThumbSticks.Left.X > 0.2F)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.pauseMenu.MoveCursorRight();
                }
                if (mPad.IsButtonDown(Buttons.A) == true && oPad.IsButtonUp(Buttons.A) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_select);
                    game.pauseMenu.MenuSelect();
                }
            }

            else if (game.gameOver.gameOver == true)
            {
                if (mKeys.IsKeyDown(Keys.Left) == true && oKeys.IsKeyUp(Keys.Left) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.gameOver.MoveCursorLeft();
                }
                if (mKeys.IsKeyDown(Keys.Right) == true && oKeys.IsKeyUp(Keys.Right) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.gameOver.MoveCursorRight();
                }
                if (mKeys.IsKeyDown(Keys.Space) == true && oKeys.IsKeyUp(Keys.Space) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_select);
                    game.gameOver.MenuSelect();
                }

                if (mPad.ThumbSticks.Left.X > 0.2F && oPad.ThumbSticks.Left.X < 0.2F)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.gameOver.MoveCursorRight();
                    
                }
                if (mPad.ThumbSticks.Left.X < -0.2F && oPad.ThumbSticks.Left.X > -0.2F)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.gameOver.MoveCursorLeft();
                }
                if (mPad.IsButtonDown(Buttons.A) == true && oPad.IsButtonUp(Buttons.A) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_select);
                    game.gameOver.MenuSelect();
                }
            }

            else if (game.gameHelp == true)
            {
                if (mKeys.IsKeyDown(Keys.Left) == true && oKeys.IsKeyUp(Keys.Left) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.helpMenu.MoveCursorLeft();
                }
                if (mKeys.IsKeyDown(Keys.Right) == true && oKeys.IsKeyUp(Keys.Right) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.helpMenu.MoveCursorRight();
                }
                if (mKeys.IsKeyDown(Keys.Space) == true && oKeys.IsKeyUp(Keys.Space) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_select);
                    game.helpMenu.MenuSelect();
                }

                if (mPad.ThumbSticks.Left.X > -0.2F && oPad.ThumbSticks.Left.X < -0.2F)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.helpMenu.MoveCursorLeft();
                }
                if (mPad.ThumbSticks.Left.X < 0.2F && oPad.ThumbSticks.Left.X > 0.2F)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.helpMenu.MoveCursorRight();
                }
                if (mPad.IsButtonDown(Buttons.A) == true && oPad.IsButtonUp(Buttons.A) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_select);
                    game.helpMenu.MenuSelect();
                }
            }
            else if (game.gameScore == true)
            {
                if (mKeys.IsKeyDown(Keys.Left) == true && oKeys.IsKeyUp(Keys.Left) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.highScore.MoveCursorLeft();
                }
                if (mKeys.IsKeyDown(Keys.Right) == true && oKeys.IsKeyUp(Keys.Right) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.highScore.MoveCursorRight();
                }
                if (mKeys.IsKeyDown(Keys.Space) == true && oKeys.IsKeyUp(Keys.Space) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_select);
                    game.highScore.MenuSelect();
                }

                if (mPad.ThumbSticks.Left.X > -0.2F && oPad.ThumbSticks.Left.X < -0.2F)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.highScore.MoveCursorLeft();
                }
                if (mPad.ThumbSticks.Left.X < 0.2F && oPad.ThumbSticks.Left.X > 0.2F)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.highScore.MoveCursorRight();
                }
                if (mPad.IsButtonDown(Buttons.A) == true && oPad.IsButtonUp(Buttons.A) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_select);
                    game.highScore.MenuSelect();
                }
            }

            else if (game.gameMenu == true)
            {
                if (mKeys.IsKeyDown(Keys.Left) == true && oKeys.IsKeyUp(Keys.Left) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.mainMenu.MoveCursorLeft();
                }
                if (mKeys.IsKeyDown(Keys.Right) == true && oKeys.IsKeyUp(Keys.Right) == true)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.mainMenu.MoveCursorRight();
                }
                if (mKeys.IsKeyDown(Keys.Space) == true && oKeys.IsKeyUp(Keys.Space) == true && game.restartTheGame == false)
                {
                    game.audio.soundPlay(game.audio.sfx_select);
                    game.mainMenu.MenuSelect();
                }

                if (mPad.ThumbSticks.Left.X > -0.2F && oPad.ThumbSticks.Left.X < -0.2F)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.mainMenu.MoveCursorLeft();
                }
                if (mPad.ThumbSticks.Left.X < 0.2F && oPad.ThumbSticks.Left.X > 0.2F)
                {
                    game.audio.soundPlay(game.audio.sfx_changeOption);
                    game.mainMenu.MoveCursorRight();
                }
                if (mPad.IsButtonDown(Buttons.A) == true && oPad.IsButtonUp(Buttons.A) == true && game.restartTheGame == false)
                {
                    game.audio.soundPlay(game.audio.sfx_select);
                    game.mainMenu.MenuSelect();
                }
            }
            else if (game.gamePlay == true)
            {
                if (keyPressed(Microsoft.Xna.Framework.Input.Keys.OemTilde))
                    game.toggleDebug();

                //Debug hot keys
                if (game.isDebugging())
                {
                    if (keyPressed(Microsoft.Xna.Framework.Input.Keys.R))
                        game.restartGame();

                    if (keyPressed(Microsoft.Xna.Framework.Input.Keys.E))   //End of level
                    {
                        game.endLevel();
                    }
                    if (keyPressed(Microsoft.Xna.Framework.Input.Keys.N))   //Next level
                    {
                        game.gotoNextLevel();
                    }
                }

                // Keyboard Controls
                // move player up if up on keyboard is pressed
                if (mKeys.IsKeyDown(Keys.Up) == true)
                {
                    game.thePlayer.playerPosition.Y -= 6;
                }

                // move player down if down on keyboard is pressed
                if (mKeys.IsKeyDown(Keys.Down) == true)
                {
                    game.thePlayer.playerPosition.Y += 6;
                }

                // move player left if left on keyboard is pressed
                if (mKeys.IsKeyDown(Keys.Left) == true)
                {
                    game.thePlayer.playerPosition.X -= 6;
                }

                // move player right if right on keyboard is pressed
                if (mKeys.IsKeyDown(Keys.Right) == true)
                {
                    game.thePlayer.playerPosition.X += 6;
                }

                // Shoot if the Space is pressed and we've hit the
                // timer and the initial gun is true
                if (mKeys.IsKeyDown(Keys.Space) == true &&
                   shotCounter > gunTick &&
                   game.thePlayer.initialGun == true)
                {
                    game.audio.soundPlay(game.audio.sfx_playerShoot);
                    game.thePlayer.Shoot();
                    shotCounter = 0;
                }

                else
                {
                    shotCounter += gameTime.ElapsedGameTime.Milliseconds;
                }

                // Shoot if the Space is pressed and we've hit the
                // timer and the rapid gun (power up required) is true
                if (mKeys.IsKeyDown(Keys.Space) == true &&
                  shotCounter > rapidTick &&
                  game.thePlayer.rapidGun == true)
                {
                    game.audio.soundPlay(game.audio.sfx_playerShoot);
                    game.thePlayer.Shoot();
                    shotCounter = 0;
                }
                else
                {
                    shotCounter += gameTime.ElapsedGameTime.Milliseconds;
                }

                // Shoot if the Space is pressed and we've hit the
                // timer and the spread gun (power up required) is true
                if (mKeys.IsKeyDown(Keys.Space) == true &&
                  shotCounter > spreadTick &&
                  game.thePlayer.spreadGun == true)
                {
                    game.audio.soundPlay(game.audio.sfx_playerShoot);
                    game.thePlayer.SpreadShoot();
                    shotCounter = 0;
                }

                else
                {
                    shotCounter += gameTime.ElapsedGameTime.Milliseconds;
                }

                // Shoot if the Space is pressed and we've hit the
                // timer and the rockets (power up required) is true
                if (mKeys.IsKeyDown(Keys.Space) == true &&
                  rocketCounter > rocketTick &&
                  game.thePlayer.missiles == true)
                {
                    if (game.theEnemy.Count >= 1)
                    {
                        game.thePlayer.RocketShoot();
                        rocketCounter = 0;
                    }
                }

                else
                {
                    rocketCounter += gameTime.ElapsedGameTime.Milliseconds;
                }

                // will call the FireNuke() class from the player if player has 
                // nukes and A key is pressed
                if (mKeys.IsKeyDown(Keys.A) == true && oKeys.IsKeyUp(Keys.A) == true)
                {
                    if (game.thePlayer.nuke >= 1)
                    {
                        game.thePlayer.FireNuke();
                    }
                }


                // Pause the game
                if (mKeys.IsKeyDown(Keys.Enter) == true && oKeys.IsKeyUp(Keys.Enter) == true)
                {
                    game.gamePaused = true;
                }

                // GamePad Controls
                // move player up if thumbsticks go up
                if (mPad.ThumbSticks.Left.Y > 0.2F)
                {
                    game.thePlayer.playerPosition.Y -= 6;
                }

                // move player down if thumbsticks go down
                if (mPad.ThumbSticks.Left.Y < -0.2F)
                {
                    game.thePlayer.playerPosition.Y += 6;
                }

                // move player left if thumbsticks go left
                if (mPad.ThumbSticks.Left.X < -0.2F)
                {
                    game.thePlayer.playerPosition.X -= 6;
                }

                // move player right if thumbsticks go right
                if (mPad.ThumbSticks.Left.X > 0.2F)
                {
                    game.thePlayer.playerPosition.X += 6;
                }

                // Shoot if the right trigger is pressed and we've hit the
                // timer and the initial gun is true            
                if (mPad.IsButtonDown(Buttons.RightTrigger) &&
                   shotCounter > gunTick &&
                   game.thePlayer.initialGun == true)
                {
                    game.audio.soundPlay(game.audio.sfx_playerShoot);
                    game.thePlayer.Shoot();
                    shotCounter = 0;
                }

                else
                {
                    shotCounter += gameTime.ElapsedGameTime.Milliseconds;
                }

                // Shoot if the right Trigger is pressed and we've hit the
                // timer and the rapid gun (power up required) is true
                if (mPad.IsButtonDown(Buttons.RightTrigger) &&
                  shotCounter > rapidTick &&
                  game.thePlayer.rapidGun == true)
                {
                    game.audio.soundPlay(game.audio.sfx_playerShoot);
                    game.thePlayer.Shoot();
                    shotCounter = 0;
                }

                else
                {
                    shotCounter += gameTime.ElapsedGameTime.Milliseconds;
                }

                // Shoot if the right trigger is pressed and we've hit the
                // timer and the spread gun (power up required) is true
                if (mPad.IsButtonDown(Buttons.RightTrigger) &&
                  shotCounter > spreadTick &&
                  game.thePlayer.spreadGun == true)
                {
                    game.audio.soundPlay(game.audio.sfx_playerShoot);
                    game.thePlayer.SpreadShoot();
                    shotCounter = 0;
                }

                else
                {
                    shotCounter += gameTime.ElapsedGameTime.Milliseconds;
                }

                // Shoot if the Space is pressed and we've hit the
                // timer and the rockets (power up required) is true
                if (mPad.IsButtonDown(Buttons.RightTrigger) == true &&
                  rocketCounter > rocketTick &&
                  game.thePlayer.missiles == true)
                {
                    if (game.theEnemy.Count >= 1)
                    {
                        game.thePlayer.RocketShoot();
                        rocketCounter = 0;
                    }
                }

                else
                {
                    rocketCounter += gameTime.ElapsedGameTime.Milliseconds;
                }

                // will call the FireNuke() class from the player if player has 
                // nukes and A button is pressed
                if (mPad.IsButtonDown(Buttons.B) == true && oPad.IsButtonUp(Buttons.B) == true)
                {
                    if (game.thePlayer.nuke >= 1)
                    {
                        game.thePlayer.FireNuke();
                    }
                }

                // Pause the game
                if (mPad.IsButtonDown(Buttons.Start) == true && oPad.IsButtonDown(Buttons.Start) == true)
                {
                    game.gamePaused = true;
                }

                /*
                // exit the game when Back button is pressed
                // will be removed in release
                if (mPad.IsButtonDown(Buttons.Back))
                {
                    game.Exit();
                }

                
                // exit the game when Escape key is pressed
                // will be removed in release
                if (mKeys.IsKeyDown(Keys.Escape))
                {
                    game.Exit();
                }

                //beta controls
                // weapon upgrades 1 for the intial, 2 for rapid fire, 3 for spread gun
                if (mKeys.IsKeyDown(Keys.D1) == true ||
                    mPad.IsButtonDown(Buttons.DPadUp))
                {
                    game.thePlayer.initialGun = true;
                    game.thePlayer.gunSelector = 1;
                }

                if (mKeys.IsKeyDown(Keys.D2) == true ||
                    mPad.IsButtonDown(Buttons.DPadLeft))
                {
                    game.thePlayer.rapidGun = true;
                    game.thePlayer.gunSelector = 2;
                }

                if (mKeys.IsKeyDown(Keys.D3) == true
                    ||
                    mPad.IsButtonDown(Buttons.DPadDown))
                {
                    game.thePlayer.spreadGun = true;
                    game.thePlayer.gunSelector = 3;
                    game.thePlayer.missiles = true;
                }

                //simulate being healed
                if ((mKeys.IsKeyDown(Keys.H) == true && oKeys.IsKeyDown(Keys.H) == false) ||
                    (mPad.IsButtonDown(Buttons.DPadRight) && oPad.IsButtonDown(Buttons.DPadRight)))
                {
                    game.thePlayerHealth.Heal();
                }*/
            }

            // Update keyboard and gamepad state.
            oKeys = mKeys;
            oPad = mPad;

            base.Update(gameTime);
        }

        //Returns true when key is pressed once
        public bool keyPressed(Keys key)
        {
            if (mKeys.IsKeyDown(key) && oKeys.IsKeyUp(key))
                return true;
            else
                return false;
        }

    }
}