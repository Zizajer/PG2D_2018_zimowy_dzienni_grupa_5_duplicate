using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dungeon_Crawler
{
    public class DrawManager
    {

        private Texture2D mag;
        private Texture2D warrior;
        private Texture2D ranger;
        private Texture2D button;
        private Texture2D gameLogo;
        private Texture2D controls;
        private SpriteFont buttonFont;
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
            button = Content.Load<Texture2D>("Controls/button");
            gameLogo = Content.Load<Texture2D>("Arts/DungeonCrawlerLogo");
            controls = Content.Load<Texture2D>("Arts/Controls");
            buttonFont = Content.Load<SpriteFont>("fonts/ButtonFont");

            var magButton = new Button(button, buttonFont)
            {
                Position = new Vector2(50, 350),
                Text = "Mag",
            };

            magButton.Click += magButton_Click;

            var warriorButton = new Button(button, buttonFont)
            {
                Position = new Vector2(575, 300),
                Text = "Warrior",
            };

            warriorButton.Click += warriorButton_Click;

            var rangerButton = new Button(button, buttonFont)
            {
                Position = new Vector2(1100, 350),
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
            Global.GameStates[4] = false;
        }

        private void goBackFromHelpButton_Click(object sender, EventArgs e)
        {
            Global.GameStates[3] = false;
        }

        private void QuitGameButton_Click(object sender, EventArgs e)
        {
            game.Exit();
        }

        private void AboutGameButton_Click(object sender, EventArgs e)
        {
            Global.GameStates[4] = true;
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            Global.GameStates[3] = true;
        }

        private void StartGameButton_Click(object sender, EventArgs e)
        {
            Global.GameStates[2] = true;
        }

        void rangerButton_Click(object sender, EventArgs e)
        {

            Global.playerClass = ("Ranger");
            Global.levelmanager.setPlayer("Ranger");
            Global.CombatManager.setPLayer();
            Global.GameStates[1] = true;
        }

        void warriorButton_Click(object sender, EventArgs e)
        {
            Global.playerClass = ("Warrior");
            Global.levelmanager.setPlayer("Warrior");
            Global.CombatManager.setPLayer();
            Global.GameStates[1] = true;
        }

        void magButton_Click(object sender, EventArgs e)
        {
            Global.playerClass = ("Mage");
            Global.levelmanager.setPlayer("Mage");
            Global.CombatManager.setPLayer();
            Global.GameStates[1] = true;
        }

        void quitButton_Click(object sender, EventArgs e)
        {
            Global.GameStates[2] = false;
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
        }

        public void DrawMainMenu(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, GameTime gameTime)
        {
            graphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            spriteBatch.Draw(gameLogo, new Rectangle(400, 0, gameLogo.Width/2, gameLogo.Height/2), Color.White);

           // spriteBatch.DrawString(buttonFont, "Dungeon Crawler", new Vector2(575, 405), Color.Black);

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

            spriteBatch.DrawString(buttonFont, "Cezary Witkowski - Developer", new Vector2(575, 405), Color.Black);

            spriteBatch.DrawString(buttonFont, "Marcin Kapelan - Developer", new Vector2(575, 365), Color.Black);

            spriteBatch.DrawString(buttonFont, "Piotr Sadza - Grafik/Dzwiekowiec", new Vector2(575, 325), Color.Black);

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

            spriteBatch.Draw(mag, new Rectangle(50, 50, mag.Width, mag.Height), Color.White);

            spriteBatch.Draw(warrior, new Rectangle(575, 50, warrior.Width, warrior.Height), Color.White);

            spriteBatch.Draw(ranger, new Rectangle(1100, 50, ranger.Width, ranger.Height), Color.White);

            foreach (Button button in heroChooseButtons)
            {
                button.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();

        }
    }
}
