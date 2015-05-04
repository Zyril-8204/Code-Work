using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace orion
{
    class ToolBar
    {
        //Position of tool bar's components
        Vectors barOrigin = new Vectors(0, 0);
        Vectors HealthOrigin = new Vectors(8, 0);
        Vectors bossHealthOrigin = new Vectors(8, 700);
        Vectors LivesOrigin = new Vectors(384, 0);
        Vectors rapidOrigin = new Vectors(896, 0);
        Vectors spreadOrigin = new Vectors(960, 0);
        Vectors rocketOrigin = new Vectors(1024, 0);
        Vectors nukeOrigin = new Vectors(1152, 0);
        Vectors scoreOrigin = new Vectors(10, 74);

        int maxLives = 8;       //max number of lives that can be drawn to the screen
        int healthWidth = 360;  //Witdth of health bar
        int bossHealthWidth = 1264;

        //Icon textures
        Texture2D bar;
        Texture2D pixel;
        Texture2D lifeIcon;
        Texture2D rapidIcon;
        Texture2D spreadIcon;
        Texture2D rocketIcon;
        Texture2D nukeIcon;

        SpriteFont font;        //Font for drawing number of rockets or time left with power up

        //Load textures and font with a constructor
        public ToolBar(Game1 game)
        {
            bar = game.Content.Load<Texture2D>("ToolBar");
            pixel = game.Content.Load<Texture2D>("pixel");
            lifeIcon = game.Content.Load<Texture2D>("ExtraLife");
            rapidIcon = game.Content.Load<Texture2D>("RapidPU");
            spreadIcon = game.Content.Load<Texture2D>("SpreadShot");
            rocketIcon = game.Content.Load<Texture2D>("Rockets");
            nukeIcon = game.Content.Load<Texture2D>("Nuke");

            font = game.Content.Load<SpriteFont>("MainMenuFont");
        }

        //Draws the menu
        public void draw(SpriteBatch sb, Game1 game)
        {
            drawRect(sb, HealthOrigin, healthWidth, 64, Color.Red);                             //Background for healthbar
            float percent = (float)game.thePlayerHealth.theCurrentHealth * .01f;               //Figures the length of the healthbar
            drawRect(sb, HealthOrigin, (int)(healthWidth * percent), 64, Color.LimeGreen);      //Draws healthbar
            drawSprite(sb, bar, barOrigin);                                                     //Draws tool bar
            drawLives(sb, LivesOrigin, 64, game.playerLives, maxLives);                         //Draw lives as sprites
            if (game.thePlayer.rapidGun) drawSprite(sb, rapidIcon, rapidOrigin);                //If rapidPU, show rapid icon
            if (game.thePlayer.spreadGun) drawSprite(sb, spreadIcon, spreadOrigin);             //If spreadPU, show spread icon
            if (game.thePlayer.missiles)                                                        //If rockets, show rockets
            {
                drawSprite(sb, rocketIcon, rocketOrigin);
                //Show missile value here
                //drawValue(sb, font, rocketOrigin + new Vectors(64, 0), numberOfMissiles);
            }
            if (game.thePlayer.nuke > 0)
            {
                drawSprite(sb, nukeIcon, nukeOrigin);
                drawValue(sb, font, nukeOrigin + new Vectors(64, 0), game.thePlayer.nuke);
            }

            drawValue(sb, font, scoreOrigin, game.theScore.score);

            //Draw boss health bar
            if (game.boss1Active || game.boss2Active || game.boss3Active || game.boss4Active)
            {
                drawRect(sb, bossHealthOrigin, bossHealthWidth, 8, Color.Red); //Background for healthbar

                float percent2 = 1; //Calculate percent based on boss' current health and max health
                if (game.boss1Active) percent2 = game.Boss1.health / game.Boss1.maxHealth;
                if (game.boss2Active) percent2 = game.Boss2.health / game.Boss2.maxHealth;
                if (game.boss3Active) percent2 = game.Boss3.health / game.Boss3.maxHealth;
                if (game.boss4Active) percent2 = game.Boss4.health / game.Boss4.maxHealth;

                drawRect(sb, bossHealthOrigin, (int)(bossHealthWidth * percent2), 8, Color.LimeGreen); //Draws healthbar
            }
        }

        //Draws a sprite
        void drawSprite(SpriteBatch sb, Texture2D texture, Vectors vect)
        {
            sb.Draw(texture, new Rectangle((int)vect.x, (int)vect.y, texture.Width, texture.Height), Color.White);
        }

        //Draws a solid rectangle
        void drawRect(SpriteBatch sb, Vectors origin, int BoxWidth, int BoxHeight, Color color)
        {
            sb.Draw(pixel, new Rectangle((int)origin.x, (int)origin.y, BoxWidth, BoxHeight), color);
        }

        //Draws lives as sprites
        void drawLives(SpriteBatch sb, Vectors startLocation, int spacing, int currentValue, int maxNum)
        {
            for (int index = 0; index < currentValue && index < maxNum; index++)
            {
                Vectors position = new Vectors(startLocation.x + spacing * index, startLocation.y);
                drawSprite(sb, lifeIcon, position);
            }
        }

        //draws a values as text to the screen
        void drawValue(SpriteBatch sb, SpriteFont spriteFont, Vectors position, int value)
        {
            sb.DrawString(spriteFont, value.ToString(), new Vector2(position.x, position.y), Color.White);
        }

    }
}