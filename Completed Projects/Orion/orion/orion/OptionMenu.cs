using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace orion
{
    public class OptionMenu
    {
        Texture2D buttonSprite;
        Texture2D selectorSprite;
        SpriteFont font;
        Vector2 startPosition;
        Vector2 textOffset = new Vector2(30, 10);
        int buttonSpacing;
        int buttonWidth, buttonHeight;
        bool fading = true;
        int alpha = 255;

        Game1 game;
        public int selector = 0;
        public int difficulty = 1;
        public bool showOptions = false;

        string[][] buttonNames;
        string[] mainMenu =       {"Setting", "Sound", "Music", "Back" };   //4 options
        string[] difficultyMenu = {"Easy" , "Normal", "Hard", "Insane"};    //4 options
        string[] soundMusicMenu = { "On", "Off", "Back"};                   //2 options
        int menuLevel = 0;

        public OptionMenu(Game1 gameRef)
        {
            //Insert button names in 2D array
            buttonNames = new string[4][];
            buttonNames[0] = mainMenu;          //Main option menu
            buttonNames[1] = difficultyMenu;    //Difficulty menu
            buttonNames[2] = soundMusicMenu;    //Sound menu
            buttonNames[3] = soundMusicMenu;    //Music menu

            buttonWidth = 180;
            buttonHeight = 90;
            game = gameRef;
            buttonSprite = game.Content.Load<Texture2D>("ButtonBlank");
            selectorSprite = game.Content.Load<Texture2D>("selector");
            font = game.Content.Load<SpriteFont>("MainMenuFont");
            startPosition = new Vector2(selectorSprite.Width + 50, 720 - buttonHeight - 10);
            buttonSpacing = selectorSprite.Width + 32;
        }
        public void Draw(SpriteBatch sb)
        {
            Vector2 drawingPosition = new Vector2(startPosition.X, startPosition.Y);
            Color color;

            //Draw each button on current menu
            for (int i = 0; i < buttonNames[menuLevel].Length; i++)
            {
                if (selector == i)
                {
                    //Draw selector
                    sb.Draw(selectorSprite, drawingPosition + new Vector2(-selectorSprite.Width, 28), new Color(alpha,alpha,alpha,alpha));
                    color = Color.Gray;  //Draw dark button
                }
                else
                    color = Color.White; //Draw normal button

                //Draw button
                sb.Draw(buttonSprite, new Rectangle((int)drawingPosition.X, (int)drawingPosition.Y, buttonWidth,buttonHeight), color);
                //draw Text
                sb.DrawString(font, buttonNames[menuLevel][i], drawingPosition + textOffset, Color.Red);
                //Adjust drawing position
                drawingPosition.X += buttonWidth + buttonSpacing;
            }
        }
        public void Update() 
        {
            //Flash alpha value
            if (fading)
            {
                alpha -= 15;
                if (alpha <= 0)
                {
                    fading = false;
                    alpha = 0;
                }
            }
            else
            {
                alpha += 15;
                if (alpha >= 255)
                {
                    alpha = 255;
                    fading = true;
                }
            }
        }
        public void MoveCursorLeft()
        {
            selector--;
            if (selector < 0) selector = buttonNames[menuLevel].Length - 1;
        }
        public void MoveCursorRight()
        {
            selector++;
            if (selector > buttonNames[menuLevel].Length - 1) selector = 0;
        }
        void SelectMainOption()
        {
            switch (selector)
                {
                    case 0:
                        menuLevel = 1;
                        break;
                    case 1:
                        menuLevel = 2;
                        break;
                    case 2:
                        menuLevel = 3;
                        break;
                    case 3:
                        menuLevel = 0;
                        selector = 0;
                        game.gameMenu = true;
                        game.mainMenu.menuStart = true;
                        game.mainMenu.cursorSelector = 0;
                        game.gameOptions = false;
                        break;
                }

            selector = 0;
        }
        void SelectDifficultyOption()
        {
            switch (selector)
            {
                case 0:                 //Easy
                    difficulty = 0;
                    menuLevel = 0;
                    break;
                case 1:                 //Normal
                    difficulty = 1;
                    menuLevel = 0;
                    break;
                case 2:                 //Hard
                    difficulty = 2;
                    menuLevel = 0;
                    break;
                case 3:                 //Insane
                    difficulty = 3;
                    menuLevel = 0;
                    break;
            }

            selector = 0;
        }
        void SelectSoundOption()
        {
            switch (selector)
            {
                case 0:                 //On
                    game.audio.soundOn = true;
                    menuLevel = 0;
                    break;
                case 1:                 //Off
                    game.audio.soundOn = false;
                    menuLevel = 0;
                    break;
                case 2:                 //Back
                    menuLevel = 0;
                    break;
            }

            selector = 0;
        }
        void SelectMusicOption()
        {
            switch (selector)
            {
                case 0: 
                case 1:                 //On / Off
                    game.audio.toggleMusic();
                    menuLevel = 0;
                    break;
                case 2:                 //Back
                    menuLevel = 0;
                    break;
            }

            selector = 0;
        }
        public void MenuSelect()
        {
            switch (menuLevel)
            {
                case 0: SelectMainOption(); break;          //Main option menu
                case 1: SelectDifficultyOption(); break;    //Difficulty menu
                case 2: SelectSoundOption(); break;         //Sound menu
                case 3: SelectMusicOption(); break;         //Music menu
            }
        }
    }
}
