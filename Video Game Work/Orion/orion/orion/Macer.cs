using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace orion
{
    public class Macer
    {
        Circles position = new Circles();
        Vectors direction = new Vectors(1, 1);
        Circles macePosition = new Circles();
        Vectors offset = new Vectors(128, 0);
        public float health = 1000;
        public float maxHealth = 1000;
        Random rnd = new Random();
        Game1 theGame;

        bool maceLoaded = true;
        int playerHurt = 0;
        int maceDegrees = 0;
        int extention = 0;
        int extentionMax = 256;
        int macePause = 0;
        int shipRotation = 90;

        int portX = 1280;
        int portY = 720;
        int spriteRadius = 128;
        bool fighting = false;
        Texture2D ship, mace;

        //Death sequence variables
        float alpha = 1;
        int deathTimer = 0;
        int deathPause = 300;
        float rValue = 1;
        

        public Macer(Game1 game)
        {
            theGame = game;
            position = new Circles(640, -256, 0, 128);
            ship = game.Content.Load<Texture2D>("Boss3");
            mace = game.Content.Load<Texture2D>("Boss3Mace");
            health = maxHealth;
        }

        public bool isDead()
        {
            if (health <= 0 && deathTimer > deathPause)
                return true;
            else
                return false;
        }

        public void Update()
        {
            //Death sequence
            if (health <= 0)
            {
                alpha -= .01f;
                deathTimer += 1;
                if (alpha <= 0)
                    rValue -= .05f;
                else
                    theGame.addExplosion(new Vectors(position.vertex.x + rnd.Next(-128, 128), position.vertex.y + rnd.Next(-128, 128)));
                return;
            }


            //Check for a collision with the player
            Circles player = new Circles(theGame.thePlayer.playerPosition.X,
                                         theGame.thePlayer.playerPosition.Y,
                                         0, 64);
            Circles tempCirc = new Circles(macePosition.vertex, 64);    //Mace

            if ((Circles.overlap(tempCirc, player) || Circles.overlap(position, player)) && playerHurt == 0)
            {
                theGame.thePlayerHealth.rumble = true;
                //game.thePlayerHealth.theFlash = true;
                theGame.thePlayerHealth.checkAttack = true;
                playerHurt = 50;
            }

            if (playerHurt > 0) playerHurt -= 1;



            if (fighting)
            {

                if (maceLoaded)
                {
                    if (macePause < 0)
                        maceLoaded = false;
                    else
                    {
                        shipRotation += 1;
                        if (shipRotation > 359) shipRotation = 0;
                        macePause -= 1;
                    }
                }
                else
                {
                    if (extention < extentionMax && maceDegrees < 90) extention += 16;

                    if (extention > 0 && maceDegrees > 315) extention -= 16;
                    if (maceDegrees < 360) maceDegrees += 2;
                    else
                    {
                        maceLoaded = true;
                        macePause = 100;
                        extention = 0;
                        maceDegrees = 0;
                        extentionMax = 64 * rnd.Next(2, 6);
                    }
                }

                //calculate position of ship
                BouceOffPort();
                position.vertex = position.vertex + direction;

                //Calculate position of mace
                Circles temp = new Circles(position.vertex, 0);
                temp.radius = position.radius + extention;
                macePosition.vertex = temp.getArcPosition(maceDegrees + shipRotation);


                //Check for collision with player fire
                for (int i = theGame.thePlayer.thePlayerFire.Count - 1; i >= 0; i--)
                {
                    PlayerFire shot = theGame.thePlayer.thePlayerFire[i];
                    //This is the position of the player fire
                    Rectangle shotPos = new Rectangle((int)shot.firePosition.X, (int)shot.firePosition.Y, 1, 1);
                    Rectangle shipPos = new Rectangle((int)position.vertex.x - spriteRadius + 32, (int)position.vertex.y - spriteRadius + 32, 256, 256);

                    if (shipPos.Contains(shotPos))
                    {
                        health -= 1;
                        theGame.thePlayer.thePlayerFire.Remove(shot);
                        theGame.audio.soundPlay(theGame.audio.sfx_hit);
                    }
                }



            }
            else
            {
                position.vertex.y += 4;

                //Calculate position of mace
                Circles temp = new Circles(position.vertex, 0);
                temp.radius = position.radius + extention;
                macePosition.vertex = temp.getArcPosition(maceDegrees + shipRotation);

                if (position.vertex.y > 260)
                    fighting = true;
            }
        } // end update

        public void Draw(SpriteBatch sb)
        {
            float offset = -position.radius;
            Vector2 pos = new Vector2(position.vertex.x + offset, position.vertex.y + offset);
            offset = -macePosition.radius;
            Vector2 macePos = new Vector2(macePosition.vertex.x + offset, macePosition.vertex.y + offset);
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

            sb.Draw(mace, macePos, color);
            //sb.Draw(ship, pos, Color.White);
            sb.Draw(ship, new Rectangle((int)pos.X + ship.Width / 2, (int)pos.Y + ship.Height / 2, ship.Width, ship.Height),
                new Rectangle(0, 0, ship.Width, ship.Height), color, shipRotation * 0.01745329f,
                new Vector2(ship.Width / 2, ship.Height / 2), SpriteEffects.None, 0.0f);
        }

        void BouceOffPort()
        {
            if (position.vertex.x < 0 + spriteRadius || position.vertex.x > portX - spriteRadius * 2)
            {
                Vectors n = new Vectors(1, 0);
                reflect(n);
            }

            if (position.vertex.y < 64 + spriteRadius || position.vertex.y > portY - spriteRadius * 2)
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

    }
}