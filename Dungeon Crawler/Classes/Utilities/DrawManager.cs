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
        private SpriteFont buttonFont;
        private Game1 game;

        private List<Button> _gameComponents;

        public DrawManager(ContentManager Content, Game1 game)
        {
            this.game = game;
            mag = Content.Load<Texture2D>("Arts/mage");
            warrior = Content.Load<Texture2D>("Arts/warrior");
            ranger = Content.Load<Texture2D>("Arts/ranger");
            button = Content.Load<Texture2D>("Controls/button");
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
                Position = new Vector2(575, 625),
                Text = "Quit",
            };

            quitButton.Click += quitButton_Click;

            _gameComponents = new List<Button>()
                {
                    magButton,
                    warriorButton,
                    rangerButton,
                    quitButton,
                };
        }

        void rangerButton_Click(object sender, EventArgs e)
        {

            Global.playerClass = ("Ranger");
            Global.levelmanager.setPlayer("Ranger");
            Global.CombatManager.setPLayer();
            Global.IsGameStarted = true;
        }

        void warriorButton_Click(object sender, EventArgs e)
        {
            Global.playerClass = ("Warrior");
            Global.levelmanager.setPlayer("Warrior");
            Global.CombatManager.setPLayer();
            Global.IsGameStarted = true;
        }

        void magButton_Click(object sender, EventArgs e)
        {
            Global.playerClass = ("Mage");
            Global.levelmanager.setPlayer("Mage");
            Global.CombatManager.setPLayer();
            Global.IsGameStarted = true;
        }

        void quitButton_Click(object sender, EventArgs e)
        {
            game.Exit();
        }



        public void UpdateMainMenu(GameTime gameTime)
        {
            foreach (Button button in _gameComponents)
                button.Update(gameTime);
        }

        public void DrawMainMenu(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevice, GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.DrawString(buttonFont, "Choose Character", new Vector2(550, 10), Color.Black);

            spriteBatch.Draw(mag, new Rectangle(50, 50, mag.Width, mag.Height), Color.White);

            spriteBatch.Draw(warrior, new Rectangle(575, 50, warrior.Width, warrior.Height), Color.White);

            spriteBatch.Draw(ranger, new Rectangle(1100, 50, ranger.Width, ranger.Height), Color.White);

            foreach (Button button in _gameComponents)
            {
                button.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();

        }
    }
}
