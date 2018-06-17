using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon_Crawler
{
    class GUI
    {
        public LevelManager lm;

        public GraphicsDeviceManager graphics;
        private SpriteFont font;
        private string[] console = { "", "", "", "", "", "" };
        private double gameTime;
        private double lastMsgGametime = 0;
        private double afterHowLongClearHighestMsg = 2; //sec

        public bool drawingStats = true;

        private int HealthBarOverCharacterBackgroundWidth = 50;
        private int HealthBarOverCharacterHeight = 10;

        private Texture2D BossSkull;

        public bool drawingNewLevelMsg = false;
        public float newLevelTimer=0;
        public float newLevelTimerHowLongToShow = 3f;

        public GUI(GraphicsDeviceManager graphics, ContentManager content)
        {
            this.graphics = graphics;
            font = content.Load<SpriteFont>("fonts/Default");
            BossSkull = content.Load<Texture2D>("icons/Skull");
        }
        public void Draw(SpriteBatch spriteBatch,GameTime gameTime)
        {
            int tempX = Global.Camera.ViewportWorldBoundry().X+1;
            int tempY = Global.Camera.ViewportWorldBoundry().Y+1;
            int tempX2 = tempX + (int)(Global.Camera.ViewportWorldBoundry().Width * 0.28);
            int tempY2 = tempY + (int)(Global.Camera.ViewportWorldBoundry().Height * 0.07);
            int tempY3 = tempY + (int)(Global.Camera.ViewportWorldBoundry().Height * 0.14);
            int tempY4 = tempY + (int)(Global.Camera.ViewportWorldBoundry().Height * 0.25);
            int PlayerBarsWidth = (int)(Global.Camera.ViewportWorldBoundry().Width * 0.35);
            int PlayerBarsHeight = (int)(Global.Camera.ViewportWorldBoundry().Height * 0.05);

            int gameOverX = (int)(Global.Camera.ViewportWorldBoundry().X+Global.Camera.ViewportWorldBoundry().Width / 2 - font.MeasureString("Game Over").Length() / 2);
            int gameOverY = Global.Camera.ViewportWorldBoundry().Y+Global.Camera.ViewportWorldBoundry().Height / 3;
            float scale = Global.Camera.Zoom;
            if (lm.player.CurrentHealth > 0)
            {
                //**player stats**//
                DrawPlayerStatBars(spriteBatch, lm.player, tempX, tempY, tempX, tempY2, 0, 0, PlayerBarsWidth, PlayerBarsHeight, scale);

                //Draw healthbar
                DrawHealthBarOverCharacter(spriteBatch, lm.player);

                //Draw level
                spriteBatch.DrawString(font, lm.player.Level.ToString(), new Vector2(lm.player.Position.X + HealthBarOverCharacterBackgroundWidth + 2, lm.player.Position.Y - 27), Color.Black, 0.0f, Vector2.One, 0.5f / scale, SpriteEffects.None, Layers.Text);

                //Draw name
                spriteBatch.DrawString(font, lm.player.Name, new Vector2((int)lm.player.Position.X, (int)lm.player.Position.Y - 39), Color.Black, 0.0f, Vector2.One, 0.5f / scale, SpriteEffects.None, Layers.Text);

                DrawDungeonLevel(spriteBatch, tempX, tempY3, scale);

                DrawInventory(spriteBatch, tempX, tempY3, scale);
                if (drawingNewLevelMsg)
                {
                    newLevelTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (newLevelTimer < newLevelTimerHowLongToShow)
                    {
                        string s = "You advanced from level " + (lm.player.Level-1) + " to " + lm.player.Level;
                        DrawMsgInTheMiddleOfScreen(s, spriteBatch, gameOverX, gameOverY, scale);
                    }else
                    {
                        drawingNewLevelMsg = false;
                    }
                }
            }
            else
            {
                DrawMsgInTheMiddleOfScreen("Game Over",spriteBatch, gameOverX, gameOverY, scale);
            }
            DrawConsole(font, tempX, tempY, scale, spriteBatch);
            if(drawingStats) DrawStats(font,tempX,tempY,scale,spriteBatch);
        }

        public void DrawEnemyPlates(SpriteBatch spriteBatch, GameTime gameTime)
        {
            float scale = Global.Camera.Zoom;
            if (lm.player.CurrentHealth > 0)
            {
                if (lm.levels[lm.player.CurrentMapLevel].isBossLevel)
                {
                    //**boss stats**//
                    if (lm.levels[lm.player.CurrentMapLevel].enemies.Count > 0)
                    {
                        //Draw healthbar
                        DrawHealthBarOverCharacter(spriteBatch, lm.levels[lm.player.CurrentMapLevel].enemies[0]);

                        //Draw level
                        spriteBatch.Draw(BossSkull, new Vector2(lm.levels[lm.player.CurrentMapLevel].enemies[0].Position.X + HealthBarOverCharacterBackgroundWidth + 2, lm.levels[lm.player.CurrentMapLevel].enemies[0].Position.Y - 27), null, Color.White, 0.0f, Vector2.One, 0.5f / scale, SpriteEffects.None, Layers.Text);

                        //Draw name
                        spriteBatch.DrawString(font, lm.levels[lm.player.CurrentMapLevel].enemies[0].Name, new Vector2((int)lm.levels[lm.player.CurrentMapLevel].enemies[0].Position.X, (int)lm.levels[lm.player.CurrentMapLevel].enemies[0].Position.Y - 39), Color.Black, 0.0f, Vector2.One, 0.5f / scale, SpriteEffects.None, Layers.Text);
                    }
                }
                else
                {
                    //**enemy stats**//
                    foreach (Character enemy in lm.levels[lm.player.CurrentMapLevel].enemies)
                    {
                        //Draw healthbar
                        DrawHealthBarOverCharacter(spriteBatch, enemy);

                        //Draw level
                        spriteBatch.DrawString(font, enemy.Level.ToString(), new Vector2(enemy.Position.X + HealthBarOverCharacterBackgroundWidth + 2, enemy.Position.Y - 27), Color.Black, 0.0f, Vector2.One, 0.5f / scale, SpriteEffects.None, Layers.Text);

                        //Draw name
                        spriteBatch.DrawString(font, enemy.Name, new Vector2((int)enemy.Position.X, (int)enemy.Position.Y - 39), Color.Black, 0.0f, Vector2.One, 0.5f / scale, SpriteEffects.None, Layers.Text);
                    }
                }
            }
        }
        private void DrawMsgInTheMiddleOfScreen(String s, SpriteBatch spriteBatch, int X, int Y, float scale)
        {
            spriteBatch.DrawString(font, s, new Vector2(X, Y), Color.White, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
        }

        public void nextLevel(int level)
        {
            drawingNewLevelMsg = true;
            newLevelTimer = 0;
        }

        private void DrawDungeonLevel(SpriteBatch spriteBatch, int tempX, int tempY3, float scale)
        {
            string s = " \nDungeon Level " + (lm.player.CurrentMapLevel + 1);
            spriteBatch.DrawString(font, s, new Vector2(tempX, tempY3), Color.White, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
        }

        private void DrawConsole(SpriteFont font, int tempX, int tempY, float scale, SpriteBatch spriteBatch)
        {
            tempY += (int)(0.84 * Global.Camera.ViewportWorldBoundry().Height);
            string tempString = console[0] + "\n" + console[1] + "\n" + console[2] + "\n" + console[3] + "\n" + console[4] + "\n" + console[5];
            spriteBatch.DrawString(font, tempString, new Vector2(tempX, tempY), Color.White, 0.0f, Vector2.One, 0.7f * (1 / scale), SpriteEffects.None, Layers.Text);
        }

        private void DrawStats(SpriteFont font, int tempX, int tempY, float scale, SpriteBatch spriteBatch)
        {
            Player p = lm.player;
            string tempString = "Stats:\n" + "Level: " + p.Level+ "\n"+"Exp: "+p.Experience + "\n" + "Attack: " + p.Attack + "\n" + "Magic Attack: " + p.SpAttack + "\n" + "Defense: " + p.Defense + "\n" + "Magic Defense: " + p.SpDefense + "\n" + "Speed: " + Math.Round(p.Speed,1) + "\n" + "Attack Speed: " + p.timeBetweenActions + "\n";
            tempX += (int)(Global.Camera.ViewportWorldBoundry().Width) - (int)Math.Ceiling(font.MeasureString(tempString).Length()* 0.7f * (1 / scale));
            tempY += (int)(Global.Camera.ViewportWorldBoundry().Height)-130;
            spriteBatch.DrawString(font, tempString, new Vector2(tempX, tempY), Color.White, 0.0f, Vector2.One, 0.7f * (1 / scale), SpriteEffects.None, Layers.Text);
        }

        private void DrawInventory(SpriteBatch spriteBatch, int tempX, int tempY3, float scale)
        {
            //Draw iventory and map level
            float InventoryStringPixelLength = 0;
            if (lm.player.Inventory.Count > 0)
            {
                string s = "Inventory: ";
                spriteBatch.DrawString(font, s, new Vector2(tempX + InventoryStringPixelLength, tempY3), Color.White, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
                InventoryStringPixelLength += font.MeasureString(s).Length() * 1 / scale;
                for (int i = 0; i < lm.player.Inventory.Count; i++)
                {
                    string ItemName = lm.player.Inventory[i].Name + ", ";
                    if (lm.player.SelectedItem == i)
                    {
                        spriteBatch.DrawString(font, ItemName, new Vector2(tempX + InventoryStringPixelLength, tempY3), Color.Blue, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
                    }
                    else
                    {
                        spriteBatch.DrawString(font, ItemName, new Vector2(tempX + InventoryStringPixelLength, tempY3), Color.White, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
                    }

                    InventoryStringPixelLength += font.MeasureString(ItemName).Length() * 1 / scale;
                }
            }
            else
            {
                string s = "No items in inventory";
                spriteBatch.DrawString(font, s, new Vector2(tempX + InventoryStringPixelLength, tempY3), Color.White, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
            }
            
        }

        private void DrawHealthBarOverCharacter(SpriteBatch spriteBatch, Character character)
        {
            int HealthBarCurrentHealthWidth = (int)((double)character.CurrentHealthPercent / 100 * HealthBarOverCharacterBackgroundWidth);

            Texture2D HealthBarCurrentHealthTexture = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Texture2D HealthBarBackgroundTexture = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            HealthBarBackgroundTexture.SetData<Color>(new Color[] { Color.Black });

            if (character.CurrentHealthPercent > 65)
            {
                HealthBarCurrentHealthTexture.SetData<Color>(new Color[] { Color.Green });
            }
            else if (character.CurrentHealthPercent > 35)
            {
                HealthBarCurrentHealthTexture.SetData<Color>(new Color[] { Color.Orange });
            }
            else
            {
                HealthBarCurrentHealthTexture.SetData<Color>(new Color[] { Color.Red });
            }
            Rectangle temp = new Rectangle((int)character.Position.X, (int)character.Position.Y - 25, HealthBarOverCharacterBackgroundWidth, HealthBarOverCharacterHeight);
            spriteBatch.Draw(HealthBarBackgroundTexture, temp, null, Color.White, 0f, new Vector2(HealthBarBackgroundTexture.Width / 2, HealthBarBackgroundTexture.Height / 2), SpriteEffects.None, Layers.Text);
            spriteBatch.Draw(HealthBarCurrentHealthTexture, new Rectangle((int)character.Position.X, (int)character.Position.Y - 25, HealthBarCurrentHealthWidth, HealthBarOverCharacterHeight), Color.White);

            HealthBarBackgroundTexture.Dispose();
            HealthBarCurrentHealthTexture.Dispose();
        }

        private void DrawPlayerStatBars(SpriteBatch spriteBatch, Player player, int hpX, int hpY, int resourceX, int resourceY, int xpX, int xpY, int width, int height, float scale)
        {
            int HealthBarCurrentHealthWidth = (int)((double)player.CurrentHealthPercent / 100 * width);
            int ResourceBarCurrentResourceWidth = (int)((double)player.CurrentResourcePercent / 100 * width);

            Texture2D HealthBarCurrentHealthTexture = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Texture2D HealthBarBackgroundTexture = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            HealthBarBackgroundTexture.SetData<Color>(new Color[] { Color.Black });

            Texture2D ResourceBarCurrentResourceTexture = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Texture2D ResourceBarBackgroundTexture = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            ResourceBarBackgroundTexture.SetData<Color>(new Color[] { Color.Black });

            if (player.CurrentHealthPercent > 65)
            {
                HealthBarCurrentHealthTexture.SetData<Color>(new Color[] { Color.Green });
            }
            else if (player.CurrentHealthPercent > 35)
            {
                HealthBarCurrentHealthTexture.SetData<Color>(new Color[] { Color.Orange });
            }
            else
            {
                HealthBarCurrentHealthTexture.SetData<Color>(new Color[] { Color.DarkRed });
            }

            if (Global.playerClass.Equals("Warrior"))
            {
                ResourceBarCurrentResourceTexture.SetData<Color>(new Color[] { Color.Red });
            }
            else if (Global.playerClass.Equals("Ranger"))
            {
                ResourceBarCurrentResourceTexture.SetData<Color>(new Color[] { Color.Yellow });
            }
            else
            {
                ResourceBarCurrentResourceTexture.SetData<Color>(new Color[] { Color.Blue });
            }
                
            spriteBatch.Draw(HealthBarBackgroundTexture, new Rectangle(hpX, hpY, width, height), Color.White * 0.5f);
            spriteBatch.Draw(HealthBarCurrentHealthTexture, new Rectangle(hpX, hpY, HealthBarCurrentHealthWidth, height), Color.White * 0.7f);

            spriteBatch.Draw(ResourceBarBackgroundTexture, new Rectangle(resourceX, resourceY, width, height), Color.White * 0.5f);
            spriteBatch.Draw(ResourceBarCurrentResourceTexture, new Rectangle(resourceX, resourceY, ResourceBarCurrentResourceWidth, height), Color.White * 0.7f);

            //Draw HP ratio
            string HealthText = (int)player.CurrentHealth + "/" + player.Health;
            Vector2 HealthTextOffsetFromCenter = font.MeasureString(HealthText);
            spriteBatch.DrawString(font, HealthText, new Vector2(hpX + (float)width / 2 - HealthTextOffsetFromCenter.X / 2 * 0.75f / scale, hpY+10* 0.75f / scale), Color.White, 0.0f, Vector2.One, 0.75f / scale, SpriteEffects.None, Layers.Text);

            //Draw mana ratio
            string ManaText = (int)player.CurrentResource + "/" + player.Resource;
            Vector2 ManaTextOffsetFromCenter = font.MeasureString(ManaText);
            spriteBatch.DrawString(font, ManaText, new Vector2(resourceX + (float)width / 2 - ManaTextOffsetFromCenter.X / 2 * 0.75f / scale, resourceY+10* 0.75f / scale), Color.White, 0.0f, Vector2.One, 0.75f / scale, SpriteEffects.None, Layers.Text);

            HealthBarBackgroundTexture.Dispose();
            HealthBarCurrentHealthTexture.Dispose();
            ResourceBarBackgroundTexture.Dispose();
            ResourceBarCurrentResourceTexture.Dispose();
        }

        public void Update(GameTime gameTime)
        {
            this.gameTime += gameTime.ElapsedGameTime.TotalSeconds;

            if (lastMsgGametime + afterHowLongClearHighestMsg < this.gameTime)
                ScrollConsole();
        }
        public void WriteToConsole(string msg)
        {
            if (console[5].Equals(msg)) return;

            console[0] = console[1];
            console[1] = console[2];
            console[2] = console[3];
            console[3] = console[4];
            console[4] = console[5];
            console[5] = msg;

            lastMsgGametime = gameTime;
        }
        public void ScrollConsole()
        {
            console[0] = console[1];
            console[1] = console[2];
            console[2] = console[3];
            console[3] = console[4];
            console[4] = console[5];
            console[5] = "";

            lastMsgGametime = gameTime;
        }
    }
}
