using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace orion
{
    class GassCannon
    {
        bool faceRight = true;
        bool doubleCannon = true;
        public Vectors position;
        int waitTime = 0;
        int maxWaitTime = 300;
        int pause = 10;
        public int health = 3;
        Random rnd = new Random();

        public GassCannon(Vectors pos, bool isFacingRight, bool isDoubleCannon)
        {
            position = pos;
            faceRight = isFacingRight;
            doubleCannon = isDoubleCannon;
        }

        public void update(Game1 game)
        {
            position.y += 1;

            waitTime++;

            if (waitTime < maxWaitTime)
            {

                if (waitTime % pause == 0 && health > 0)
                {
                    Vectors tempDir = new Vectors();
                    Vectors offset = new Vectors();
                    if (faceRight)
                    {
                        offset.set(0, -16);
                        tempDir.set(6, rnd.Next(-1, 1));
                    }
                    else
                    {
                        offset.set(-64, -16);
                        tempDir.set(-6, rnd.Next(-1, 1));
                    }
                    game.addGass(position + offset, tempDir);
                }
            }
            else
            {
                if (waitTime > maxWaitTime * 2)
                    waitTime = 0;
            }
        }

        public void draw(SpriteBatch sb, Texture2D gunSprite)
        {
            //Draws dead guns
            if (health > 0)
            {
                int xx;
                if (faceRight)
                {
                    if (doubleCannon) xx = 2;
                    else xx = 3;
                }
                else
                {
                    if (doubleCannon) xx = 0;
                    else xx = 1;
                }
                sb.Draw(gunSprite, new Rectangle((int)position.x, (int)position.y, 64, 64), new Rectangle(xx * 64, 0, 64, 64), Color.White);
            }
        }
    }
}