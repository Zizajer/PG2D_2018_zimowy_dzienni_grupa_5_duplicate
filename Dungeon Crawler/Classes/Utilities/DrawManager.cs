using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dungeon_Crawler
{
    public class DrawManager
    {
        private Checkbox checkbox;
        private Texture2D mag;
        private Texture2D warrior;
        private Texture2D ranger;
        private Texture2D button;
        private Texture2D gameLogo;
        private Texture2D controls;
        private Texture2D checkboxT;
        private SpriteFont buttonFont;

        private Inputbox inputbox;
        private Game1 game;

        private List<Button> heroChooseButtons;
        private List<Button> MainMenuButtons;
        private List<Button> HelpMenuButtons;
        private List<Button> AboutMenuButtons;

        public DrawManager(ContentManager Content, Game1 game)
        {
            this.game = game;
            mag = Content.Load<Texture2D>("Arts/mage");
            warrior = Content.Load<Texture2D>("Arts/warrior");
            ranger = Content.Load<Texture2D>("Arts/ranger");
            checkboxT = Content.Load<Texture2D>("Controls/checkbox");
            button = Content.Load<Texture2D>("Controls/button");
            gameLogo = Content.Load<Texture2D>("Arts/DungeonCrawlerLogo");
            controls = Content.Load<Texture2D>("Arts/Controls");
            buttonFont = Content.Load<SpriteFont>("fonts/Chiller");

            inputbox = new Inputbox(button, buttonFont)
            {
                Position = new Vector2(675, 475)
            };

            checkbox = new Checkbox(checkboxT, buttonFont)
            {
                Position = new Vector2(675, 575)
            };
            checkbox.Click += checkbox_Click;

            var magButton = new Button(button, buttonFont)
            {
                Position = new Vector2(300, 350),
                Text = "Mage"
            };

            magButton.Click += magButton_Click;

            var warriorButton = new Button(button, buttonFont)
            {
                Position = new Vector2(575, 350),
                Text = "Warrior",
            };

            warriorButton.Click += warriorButton_Click;

            var rangerButton = new Button(button, buttonFont)
            {
                Position = new Vector2(850, 350),
                Text = "Ranger",
            };

            rangerButton.Click += rangerButton_Click;

            var quitButton = new Button(button, buttonFont)
            {
                Position = new Vector2(575, 725),
                Text = "Go Back",
            };

            quitButton.Click += quitButton_Click;

            heroChooseButtons = new List<Button>()
                {
                    magButton,
                    warriorButton,
                    rangerButton,
                    quitButton,
                };

            var startGameButton = new Button(button, buttonFont)
            {
                Position = new Vector2(575, 500),
                Text = "Start Game",
            };

            startGameButton.Click += StartGameButton_Click;

            var helpButton = new Button(button, buttonFont)
            {
                Position = new Vector2(575, 575),
                Text = "How to Play",
            };

            helpButton.Click += HelpButton_Click;

            var aboutGameButton = new Button(button, buttonFont)
            {
                Position = new Vector2(575, 650),
                Text = "About",
            };

            aboutGameButton.Click += AboutGameButton_Click;

            var quitGameButton = new Button(button, buttonFont)
            {
                Position = new Vector2(575, 725),
                Text = "Exit",
            };

            quitGameButton.Click += QuitGameButton_Click;

            MainMenuButtons = new List<Button>()
                {
                    startGameButton,
                    helpButton,
                    aboutGameButton,
                    quitGameButton,
                };

            var goBackFromHelpButton = new Button(button, buttonFont)
            {
                Position = new Vector2(575, 725),
                Text = "Go back",
            };

            goBackFromHelpButton.Click += goBackFromHelpButton_Click;

            HelpMenuButtons = new List<Button>()
                {
                    goBackFromHelpButton,
                };

            var goBackFromAboutButton = new Button(button, buttonFont)
            {
                Position = new Vector2(575, 725),
                Text = "Go back",
            };

            goBackFromAboutButton.Click += GoBackFromAboutButton_Click;

            AboutMenuButtons = new List<Button>()
                {
                    goBackFromAboutButton,
                };


        }

        private void GoBackFromAboutButton_Click(object sender, EventArgs e)
        {
            Global.CurrentGameState = 0;
        }

        private void goBackFromHelpButton_Click(object sender, EventArgs e)
        {
            Global.CurrentGameState = 0;
        }

        private void QuitGameButton_Click(object sender, EventArgs e)
        {
            game.Exit();
        }

        private void AboutGameButton_Click(object sender, EventArgs e)
        {
            Global.CurrentGameState = Global.Gamestates.isAboutMenu;
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            Global.CurrentGameState = Global.Gamestates.isHelpMenu;
        }

        private void StartGameButton_Click(object sender, EventArgs e)
        {
            Global.CurrentGameState = Global.Gamestates.isHeroChooseMenu;
        }

        void rangerButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Global.playerName))
                Global.playerName = "Player";
            Global.playerName = ToTitleCase(Global.playerName);
            Global.playerClass = ("Ranger");
            Global.levelmanager.setPlayer("Ranger");
            Global.CombatManager.setPLayer();
            Global.CurrentGameState = Global.Gamestates.isGameActive;
        }


        void warriorButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Global.playerName))
                Global.playerName = "Player";
            Global.playerName = ToTitleCase(Global.playerName);
            Global.playerClass = ("Warrior");
            Global.levelmanager.setPlayer("Warrior");
            Global.CombatManager.setPLayer();
            Global.CurrentGameState = Global.Gamestates.isGameActive;
        }

        void magButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Global.playerName))
                Global.playerName = "Player";
            Global.playerName = ToTitleCase(Global.playerName);
            Global.playerClass = ("Mage");
            Global.levelmanager.setPlayer("Mage");
            Global.CombatManager.setPLayer();
            Global.CurrentGameState = Global.Gamestates.isGameActive;
        }

        void checkbox_Click(object sender, EventArgs e)
        {
            Global.hardMode = !Global.hardMode;
        }

        void quitButton_Click(object sender, EventArgs e)
        {
            Global.CurrentGameState = Global.Gamestates.isMainMenu;
        }

        public void UpdateMainMenu(GameTime gameTime)
        {
            foreach (Button button in MainMenuButtons)
                button.Update(gameTime);   
        }

        public void UpdateHelpMenu(GameTime gameTime)
        {
            foreach (Button button in HelpMenuButtons)
                button.Update(gameTime);
        }

        public void UpdateAboutMenu(GameTime gameTime)
        {
            foreach (Button button in AboutMenuButtons)
                button.Update(gameTime);
        }

        public void UpdateChooseHeroMenu(GameTime gameTime)
        {
            foreach (Button button in heroChooseButtons)
                button.Update(gameTime);
            checkbox.Update(gameTime);
            inputbox.Update(gameTime);
        }

        public void DrawMainMenu(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, GameTime gameTime)
        {
            graphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            spriteBatch.Draw(gameLogo, new Rectangle(400, 0, gameLogo.Width/2, gameLogo.Height/2), Color.White);

            foreach (Button button in MainMenuButtons)
            {
                button.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();

        }

        public void DrawHelpMenu(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, GameTime gameTime)
        {
            graphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            spriteBatch.Draw(controls, new Rectangle(0, 0, controls.Width, controls.Height), Color.White);

            foreach (Button button in HelpMenuButtons)
            {
                button.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();

        }

        public void DrawAboutMenu(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, GameTime gameTime)
        {
            graphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            spriteBatch.DrawString(buttonFont, "Cezary Witkowski - Project Leader/Developer/Merge Master", new Vector2(315, 325), Color.Black);

            spriteBatch.DrawString(buttonFont, "Marcin Kapelan - Developer", new Vector2(315, 365), Color.Black);

            spriteBatch.DrawString(buttonFont, "Piotr Sadza - Graphic design/Sound design", new Vector2(315, 405), Color.Black);

            foreach (Button button in AboutMenuButtons)
            {
                button.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();

        }

        public void DrawChooseHeroMenu(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, GameTime gameTime)
        {
            graphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            spriteBatch.DrawString(buttonFont, "Choose Character", new Vector2(550, 10), Color.Black);

            int width = 200, height = 300;

            spriteBatch.Draw(mag, new Rectangle(275, 50, width, height), Color.White);

            spriteBatch.Draw(warrior, new Rectangle(550, 50, width, height), Color.White);

            spriteBatch.Draw(ranger, new Rectangle(825, 50, width, height), Color.White);
            string s;
            if (Global.hardMode)
            {
                s = "Difficulty: Hard";
            }
            else
            {
                s = "Difficulty: Easy";
            }
            spriteBatch.DrawString(buttonFont, s, new Vector2(535, 587), Color.Black);

            spriteBatch.DrawString(buttonFont, "Enter your name", new Vector2(535, 487), Color.Black);
            inputbox.Draw(gameTime,spriteBatch);

            foreach (Button button in heroChooseButtons)
            {
                button.Draw(gameTime, spriteBatch);
            }

            checkbox.Draw(gameTime, spriteBatch);
            spriteBatch.End();

        }
        public string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }
    }
}
