using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dungeon_Crawler
{
    class GUI
    {
        LevelManager lm;

        GraphicsDeviceManager graphics;
        private SpriteFont font;
        private string[] console = { "", "", "", "" };
        private double gameTime;
        private double lastMsgGametime = 0;
        private double afterHowLongClearHighestMsg = 4; //sec

        public GUI(GraphicsDeviceManager graphics, SpriteFont f)
        {
            this.graphics = graphics;
            font = f;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            int tempX = Global.Camera.ViewportWorldBoundry().X;
            int tempY = Global.Camera.ViewportWorldBoundry().Y;
            int tempX2 = Global.Camera.ViewportWorldBoundry().X + (int)(Global.Camera.ViewportWorldBoundry().Width * 0.28);
            int tempY2 = Global.Camera.ViewportWorldBoundry().Y + (int)(Global.Camera.ViewportWorldBoundry().Height * 0.07);
            int tempY3 = Global.Camera.ViewportWorldBoundry().Y + (int)(Global.Camera.ViewportWorldBoundry().Height * 0.21);

            int gameOverX = (int)(Global.Camera.ViewportWorldBoundry().X+Global.Camera.ViewportWorldBoundry().Width / 2 - font.MeasureString("Game Over").Length() / 2);
            int gameOverY = Global.Camera.ViewportWorldBoundry().Y+Global.Camera.ViewportWorldBoundry().Height / 3;
            float scale = Global.Camera.Zoom;
            if (lm.player.CurrentHealth > 0)
            {
                string healthString = "Health " + lm.player.CurrentHealth;
                string manaString = "Mana " + (int)lm.player.Mana;
                string s = lm.player.getItems() + " \nLevel " + (lm.player.CurrentMapLevel + 1);
                spriteBatch.DrawString(font, healthString, new Vector2(tempX, tempY), Color.OrangeRed, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
                spriteBatch.DrawString(font, manaString, new Vector2(tempX2, tempY), Color.DeepSkyBlue, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
                spriteBatch.DrawString(font, s, new Vector2(tempX, tempY2), Color.White, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);

                if (lm.levels[lm.player.CurrentMapLevel].isBossLevel)
                {
                    if(lm.levels[lm.player.CurrentMapLevel].enemies.Count>0){
                        string bossHealth = "Boss Health: " + lm.levels[lm.player.CurrentMapLevel].enemies[0].CurrentHealth;
                        spriteBatch.DrawString(font, bossHealth, new Vector2(tempX, tempY3), Color.White, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
                    }
                    else
                    {
                        string bossHealth = "Boss is dead";
                        spriteBatch.DrawString(font, bossHealth, new Vector2(tempX, tempY3), Color.White, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
                    }
                    
                }
                else
                {
                    foreach(Enemy enemy in lm.levels[lm.player.CurrentMapLevel].enemies)
                    {
                        //Health bar color
                        Texture2D HealthBarTexture = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                        if (enemy.CurrentHealthPercent > 65)
                        {
                            HealthBarTexture.SetData<Color>(new Color[] { Color.Green });
                        }
                        else if (enemy.CurrentHealthPercent > 35)
                        {
                            HealthBarTexture.SetData<Color>(new Color[] { Color.Orange });
                        }
                        else
                        {
                            HealthBarTexture.SetData<Color>(new Color[] { Color.Red });
                        }
                        //Health bar width
                        int HealthBarWidth = (int)((double)enemy.CurrentHealthPercent / 100 * 50);
                        int HealthBarHeight = 10;

                        spriteBatch.Draw(HealthBarTexture, new Rectangle((int)enemy.Position.X, (int)enemy.Position.Y - 30, HealthBarWidth, HealthBarHeight), Color.White);

                        string LevelString = "Level " + enemy.Level.ToString();
                        spriteBatch.DrawString(font, LevelString, new Vector2(enemy.Position.X + enemy.getWidth(), enemy.Position.Y), Color.Black, 0.0f, Vector2.One, 0.5f / scale, SpriteEffects.None, Layers.Text);
                    }
                }
            }
            else
            {
                spriteBatch.DrawString(font, "Game Over", new Vector2(gameOverX, gameOverY), Color.White, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
            }
            tempY += (int)(0.8 * Global.Camera.ViewportWorldBoundry().Height);
            string tempString = console[0] + "\n" + console[1] + "\n" + console[2] + "\n" + console[3];
            spriteBatch.DrawString(font, tempString, new Vector2(tempX, tempY), Color.White, 0.0f, Vector2.One, 0.7f * (1 / scale), SpriteEffects.None, Layers.Text);
        }

        internal void addLevelMananger(LevelManager levelManager)
        {
            lm = levelManager;
        }

        public void Update(GameTime gameTime)
        {
            if (lm.player.CurrentHealth <= 0)
                Global.GameState = false;
            this.gameTime += gameTime.ElapsedGameTime.TotalSeconds;

            if (lastMsgGametime + afterHowLongClearHighestMsg < this.gameTime)
                WriteToConsole("");
        }
        public void WriteToConsole(string msg)
        {
            console[0] = console[1];
            console[1] = console[2];
            console[2] = console[3];
            console[3] = msg;

            lastMsgGametime = gameTime;
        }
    }
}
