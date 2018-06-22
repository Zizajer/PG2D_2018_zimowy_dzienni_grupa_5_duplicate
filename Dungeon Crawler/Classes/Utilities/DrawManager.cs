using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
        private SpriteFont smallButtonFont;

        private Inputbox inputbox;
        private Game1 game;
        private LevelManager lm;
        private List<Button> heroChooseButtons;
        private List<Button> MainMenuButtons;
        private List<Button> HelpMenuButtons;
        private List<Button> AboutMenuButtons;
        private List<Button> ClassSpecificButtons;

        private int screenWidth;
        private int screenHeight;

        public DrawManager(ContentManager Content, Game1 game, LevelManager lm, int screenWidth, int screenHeight)
        {
            this.lm = lm;
            this.game = game;
            mag = Content.Load<Texture2D>("Arts/mage");
            warrior = Content.Load<Texture2D>("Arts/warrior");
            ranger = Content.Load<Texture2D>("Arts/ranger");
            checkboxT = Content.Load<Texture2D>("Controls/checkbox");
            button = Content.Load<Texture2D>("Controls/button");
            gameLogo = Content.Load<Texture2D>("Arts/DungeonCrawlerLogo");
            controls = Content.Load<Texture2D>("Arts/Controls");
            buttonFont = Content.Load<SpriteFont>("fonts/Chiller");
            smallButtonFont = Content.Load<SpriteFont>("fonts/smallChiller");

            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            CreateButtons();

        }

        private void GoBackFromAboutButton_Click(object sender, EventArgs e)
        {
            Global.CurrentGameState = Global.Gamestates.isMainMenu;
        }

        private void goBackFromHelpButton_Click(object sender, EventArgs e)
        {
            Global.CurrentGameState = Global.Gamestates.isMainMenu;
        }

        private void goBackFromClassSpecificButton_Click(object sender, EventArgs e)
        {
            Global.CurrentGameState = Global.Gamestates.isHeroChooseMenu;
        }

        private void startAdventureButton_Click(object sender, EventArgs e)
        {
            Global.SoundManager.launchGameClick.Play();
            //TO-DO add here second song for loop play
            if(Global.playerClass=="Mage")
                Global.levelmanager.setPlayer("Mage");
            else if (Global.playerClass == "Warrior")
                Global.levelmanager.setPlayer("Warrior");
            else
                Global.levelmanager.setPlayer("Ranger");
            Global.CombatManager.setPLayer();
            Global.CurrentGameState = Global.Gamestates.isGameActive;
        }

        private void randomNameButton_Click(object sender, EventArgs e)
        {
            Global.playerName = getRandomName(Global.playerClass);
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
            Global.playerClass = ("Ranger");
            Global.playerName = getRandomName(Global.playerClass);
            Global.CurrentGameState = Global.Gamestates.isClassSpecificMenu;
        }

        void warriorButton_Click(object sender, EventArgs e)
        {
            Global.playerClass = ("Warrior");
            Global.playerName = getRandomName(Global.playerClass);
            Global.CurrentGameState = Global.Gamestates.isClassSpecificMenu;
        }

        void magButton_Click(object sender, EventArgs e)
        {
            Global.playerClass = ("Mage");
            Global.playerName = getRandomName(Global.playerClass);
            Global.CurrentGameState = Global.Gamestates.isClassSpecificMenu;
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
        }

        public void UpdateClassSpecificMenu(GameTime gameTime)
        {
            foreach (Button button in ClassSpecificButtons)
            {
                button.Update(gameTime);
            }
            inputbox.Update(gameTime);
        }

        public void DrawMainMenu(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, GameTime gameTime)
        {
            graphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            spriteBatch.Draw(gameLogo, new Rectangle(screenWidth/2 - gameLogo.Width / 4, 0, gameLogo.Width/2, gameLogo.Height/2), Color.White);

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

            string s;
            s = "Game Authors:";
            spriteBatch.DrawString(buttonFont, s, new Vector2(screenWidth / 2 - buttonFont.MeasureString(s).Length() / 2, 200), Color.Black);
            s = "Cezary Witkowski - Project Leader/Developer/Merge Master";
            spriteBatch.DrawString(buttonFont, s, new Vector2(screenWidth / 2 - buttonFont.MeasureString(s).Length() / 2, 275), Color.Black);
            s = "Marcin Kapelan - Developer";
            spriteBatch.DrawString(buttonFont, s, new Vector2(screenWidth / 2 - buttonFont.MeasureString(s).Length() / 2, 359), Color.Black);
            s = "Piotr Sadza - Graphic design/Sound design";
            spriteBatch.DrawString(buttonFont, s, new Vector2(screenWidth / 2 - buttonFont.MeasureString(s).Length() / 2, 425), Color.Black);

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
            string s= "Choose Character";
            spriteBatch.DrawString(buttonFont, s, new Vector2(screenWidth / 2 - buttonFont.MeasureString(s).Length() / 2, 10), Color.Black);

            spriteBatch.Draw(mag, new Rectangle(screenWidth / 3 - mag.Width / 2, 100, mag.Width, mag.Height), Color.White);

            spriteBatch.Draw(warrior, new Rectangle(screenWidth / 2 - warrior.Width / 2, 100, warrior.Width, warrior.Height), Color.White);

            spriteBatch.Draw(ranger, new Rectangle(screenWidth * 2 / 3 - ranger.Width / 2, 100, ranger.Width, ranger.Height), Color.White);
            
            if (Global.hardMode)
            {
                s = "Difficulty: Hard";
            }
            else
            {
                s = "Difficulty: Easy";
            }

            spriteBatch.DrawString(buttonFont, s, new Vector2(screenWidth / 2 - buttonFont.MeasureString(s).Length() / 2 - checkbox.Rectangle.Width, 587), Color.Black);

            foreach (Button button in heroChooseButtons)
            {
                button.Draw(gameTime, spriteBatch);
            }

            checkbox.Draw(gameTime, spriteBatch);
            spriteBatch.End();

        }

        public void DrawClassSpecificMenu(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, GameTime gameTime)
        {
            graphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            string s = "Enter your hero name";
            spriteBatch.DrawString(buttonFont, s, new Vector2(screenWidth / 2 - buttonFont.MeasureString(s).Length() / 2, 10), Color.Black);

            if(Global.playerClass == "Mage")
            {
                spriteBatch.Draw(mag, new Rectangle(screenWidth / 2 - mag.Width / 2, 100, mag.Width, mag.Height), Color.White);
            }
            else if(Global.playerClass == "Warrior")
            {
                spriteBatch.Draw(warrior, new Rectangle(screenWidth / 2 - warrior.Width / 2, 100, warrior.Width, warrior.Height), Color.White);
            }
            else
            {
                spriteBatch.Draw(ranger, new Rectangle(screenWidth / 2 - ranger.Width / 2, 100, ranger.Width, ranger.Height), Color.White);
            }

            inputbox.Draw(gameTime, spriteBatch);

            foreach (Button button in ClassSpecificButtons)
            {
                button.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();

        }
        public static string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        private string getRandomName(string playerClass)
        {
            if (playerClass == "Mage")
            {
                return ToTitleCase(lm.MageNamesList[Global.random.Next(lm.MageNamesList.Count)]);
            }
            else if (playerClass == "Warrior")
            {
                return ToTitleCase(lm.WarriorNamesList[Global.random.Next(lm.WarriorNamesList.Count)]);
            }
            else if (playerClass == "Ranger")
            {
                return ToTitleCase(lm.RangerNamesList[Global.random.Next(lm.RangerNamesList.Count)]);
            }
            else
            {
                return "Player";
            }
        }

        private void CreateButtons()
        {
            inputbox = new Inputbox(button, buttonFont)
            {
                Position = new Vector2(screenWidth / 2 - button.Width / 2, 420)
            };

            checkbox = new Checkbox(checkboxT, buttonFont)
            {
                Position = new Vector2(750, 600)
            };
            checkbox.Click += checkbox_Click;

            var magButton = new Button(button, buttonFont)
            {
                Position = new Vector2(screenWidth / 3 - button.Width / 2, 420),
                Text = "Mage"
            };

            magButton.Click += magButton_Click;

            var warriorButton = new Button(button, buttonFont)
            {
                Position = new Vector2(screenWidth / 2 - button.Width / 2, 420),
                Text = "Warrior",
            };

            warriorButton.Click += warriorButton_Click;

            var rangerButton = new Button(button, buttonFont)
            {
                Position = new Vector2(screenWidth * 2 / 3 - button.Width / 2, 420),
                Text = "Ranger",
            };

            rangerButton.Click += rangerButton_Click;

            var quitButton = new Button(button, buttonFont)
            {
                Position = new Vector2(screenWidth / 2 - button.Width / 2, 700),
                Text = "Go back",
            };

            quitButton.Click += quitButton_Click;

            heroChooseButtons = new List<Button>()
                {
                    magButton,
                    warriorButton,
                    rangerButton,
                    quitButton,
                };

            //main menu
            var startGameButton = new Button(button, buttonFont)
            {
                Text = "Start Game",
                Position = new Vector2(screenWidth / 2 - button.Width / 2, 475)
            };

            startGameButton.Click += StartGameButton_Click;

            var helpButton = new Button(button, buttonFont)
            {
                Text = "How to Play",
                Position = new Vector2(screenWidth / 2 - button.Width / 2, 550)
            };

            helpButton.Click += HelpButton_Click;

            var aboutGameButton = new Button(button, buttonFont)
            {
                Text = "About",
                Position = new Vector2(screenWidth / 2 - button.Width / 2, 625)
            };

            aboutGameButton.Click += AboutGameButton_Click;

            var quitGameButton = new Button(button, buttonFont)
            {
                Text = "Exit",
                Position = new Vector2(screenWidth / 2 - button.Width / 2, 700)
            };

            quitGameButton.Click += QuitGameButton_Click;

            MainMenuButtons = new List<Button>()
                {
                    startGameButton,
                    helpButton,
                    aboutGameButton,
                    quitGameButton,
                };

            //main menu
            var goBackFromHelpButton = new Button(button, buttonFont)
            {
                Position = new Vector2(screenWidth / 2 - button.Width / 2, 700),
                Text = "Go back",
            };

            goBackFromHelpButton.Click += goBackFromHelpButton_Click;

            HelpMenuButtons = new List<Button>()
                {
                    goBackFromHelpButton,
                };

            var goBackFromAboutButton = new Button(button, buttonFont)
            {
                Position = new Vector2(screenWidth / 2 - button.Width / 2, 700),
                Text = "Go back",
            };

            goBackFromAboutButton.Click += GoBackFromAboutButton_Click;

            AboutMenuButtons = new List<Button>()
                {
                    goBackFromAboutButton,
                };

            var goBackFromClassSpecificButton= new Button(button, buttonFont)
            {
                Position = new Vector2(screenWidth / 2 - button.Width / 2, 700),
                Text = "Go back",
            };
            goBackFromClassSpecificButton.Click += goBackFromClassSpecificButton_Click;

            var startAdventureButton = new Button(button, buttonFont)
            {
                Position = new Vector2(screenWidth / 2 - button.Width / 2, 587),
                Text = "Start Adventure!",
            };

            startAdventureButton.Click += startAdventureButton_Click;

            var randomNameButton = new Button(button, smallButtonFont)
            {
                Position = new Vector2(screenWidth / 2 - button.Width / 2, 470),
                Text = "Generate Name (you can also type your own)",
            };

            randomNameButton.Click += randomNameButton_Click;

            ClassSpecificButtons = new List<Button>()
                {
                    goBackFromClassSpecificButton,
                    startAdventureButton,
                    randomNameButton
                };
        }
    }
}
