using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace orion
{
    class gunManager
    {


        struct bullets
        {
            public Vectors pos;
            public Vectors dir;
        }

        public List<gun> guns = new List<gun>();
        List<bullets> shots = new List<bullets>();
        Random rnd = new Random();
        Vectors newGunPos = new Vectors(0, 0);
        Vectors oldGunPos = new Vectors(1, 1);
        float spriteFrame = 0f;
        public Vectors gunDirection = new Vectors(0, 1);
        public bool spawningActive = true;

        public Vectors spawnTile = new Vectors(75, 75, 75);

        public gunManager()
        {

        }

        public void update(Game1 game, tiles map)
        {
            //updates animation for bullets
            if (spriteFrame > 3) spriteFrame = 0f;
            else spriteFrame += .25f;


            //spawn guns
            if (spawningActive)
                map.spawnGunPad(this, spawnTile);

            //Update guns
            for (int gunNumber = 0; gunNumber < guns.Count; gunNumber++)
            {
                //check for collision with bullets 
                for (int shotNumber = 0; shotNumber < game.thePlayer.thePlayerFire.Count; shotNumber++)
                {
                    if (Circles.overlap(new Circles(guns[gunNumber].gunLocation, 32),
                        new Circles(new Vectors(game.thePlayer.thePlayerFire[shotNumber].firePosition.X, game.thePlayer.thePlayerFire[shotNumber].firePosition.Y), 2)))
                    {
                        guns[gunNumber].health--;
                        game.thePlayer.thePlayerFire.Remove(game.thePlayer.thePlayerFire[shotNumber]);
                    }
                }

                if (guns[gunNumber].update(game, gunDirection))
                {
                    bullets newShot;
                    newShot.pos = guns[gunNumber].gunLocation;
                    newShot.dir = new Vectors();
                    newShot.dir.setPolar(5, guns[gunNumber].dir);
                    shots.Add(newShot);
                    game.audio.soundPlay(game.audio.sfx_turretFire);
                }

                if (guns[gunNumber].isDead())
                {
                    if (guns[gunNumber].health <= 0)
                    {
                        game.theScore.score += 25;
                        game.addExplosion(guns[gunNumber].gunLocation + new Vectors(-64, -64));
                    }
                    guns.Remove(guns[gunNumber]);
                }
            }

            //update bullets
            for (int i = 0; i < shots.Count; i++)
            {
                //Move the bullet
                bullets newShot = shots[i];
                newShot.pos = newShot.pos + newShot.dir;
                shots[i] = newShot;

                //Check if bullet is outside screen
                if (newShot.pos.x < 0 || newShot.pos.x > 1280 ||
                    newShot.pos.y < 0 || newShot.pos.y > 720)
                {
                    shots.Remove(shots[i]);
                    return;
                }

                //check for bullets colliding with player
                if (Circles.overlap(new Circles(new Vectors(game.thePlayer.playerPosition.X + 64, game.thePlayer.playerPosition.Y + 64), 64),
                    new Circles(shots[i].pos, 1)))
                {
                    game.thePlayerHealth.rumble = true;
                    //game.thePlayerHealth.theFlash = true;
                    game.thePlayerHealth.checkAttack = true;
                    shots.Remove(shots[i]);
                }
            }
        }

        public void draw(SpriteBatch sb, Texture2D gunsprite, Texture2D bulletsprite)
        {
            for (int i = 0; i < guns.Count; i++)
            {
                guns[i].draw(sb, gunsprite);
            }

            for (int i = 0; i < shots.Count; i++)
            {
                Rectangle source = new Rectangle((int)spriteFrame * 16, 0, 16, 16);
                sb.Draw(bulletsprite, new Rectangle((int)shots[i].pos.x, (int)shots[i].pos.y, 16, 16), source, Color.White);
            }
        }

        public void addGun(Vectors pos)
        {
            guns.Add(new gun(pos));
        }

    }
}