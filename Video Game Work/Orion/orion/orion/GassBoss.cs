using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace orion
{
    public class GassBoss
    {

        Circles position;
        Vectors direction = new Vectors(1, 2);
        Vectors gunPosition;

        float rotation;

        int rotationWait = 0;       //Decides when the craft will rotate
        public float health = 400;           //Ship's health
        public float maxHealth = 400;
        int portX = 1280;           //Size of the screen
        int portY = 720;
        int gunRespawnPause = 0;    //Decides when the turrent on the craft will regenerate
        int gassShotPause = 0;      //Pause in between plumes of projectile gass
        int hurtPlayer = 0;         //Pause in between hits on the player

        float imageIndex = 0;

        Texture2D shipSprite;       //Boss' sprites
        Texture2D gasSprite;
        Texture2D turretSprite;
        Texture2D bulletSprite;

        bool fighting = false;      //Decides if the boss has entered the screen and is ready to fight
        bool cannonSwitch = true;   //Decides which pair of jets to use to shoot gass
        bool turnRight = true;      //Decides if the boss rotates right (or left)

        List<Gass> gassList = new List<Gass>();
        Random rnd = new Random();
        gunManager guns = new gunManager();

        Game1 theGame;

        //Death sequence variables
        float alpha = 1;
        int deathTimer = 0;
        int deathPause = 300;
        float rValue = 1;

        public GassBoss(Game1 game)
        {
            theGame = game;
            turretSprite = game.Content.Load<Texture2D>("Boss2Gun");
            bulletSprite = game.Content.Load<Texture2D>("GunShot");
            gasSprite = game.Content.Load<Texture2D>("Gass");
            shipSprite = game.Content.Load<Texture2D>("GassBoss");

            position = new Circles(640, -256, 0, 256 / 2);
            gunPosition = position.vertex;

            guns.spawningActive = false;
            guns.gunDirection = new Vectors(0, 0);
            guns.addGun(gunPosition);
            health = maxHealth;
        }


        public void update()
        {
            //Death sequence
            if (health <= 0)
            {
                //remove gas from list
                gassList = new List<Gass>();
                //remove shots from gun manager
                guns = new gunManager();

                alpha -= .01f;
                deathTimer += 1;
                if (alpha <= 0)
                    rValue -= .05f;
                else
                    theGame.addExplosion(new Vectors(position.vertex.x + rnd.Next(-128, 128), position.vertex.y + rnd.Next(-128, 128)));
                return;
            }


            if (imageIndex > 9) imageIndex = 0;
            else imageIndex += .1f;

            //Check for collision with player
            Circles player = new Circles(theGame.thePlayer.playerPosition.X,
                                         theGame.thePlayer.playerPosition.Y,
                                         0, 64);
            if (Circles.overlap(player, position) && hurtPlayer < 1)
            {
                theGame.thePlayerHealth.rumble = true;
                //game.thePlayerHealth.theFlash = true;
                theGame.thePlayerHealth.checkAttack = true;
                hurtPlayer = 50;
            }

            hurtPlayer -= 1;

            //Disable guncount check to allow boss vuneralblility all the time
            //if (guns.guns.Count == 0)
            //{
            //Check for collision with player fire
            for (int i = theGame.thePlayer.thePlayerFire.Count - 1; i >= 0; i--)
            {
                PlayerFire shot = theGame.thePlayer.thePlayerFire[i];
                //This is the position of the player fire
                Rectangle shotPos = new Rectangle((int)shot.firePosition.X, (int)shot.firePosition.Y, 1, 1);
                Rectangle shipPos = new Rectangle((int)position.vertex.x - 256 / 2 + 32,
                    (int)position.vertex.y - 256 + 10, 256, 256); //(int)position.vertex.y - 256 / 2 + 32, 256

                if (shipPos.Contains(shotPos))
                {
                    health -= 1;
                    theGame.thePlayer.thePlayerFire.Remove(shot);
                    theGame.audio.soundPlay(theGame.audio.sfx_hit);
                }
            }
            //}

            if (fighting)
            {



                //update gass projectiles
                for (int i = 0; i < gassList.Count; i++)
                {
                    if (gassList[i].dead())
                        gassList.Remove(gassList[i]);
                    else
                        gassList[i].update();

                    if (gassList[i].collideWithPlayer(theGame, player) && hurtPlayer < 1)
                    {
                        //hurt player
                        theGame.thePlayerHealth.rumble = true;
                        //game.thePlayerHealth.theFlash = true;
                        theGame.thePlayerHealth.checkAttack = true;
                        hurtPlayer = 50;
                    }
                }

                //Shoot Gass from cannons
                if (gassShotPause < 1)
                {
                    if (cannonSwitch)
                    {
                        Vectors gassDir = new Vectors();
                        gassDir.setPolar(5, 90 + rotation);
                        gassList.Add(new Gass(position.getArcPosition(90 + rotation) + new Vectors(-64, -64), gassDir));
                    }
                    else
                    {
                        Vectors gassDir = new Vectors();
                        gassDir.setPolar(5, 180 + rotation);
                        gassList.Add(new Gass(position.getArcPosition(180 + rotation) + new Vectors(-64, -64), gassDir));
                    }

                    if (cannonSwitch)
                    {
                        Vectors gassDir = new Vectors();
                        gassDir.setPolar(5, 270 + rotation);
                        gassList.Add(new Gass(position.getArcPosition(270 + rotation) + new Vectors(-64, -64), gassDir));
                    }
                    else
                    {
                        Vectors gassDir = new Vectors();
                        gassDir.setPolar(5, rotation);
                        gassList.Add(new Gass(position.getArcPosition(rotation) + new Vectors(-64, -64), gassDir));
                    }

                    gassShotPause = 10;
                }
                else
                    gassShotPause -= 1;

                //Respawn gun after pause
                if (guns.guns.Count == 0)
                {
                    gunRespawnPause += 1;
                    if (gunRespawnPause > 200)
                    {
                        gunRespawnPause = 0;
                        guns.addGun(position.vertex);
                    }


                }

                //calculate position of ship
                BouceOffPort();
                position.vertex = position.vertex + direction;
                guns.gunDirection = new Vectors(direction.x, direction.y);
                guns.update(theGame, null);

                //Rotate Ship
                if (rotationWait < 1)
                {
                    rotationWait = rnd.Next(100, 400);
                    int test = rnd.Next(2);
                    if (test == 0) cannonSwitch = true;
                    else cannonSwitch = false;
                    if (turnRight) turnRight = false;
                    else turnRight = true;
                }
                else
                    rotationWait -= 1;
                if (turnRight) rotation += 1;
                else rotation -= 1;

            }
            else
            {
                position.vertex.y += 4;
                if (position.vertex.y > 260)
                    fighting = true;
            }

        }

        public void draw(SpriteBatch sb)
        {
            //Calculate image source
            int xx = 256 * (int)imageIndex;
            int yy = 0;
            if (imageIndex > 4)
            {
                xx = 256 * (int)(imageIndex - 5);
                yy = 256;
            }

            Color color;

            //DrawDeathsequence
            if (health <= 0)
            {
                byte alphaValue = (byte)(MathHelper.Clamp(alpha, 0, 1) * 255);
                byte red = (byte)(MathHelper.Clamp(rValue, 0, 1) * 255);
                color = new Color(red, alphaValue, alphaValue, alphaValue);
            }
            else
                color = Color.White;

            //Draw ship
            sb.Draw(shipSprite, new Rectangle((int)position.vertex.x, (int)position.vertex.y, 256, 256),
                new Rectangle(xx, yy, 256, 256), color, rotation * 0.01745329f,
                new Vector2(position.radius, position.radius), SpriteEffects.None, 0.0f);


            //Draw Gun
            guns.draw(sb, turretSprite, bulletSprite);

            //Draw Gass
            for (int i = 0; i < gassList.Count; i++)
            {
                gassList[i].draw(sb, gasSprite);
            }
        }

        void BouceOffPort()
        {
            if (position.vertex.x < 0 + position.radius || position.vertex.x > portX - position.radius)
            {
                Vectors n = new Vectors(1, 0);
                reflect(n);
            }

            if (position.vertex.y < 64 + position.radius || position.vertex.y > portY - position.radius)
            {
                Vectors n = new Vectors(0, 1);
                reflect(n);
            }
        }

        void reflect(Vectors N)
        {
            //Rr = Ri - 2 N (Ri . N) 
            direction.set(
                direction.x - 2 * N.x * (direction.x * N.x),
                direction.y - 2 * N.y * (direction.y * N.y));
        }

        public bool isDead()
        {
            if (health <= 0 && deathTimer > deathPause)
                return true;
            else
                return false;
        }

    }
}