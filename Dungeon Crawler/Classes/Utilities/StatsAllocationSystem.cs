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

        private int screenWidth;
        private int screenHeight;

        public StatsAllocationSystem(ContentManager Content, LevelManager lm, int screenWidth, int screenHeight)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
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
            float value = 2 / 6f;
            points = lm.player.Level - 1;
            pointsToAllocate = points - pointsAllocated;

            pointsLabel.Text = "Points: " + points;
            pointsLabel.Position.X = screenWidth / 2 - font.MeasureString(pointsLabel.Text).Length() / 2;

            pointsToAllocateLabel.Text = "Points to allocate: " + pointsToAllocate;
            pointsToAllocateLabel.Position.X = screenWidth / 2 - font.MeasureString(pointsToAllocateLabel.Text).Length() / 2;

            healthLabel.Text = "Health: "+ lm.player.Health;
            healthLabel.Position.X = screenWidth * value - font.MeasureString(healthLabel.Text).Length() / 2;

            attackLabel.Text = "Attack: " + lm.player.Attack;
            attackLabel.Position.X = screenWidth * value - font.MeasureString(attackLabel.Text).Length() / 2;

            spattackLabel.Text = "SpAttack: " + lm.player.SpAttack;
            spattackLabel.Position.X = screenWidth * value - font.MeasureString(spattackLabel.Text).Length() / 2;

            defenseLabel.Text = "Defense: " + lm.player.Defense;
            defenseLabel.Position.X = screenWidth * value - font.MeasureString(defenseLabel.Text).Length() / 2;

            spdefenseLabel.Text = "SpDefense: " + lm.player.SpDefense;
            spdefenseLabel.Position.X = screenWidth * value - font.MeasureString(spdefenseLabel.Text).Length() / 2;


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
            string s = "Stats allocation menu";
            spriteBatch.DrawString(font, s, new Vector2(screenWidth / 2 - font.MeasureString(s).Length() / 2, 10), Color.Black);

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

            healthLabel = new Label(font, "", new Vector2(450, 300));
            attackLabel = new Label(font, "", new Vector2(450, 375));
            spattackLabel = new Label(font, "", new Vector2(450, 450));
            defenseLabel = new Label(font, "", new Vector2(450, 525));
            spdefenseLabel = new Label(font, "", new Vector2(450, 600));

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
            float value = 4 / 6f;
            string s;
            addHealthButton = new Button(button, font)
            {
                Position = new Vector2(screenWidth * value - button.Width / 2, 300),
                Text = "+",
            };
            addHealthButton.Click += addHealth_Click;

            addAttackButton = new Button(button, font)
            {
                Position = new Vector2(screenWidth * value - button.Width / 2, 375),
                Text = "+",
            };
            addAttackButton.Click += addAttack_Click;

            addSpAttackButton = new Button(button, font)
            {
                Position = new Vector2(screenWidth * value - button.Width / 2, 450),
                Text = "+",
            };
            addSpAttackButton.Click += addSpAttack_Click;

            addDefenseButton = new Button(button, font)
            {
                Position = new Vector2(screenWidth * value - button.Width / 2, 525),
                Text = "+",
            };
            addDefenseButton.Click += addDefense_Click;

            addSpDefenseButton = new Button(button, font)
            {
                Position = new Vector2(screenWidth * value - button.Width / 2, 600),
                Text = "+",
            };
            addSpDefenseButton.Click += addSpDefense_Click;

            s = "Go back";
            quitButton = new Button(bigButton, font)
            {
                Text = s,
                Position = new Vector2(screenWidth / 2 - font.MeasureString(s).Length(), 700),
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
