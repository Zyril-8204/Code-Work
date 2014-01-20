using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace orion
{
    public class Rockets
    {

        Vectors firePosition = new Vectors();
        Enemy target;
        Vectors direction = new Vectors(0,5);
        public bool Visible = true;
        Game1 game;
        int smallestDistance = 3000;
        int fireSpeed = 10;
        Texture2D sprite;

        public static float rad2deg = 57.29577f;    //Converts radians to degrees
        public static float deg2rad = 0.01745329f;  //Converts degrees to radians

        float rotation = 0;

        public Rockets(Game1 game1)
        {
            game = game1;
            
            for (int i = game.theEnemy.Count - 1; i >= 0; i--)
            {
                if (getDistance(firePosition,
                    new Vectors(game.theEnemy[i].enemyPosition.X, game.theEnemy[i].enemyPosition.Y)) < smallestDistance)
                    target = game.theEnemy[i];
            }
            firePosition = new Vectors(game.thePlayer.playerPosition.X,game.thePlayer.playerPosition.Y);
        }

        public void Update()
        {

            if (game.theEnemy.Exists(EnemyExists))
            {
                Vectors temp = new Vectors(target.enemyPosition.X + 64, target.enemyPosition.Y + 64);
                rotation = (temp - firePosition).getHeading();
                direction.setPolar(fireSpeed, rotation);
            }
 
            firePosition = firePosition + direction;

            // if it's off the screen turn off visible
            if (firePosition.x < 0 || firePosition.x > 1280 || firePosition.y < 0 || firePosition.y > 800)
            {
                Visible = false;
            }
        }

        public bool EnemyExists(Enemy m)
        {
            if (m == target)
                return true;
            else return false;
        }

        float getDistance(Vectors v1, Vectors v2)
        {
            return (float)Math.Sqrt(Math.Pow(v2.x - v1.x, 2) + Math.Pow(v2.y - v1.y, 2));
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(sprite, new Rectangle((int)firePosition.x, (int)firePosition.y, sprite.Width, sprite.Height),
                new Rectangle(0, 0, sprite.Width, sprite.Height), Color.White, (rotation + 90) * deg2rad,
                new Vector2(sprite.Width / 2, sprite.Height / 2), SpriteEffects.None, 0.0f);
        }

        public void LoadContent()
        {
            sprite = game.Content.Load<Texture2D>("Rocket"); // loads the texture
        }

        public bool CheckCollision(Rectangle rect)
        {
            Rectangle firerect = new Rectangle((int)firePosition.x, (int)firePosition.y, 8, 8);
            return firerect.Intersects(rect);
        }

    }
}
