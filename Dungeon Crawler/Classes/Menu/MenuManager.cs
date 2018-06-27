using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon_Crawler
{
    class MenuManager
    {
        private Texture2D mag;
        private Texture2D warrior;
        private Texture2D ranger;
        private Texture2D button;
        private Texture2D gameLogo;
        private Texture2D controls;
        private Texture2D checkboxT;
        private SpriteFont buttonFont;
        private SpriteFont smallButtonFont;

        private MenuScreenBuilder menuScreenBuilder;

        public MenuScreen MainMenuScreen;
        public MenuScreen AboutMenuScreen;
        public MenuScreen ChooseHeroMenuScreen;
        public MenuScreen HowToPlayMenuScreen;
        public MenuScreen ClassSpecificMenuScreen;

        private Game1 game;
        private LevelManager lm;

        private int screenWidth;
        private int screenHeight;

        public MenuManager(ContentManager Content, Game1 game, LevelManager lm, int screenWidth, int screenHeight)
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
            this.menuScreenBuilder = new MenuScreenBuilder(Content, screenWidth, screenHeight);
            this.MainMenuScreen = createMainMenuScreen();
            this.AboutMenuScreen = createAboutMenuScreen();
            this.ChooseHeroMenuScreen = createChooseHeroMenuScreen();
            this.HowToPlayMenuScreen = createHowToPlayMenuScreen();
        }

        public MenuScreen createMainMenuScreen() {
            menuScreenBuilder.addButton(new Vector2(screenWidth / 2 - button.Width / 2, 475), "Start Game", StartGameButton_Click);
            menuScreenBuilder.addButton(new Vector2(screenWidth / 2 - button.Width / 2, 550), "How To Play", HelpButton_Click);
            menuScreenBuilder.addButton(new Vector2(screenWidth / 2 - button.Width / 2, 625), "About", AboutGameButton_Click);
            menuScreenBuilder.addButton(new Vector2(screenWidth / 2 - button.Width / 2, 700), "Exit", QuitGameButton_Click);
            menuScreenBuilder.addArt(new Rectangle(screenWidth / 2 - gameLogo.Width / 4, 0, gameLogo.Width / 2, gameLogo.Height / 2), gameLogo);
            return menuScreenBuilder.toBuild();
        }

        public MenuScreen createAboutMenuScreen() {
            menuScreenBuilder.addButton(new Vector2(screenWidth / 2 - button.Width / 2, 700), "Go Back", GoBackFromAboutButton_Click);
            menuScreenBuilder.addLabel(new Vector2(screenWidth / 2 - buttonFont.MeasureString("Game Authors:").Length() / 2, 200), "Game Authors:");
            menuScreenBuilder.addLabel(new Vector2(screenWidth / 2 - buttonFont.MeasureString("Cezary Witkowski - Project Leader/Developer/Merge Master").Length() / 2, 275), "Cezary Witkowski - Project Leader/Developer/Merge Master");
            menuScreenBuilder.addLabel(new Vector2(screenWidth / 2 - buttonFont.MeasureString("Marcin Kapelan - Developer").Length() / 2, 359), "Marcin Kapelan - Developer");
            menuScreenBuilder.addLabel(new Vector2(screenWidth / 2 - buttonFont.MeasureString("Piotr Sadza - Graphic design/Sound design").Length() / 2, 425), "Piotr Sadza - Graphic design/Sound design");
            return menuScreenBuilder.toBuild();
        }

        public MenuScreen createChooseHeroMenuScreen()
        {
            menuScreenBuilder.addButton(new Vector2(screenWidth / 3 - button.Width / 2, 420), "Mage", magButton_Click);
            menuScreenBuilder.addButton(new Vector2(screenWidth / 2 - button.Width / 2, 420), "Warrior", warriorButton_Click);
            menuScreenBuilder.addButton(new Vector2(screenWidth * 2 / 3 - button.Width / 2, 420), "Ranger", rangerButton_Click);
            menuScreenBuilder.addButton(new Vector2(screenWidth / 2 - button.Width / 2, 700), "Go Back", quitButton_Click);
            menuScreenBuilder.addLabel(new Vector2(screenWidth / 2 - buttonFont.MeasureString("Choose Character").Length() / 2, 10), "Choose Character");
            menuScreenBuilder.addArt(new Rectangle(screenWidth / 3 - mag.Width / 2, 100, mag.Width, mag.Height), mag);
            menuScreenBuilder.addArt(new Rectangle(screenWidth / 2 - warrior.Width / 2, 100, warrior.Width, warrior.Height), warrior);
            menuScreenBuilder.addArt(new Rectangle(screenWidth * 2 / 3 - ranger.Width / 2, 100, ranger.Width, ranger.Height), ranger);
            menuScreenBuilder.addCheckbox(new Vector2(1075, 600), checkbox_Click);
            menuScreenBuilder.addLabel(new Vector2(screenWidth / 2 - buttonFont.MeasureString("Hard Difficulty On/Off").Length() / 2 - 100, 587), "Hard Difficulty On/Off");
            return menuScreenBuilder.toBuild();
        }

        public MenuScreen createHowToPlayMenuScreen()
        {
            menuScreenBuilder.addButton(new Vector2(screenWidth / 2 - button.Width / 2, 700), "Go Back", goBackFromHelpButton_Click);
            menuScreenBuilder.addArt(new Rectangle(0, 0, controls.Width, controls.Height), controls);
            return menuScreenBuilder.toBuild();
        }

        public MenuScreen createClassSpecificMenuScreen()
        {
            menuScreenBuilder.addButton(new Vector2(screenWidth / 2 - button.Width / 2, 470), "Generate Name (you can also type your own)", randomNameButton_Click);
            menuScreenBuilder.addButton(new Vector2(screenWidth / 2 - button.Width / 2, 587), "Start Adventure!", startAdventureButton_Click);
            menuScreenBuilder.addButton(new Vector2(screenWidth / 2 - button.Width / 2, 700), "Go Back", goBackFromClassSpecificButton_Click);
            menuScreenBuilder.addLabel(new Vector2(screenWidth / 2 - buttonFont.MeasureString("Enter your hero name").Length() / 2, 10), "Enter your hero name");
            menuScreenBuilder.addInputbox(new Vector2(screenWidth / 2 - button.Width / 2, 420));

            if (Global.playerClass == "Mage")
            {
                menuScreenBuilder.addArt(new Rectangle(screenWidth / 2 - mag.Width / 2, 100, mag.Width, mag.Height), mag);
            }
            else if (Global.playerClass == "Warrior")
            {
                menuScreenBuilder.addArt(new Rectangle(screenWidth / 2 - warrior.Width / 2, 100, warrior.Width, warrior.Height), warrior);
            }
            else
            {
                menuScreenBuilder.addArt(new Rectangle(screenWidth / 2 - ranger.Width / 2, 100, ranger.Width, ranger.Height), ranger);
            }

            return menuScreenBuilder.toBuild();
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
            Global.SoundManager.playInGameSong();
            if (Global.playerClass == "Mage")
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
            this.ClassSpecificMenuScreen = createClassSpecificMenuScreen();
            Global.CurrentGameState = Global.Gamestates.isClassSpecificMenu;
        }

        void warriorButton_Click(object sender, EventArgs e)
        {
            Global.playerClass = ("Warrior");
            Global.playerName = getRandomName(Global.playerClass);
            this.ClassSpecificMenuScreen = createClassSpecificMenuScreen();
            Global.CurrentGameState = Global.Gamestates.isClassSpecificMenu;
        }

        void magButton_Click(object sender, EventArgs e)
        {
            Global.playerClass = ("Mage");
            Global.playerName = getRandomName(Global.playerClass);
            this.ClassSpecificMenuScreen = createClassSpecificMenuScreen();
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
    }
}
