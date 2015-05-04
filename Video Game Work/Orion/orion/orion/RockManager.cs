using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace orion
{
    class RockManager
    {
        //////////////////////////Variables//////////////////////////
        float createRate;                       //How often rocks are created (percent: 1 - 0)
        int maxRocks;                           //Max number or rocks on screen
        int spriteSize = 128;                   //Size of rock sprite
        int LEFT, RIGHT, TOP, BOTTOM;           //Bounding regeions for screen port
        int size;                               //Number of rocks in list
        Random rnd = new Random();              //Randomizer
        Vector2 breakPlace = new Vector2();     //Position of broken rock
        public bool active;                     //If system is active. No rocks are created if false
        public List<Rock> rocks = new List<Rock>();

        //////////////////////////Constructor////////////////////////
        public RockManager()
        {
            createRate = 0.01f;
            active = true;
            maxRocks = 20;
            size = 0;

            LEFT = -spriteSize;
            RIGHT = 1280 + spriteSize;
            TOP = -spriteSize;
            BOTTOM = 720 + spriteSize;
        }

        //This sets the spawn rate for creating rocks. Rate ranges from 0 to 1.
        public void setSpawnRate(float amount)
        {
            createRate = amount;
        }

        //This deacivates and activates the system.
        public void setActive(bool isActive)
        {
            active = isActive;
        }

        ///////////////////////////Update////////////////////////////////
        //This updates all rocks in system
        public void update(Game1 game)
        {
            if (!active)    //Continues only if system is active
                return;

            //Test if new asteroid should be created
            int rndTest = rnd.Next(201);
            if (size < maxRocks && rndTest < (100 * createRate)) // if random value is less than given percent (createRate)
            {
                Rock temp = new Rock(game);     //Create rock
                rocks.Add(temp);                //Add rock to list
                size += 1;                      //Increase rock count
            }

            Rock removeable = new Rock(game);   //Desinates which rock to remove
            bool killARock = false;             //If a rock is to be destroyed
            bool breakRock = false;             //If a rock is to be broken and split into smaller pieces

            //updates rocks
            foreach (Rock r in rocks)
            {
                //Position of rock. This is for checking collisions
                Rectangle rock = new Rectangle((int)r.position.X, (int)r.position.Y, 128, 128);

                r.update(); //Updates rock's position

                //This marks a rock for destruction if it has moved outside the view port
                if (r.position.X < LEFT || r.position.X > RIGHT ||
                    r.position.Y < TOP || r.position.Y > BOTTOM)
                {
                    removeable = r;     //Mark rock to be destroyed
                    killARock = true;   //Tell program to destroy rock once loop is exited
                }

                //Check for collision with player fire
                for (int i = game.thePlayer.thePlayerFire.Count - 1; i >= 0; i--)
                {
                    PlayerFire shot = game.thePlayer.thePlayerFire[i];
                    //This is the position of the player fire
                    Rectangle shotPos = new Rectangle((int)shot.firePosition.X, (int)shot.firePosition.Y, 1, 1);
                    if (rock.Contains(shotPos)) //If their is a collision
                    {
                        r.health -= 1;          //Deduct hit points
                        if (r.health <= 0)      //If rock is destroyed, mark for destruction
                        {
                            removeable = r;
                            killARock = true;

                            if (!r.part)        //If rock is not already broken, break rock into smaller pieces
                            {
                                breakPlace = r.position;    //Mark position for creation
                                breakRock = true;           //Tell program to create rocks once loop is exited
                            }
                        }
                        game.thePlayer.thePlayerFire.Remove(shot);
                    }
                }// end player shot list loop

                //Check for a collision with the player
                Rectangle player = new Rectangle((int)game.thePlayer.playerPosition.X + 64,
                                                (int)game.thePlayer.playerPosition.Y + 64,
                                                64, 64);


                if (rock.Intersects(player))
                {
                    game.thePlayerHealth.rumble = true;         //Broken for some reason    <----  !!! ***
                    //game.thePlayerHealth.theFlash = true;       //Broken for some reason    <----  !!! ***

                    removeable = r;     //Destroy the rock if colliison is true
                    killARock = true;
                    game.thePlayerHealth.checkAttack = true;    //hurt player


                }
            }//end rock list loop

            //If a rock is marked for destruction, destroy given rock
            if (killARock)
            {
                if (removeable.health <= 0)
                {
                    game.audio.soundPlay(game.audio.sfx_rockSmash);
                    game.theScore.score += 5;
                }
                rocks.Remove(removeable);
                size -= 1;
                
            }

            //If a rock is marked to be broken, break rock into smaller pieces
            if (breakRock)
            {
                for (int i = 0; i < 3; i++) //Create 3 smaller rocks
                {
                    if (size < maxRocks)
                    {
                        rocks.Add(new Rock(game, breakPlace, rnd));
                        size += 1;
                        game.theScore.score += 5;
                    }
                }
            }//end break rock

        }//end update

        //Draw rocks in list
        public void draw(SpriteBatch sb)
        {
            foreach (Rock r in rocks)
            {
                r.draw(sb);
            }
        }


    } // end class
}