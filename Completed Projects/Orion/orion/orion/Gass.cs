using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace orion
{
    class Gass
    {
        Vectors position;
        Vectors direction;
        int size = 64;
        float alpha = 1;

        public Gass(Vectors pos, Vectors dir)
        {
            position = pos;
            direction = dir;
        }

        public bool dead()
        {
            if (alpha > 0) return false;
            else return true;
        }

        public void update()
        {
            position = position + direction;
            size += 2;
            alpha -= .02f;
        }

        public void draw(SpriteBatch sb, Texture2D texture)
        {
            byte alphaValue = (byte)(MathHelper.Clamp(alpha, 0, 1) * 255);
            sb.Draw(texture, new Rectangle((int)(position.x + size / 2), (int)(position.y + size / 2), size, size),
                new Color(alphaValue, alphaValue, alphaValue, alphaValue));
        }

        public bool collideWithPlayer(Game1 game, Circles playerLocation)
        {
            if (alpha > .2 && Circles.overlap(playerLocation, new Circles(position, size / 2)))
                return true;
            else
                return false;
        }
    }
}