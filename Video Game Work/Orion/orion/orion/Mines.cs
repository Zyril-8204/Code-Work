using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace orion
{
    class Mines
    {
        public List<Circles> mines = new List<Circles>();

        Random rnd = new Random();


        int mineRadius = 64;
        int maxMines = 20;
        Vectors mineDirection = new Vectors(.5f, 1);
        public bool minesActive = true;
        public float mineSpawnRate = 0.01f;

        public Mines()
        { }

        public void Update(Game1 game, Levels lvl)
        {
            //Test if a mine should be created
            int test = rnd.Next(301);
            if (test < (100 * mineSpawnRate) && mines.Count < maxMines && minesActive)
            {
                mines.Add(new Circles(new Vectors(rnd.Next(1281), 0), mineRadius));
            }

            //Update mines
            for (int i = 0; i < mines.Count; i++)
            {
                //Move mine
                mines[i].vertex = mines[i].vertex + mineDirection;

                //Detect collision with player
                Circles player = new Circles(new Vectors(game.thePlayer.playerPosition.X + 64, game.thePlayer.playerPosition.Y + 64), 64);
                if (Circles.overlap(player, mines[i]))
                {
                    explode(game, lvl, i);

                    //Destroy all nearby obsticles

                    game.thePlayerHealth.checkAttack = true;
                    game.thePlayerHealth.rumble = true;
                    //game.thePlayerHealth.theFlash = true;
                    mines.Remove(mines[i]);
                    return;
                }

                //Check if shot by player
                for (int shotNumber = 0; shotNumber < game.thePlayer.thePlayerFire.Count; shotNumber++)
                {
                    if (Circles.overlap(mines[i],
                        new Circles(new Vectors(game.thePlayer.thePlayerFire[shotNumber].firePosition.X, game.thePlayer.thePlayerFire[shotNumber].firePosition.Y), 2)))
                    {
                        game.thePlayer.thePlayerFire.Remove(game.thePlayer.thePlayerFire[shotNumber]);
                        explode(game, lvl, i);
                        mines.Remove(mines[i]);
                        return;
                    }
                }

                //Destroy mines outside screen
                if (mines[i].vertex.y > 784)
                    mines.Remove(mines[i]);
            }

        }

        //Draw mines and explosions
        public void draw(SpriteBatch sb, Texture2D mine, Texture2D explosion)
        {
            for (int i = 0; i < mines.Count; i++)
            {
                drawSprite(sb, mine, mines[i].vertex - new Vectors(mineRadius, mineRadius));
            }

        }

        void drawSprite(SpriteBatch sb, Texture2D texture, Vectors vect)
        {
            sb.Draw(texture, new Rectangle((int)vect.x, (int)vect.y, texture.Width, texture.Height), Color.White);
        }

        public void explode(Game1 game, Levels lvl, int mineIndex)
        {
            //add explosions

            lvl.addExplosion(mines[mineIndex].vertex - new Vectors(mineRadius, mineRadius),Color.Red);
            lvl.addExplosion(mines[mineIndex].vertex - new Vectors(mineRadius, mineRadius + 128), Color.Red);
            lvl.addExplosion(mines[mineIndex].vertex - new Vectors(mineRadius, mineRadius - 128), Color.Red);
            lvl.addExplosion(mines[mineIndex].vertex - new Vectors(mineRadius + 128, mineRadius), Color.Red);
            lvl.addExplosion(mines[mineIndex].vertex - new Vectors(mineRadius - 128, mineRadius), Color.Red);

            for (int p = 0; p < 4; p++)
            {
                lvl.addExplosion(mines[mineIndex].vertex - new Vectors(mineRadius + rnd.Next(-128, 128), mineRadius + rnd.Next(-128, 128)), Color.Red);
            }

            game.theScore.score += 25;
            mines[mineIndex].radius += 128;

            //Check for rocks
            for (int i = 0; i < lvl.Rocks.rocks.Count; i++)
            {
                if (Circles.overlap(mines[mineIndex],
                    new Circles(lvl.Rocks.rocks[i].position.X + 64, lvl.Rocks.rocks[i].position.Y + 64, 0, 64)))
                {
                    lvl.Rocks.rocks.Remove(lvl.Rocks.rocks[i]);
                }
            }

            //Check for guns
            for (int i = 0; i < lvl.guns.guns.Count; i++)
            {
                if (Circles.overlap(mines[mineIndex], new Circles(lvl.guns.guns[i].gunLocation, 32)))
                {
                    lvl.guns.guns.Remove(lvl.guns.guns[i]);
                }
            }

            //Check for Enemies
            for (int i = 0; i < game.theEnemy.Count; i++)
            {
                if (Circles.overlap(mines[mineIndex], new Circles(game.theEnemy[i].enemyPosition.X + 64, game.theEnemy[i].enemyPosition.Y + 64, 0, 64)))
                {
                    game.theEnemy.Remove(game.theEnemy[i]);
                }
            }

            //Check for player
            Circles player = new Circles(new Vectors(game.thePlayer.playerPosition.X + 64, game.thePlayer.playerPosition.Y + 64), 64);
            if (Circles.overlap(player, mines[mineIndex]))
            {
                game.thePlayerHealth.checkAttack = true;
                game.thePlayerHealth.rumble = true;

            }
        }

        public static float getWaveOffset(float currentPosition, float speed, float maxOffset)
        {
            return maxOffset * (float)Math.Sin(currentPosition + speed);
        }
    }
}