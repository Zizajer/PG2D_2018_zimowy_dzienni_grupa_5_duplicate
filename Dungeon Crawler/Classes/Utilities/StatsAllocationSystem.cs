using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dungeon_Crawler
{
    public class StatsAllocationSystem
    {
        private Texture2D button;
        private Texture2D bigButton;
        private SpriteFont font;
        private LevelManager lm;

        private List<Label> labelList;
        private List<Button> buttonList;
        //private KeyboardState pastKey;

        Label pointsLabel;
        Label pointsToAllocateLabel;

        Label healthLabel;
        Label attackLabel;
        Label spattackLabel;
        Label defenseLabel;
        Label spdefenseLabel;

        Button addHealthButton;
        Button addAttackButton;
        Button addSpAttackButton;
        Button addDefenseButton;
        Button addSpDefenseButton;
        Button quitButton;

        private int points = 0;
        private int pointsAllocated = 0;
        public int pointsToAllocate;

        public StatsAllocationSystem(ContentManager Content, LevelManager lm)
        {
            this.lm = lm;
            button = Content.Load<Texture2D>("Controls/smallButton");
            bigButton = Content.Load<Texture2D>("Controls/button");
            font = Content.Load<SpriteFont>("fonts/Chiller");
            addLabels();
            addButtons();
        }
        public void PointsUpdate(GameTime gameTime)
        {
            points = lm.player.Level - 1;
            pointsToAllocate = points - pointsAllocated;
        }
        public void Update(GameTime gameTime)
        {
            points = lm.player.Level - 1;
            pointsToAllocate = points - pointsAllocated;
            pointsLabel.Text = "Points: " + points;
            pointsToAllocateLabel.Text = "Points to allocate: " + pointsToAllocate;

            healthLabel.Text = "Health: "+ lm.player.Health;
            attackLabel.Text = "Attack: " + lm.player.Attack;
            spattackLabel.Text = "SpAttack: " + lm.player.SpAttack;
            defenseLabel.Text = "Defense: " + lm.player.Defense;
            spdefenseLabel.Text = "SpDefense: " + lm.player.SpDefense;

            ToggleStatsAllocationMenu();
            if (pointsToAllocate > 0) { 
                foreach (Button button in buttonList)
                    button.Update(gameTime);
            }
            quitButton.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevice, GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            spriteBatch.DrawString(font, "Stats allocation menu", new Vector2(480, 10), Color.Black);

            foreach (Label label in labelList)
            {
                label.Draw(gameTime, spriteBatch);
            }

            if (pointsToAllocate > 0)
            {
                foreach (Button button in buttonList)
                {
                    button.Draw(gameTime, spriteBatch);
                }
            }
            quitButton.Draw(gameTime, spriteBatch);

            spriteBatch.End();

        }
        
        private void ToggleStatsAllocationMenu()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.O) &&  Global.pastKey13.IsKeyUp(Keys.O))
            {
                if(Global.CurrentGameState == Global.Gamestates.isStatsMenu)
                {
                    Global.CurrentGameState = Global.Gamestates.isGameActive;
                }
            }

             Global.pastKey13 = Keyboard.GetState();
        }
        
        private void addLabels()
        {
            pointsLabel = new Label(font, "", new Vector2(500, 100));
            pointsToAllocateLabel = new Label(font, "", new Vector2(500, 150));

            healthLabel = new Label(font, "", new Vector2(450, 250));
            attackLabel = new Label(font, "", new Vector2(450, 300));
            spattackLabel = new Label(font, "", new Vector2(450, 350));
            defenseLabel = new Label(font, "", new Vector2(450, 400));
            spdefenseLabel = new Label(font, "", new Vector2(450, 450));

            labelList = new List<Label>()
            {
                pointsLabel,
                pointsToAllocateLabel,
                healthLabel,
                attackLabel,
                spattackLabel,
                defenseLabel,
                spdefenseLabel,
            };
        }

        private void addButtons()
        {
            addHealthButton = new Button(button, font)
            {
                Position = new Vector2(700, 240),
                Text = "+",
            };
            addHealthButton.Click += addHealth_Click;

            addAttackButton = new Button(button, font)
            {
                Position = new Vector2(700, 290),
                Text = "+",
            };
            addAttackButton.Click += addAttack_Click;

            addSpAttackButton = new Button(button, font)
            {
                Position = new Vector2(700, 340),
                Text = "+",
            };
            addSpAttackButton.Click += addSpAttack_Click;

            addDefenseButton = new Button(button, font)
            {
                Position = new Vector2(700, 390),
                Text = "+",
            };
            addDefenseButton.Click += addDefense_Click;

            addSpDefenseButton = new Button(button, font)
            {
                Position = new Vector2(700, 440),
                Text = "+",
            };
            addSpDefenseButton.Click += addSpDefense_Click;

            quitButton = new Button(bigButton, font)
            {
                Position = new Vector2(575, 625),
                Text = "Quit",
            };
            quitButton.Click += quitButton_Click;

            buttonList = new List<Button>()
            {
                addHealthButton,
                addAttackButton,
                addSpAttackButton,
                addDefenseButton,
                addSpDefenseButton,
            };
        }
        void addHealth_Click(object sender, EventArgs e)
        {
            lm.player.Health++;
            pointsAllocated++;
        }
        void addAttack_Click(object sender, EventArgs e)
        {
            lm.player.Attack++;
            pointsAllocated++;
        }
        void addSpAttack_Click(object sender, EventArgs e)
        {
            lm.player.SpAttack++;
            pointsAllocated++;
        }
        void addDefense_Click(object sender, EventArgs e)
        {
            lm.player.Defense++;
            pointsAllocated++;
        }
        void addSpDefense_Click(object sender, EventArgs e)
        {
            lm.player.SpDefense++;
            pointsAllocated++;
        }
        void quitButton_Click(object sender, EventArgs e)
        {
            if(Global.CurrentGameState == Global.Gamestates.isStatsMenu)
            {
                Global.CurrentGameState = Global.Gamestates.isGameActive;
            }
        }
    }
}
