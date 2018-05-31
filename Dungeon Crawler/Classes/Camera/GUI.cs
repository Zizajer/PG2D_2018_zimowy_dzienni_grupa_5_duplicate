using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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

        private int HealthBarOverCharacterBackgroundWidth = 50;
        private int HealthBarOverCharacterHeight = 10;

        private Texture2D BossSkull;

        public GUI(GraphicsDeviceManager graphics, ContentManager content)
        {
            this.graphics = graphics;
            font = content.Load<SpriteFont>("fonts/Default");
            BossSkull = content.Load<Texture2D>("icons/Skull");
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            int tempX = Global.Camera.ViewportWorldBoundry().X;
            int tempY = Global.Camera.ViewportWorldBoundry().Y;
            int tempX2 = Global.Camera.ViewportWorldBoundry().X + (int)(Global.Camera.ViewportWorldBoundry().Width * 0.28);
            int tempY2 = Global.Camera.ViewportWorldBoundry().Y + (int)(Global.Camera.ViewportWorldBoundry().Height * 0.07);
            int tempY3 = Global.Camera.ViewportWorldBoundry().Y + (int)(Global.Camera.ViewportWorldBoundry().Height * 0.14);
            int tempY4 = Global.Camera.ViewportWorldBoundry().Y + (int)(Global.Camera.ViewportWorldBoundry().Height * 0.21);
            int PlayerBarsWidth = (int)(Global.Camera.ViewportWorldBoundry().Width * 0.35);
            int PlayerBarsHeight = (int)(Global.Camera.ViewportWorldBoundry().Height * 0.05);

            int gameOverX = (int)(Global.Camera.ViewportWorldBoundry().X+Global.Camera.ViewportWorldBoundry().Width / 2 - font.MeasureString("Game Over").Length() / 2);
            int gameOverY = Global.Camera.ViewportWorldBoundry().Y+Global.Camera.ViewportWorldBoundry().Height / 3;
            float scale = Global.Camera.Zoom;
            if (lm.player.CurrentHealth > 0)
            {

                if (lm.levels[lm.player.CurrentMapLevel].isBossLevel)
                {
                    //**boss stats**//
                    if (lm.levels[lm.player.CurrentMapLevel].enemies.Count>0){

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
                    foreach(Character enemy in lm.levels[lm.player.CurrentMapLevel].enemies)
                    {
                        //Draw healthbar
                        DrawHealthBarOverCharacter(spriteBatch, enemy);

                        //Draw level
                        spriteBatch.DrawString(font, enemy.Level.ToString(), new Vector2(enemy.Position.X + HealthBarOverCharacterBackgroundWidth + 2, enemy.Position.Y - 27), Color.Black, 0.0f, Vector2.One, 0.5f / scale, SpriteEffects.None, Layers.Text);

                        //Draw name
                        spriteBatch.DrawString(font, enemy.Name, new Vector2((int)enemy.Position.X, (int)enemy.Position.Y - 39), Color.Black, 0.0f, Vector2.One, 0.5f / scale, SpriteEffects.None, Layers.Text);
                    }
                }

                //**player stats**//
                DrawPlayerStatBars(spriteBatch, lm.player, tempX, tempY, tempX, tempY2, 0, 0, PlayerBarsWidth, PlayerBarsHeight, scale);

                //Draw healthbar
                DrawHealthBarOverCharacter(spriteBatch, lm.player);

                //Draw level
                spriteBatch.DrawString(font, lm.player.Level.ToString(), new Vector2(lm.player.Position.X + HealthBarOverCharacterBackgroundWidth + 2, lm.player.Position.Y - 27), Color.Black, 0.0f, Vector2.One, 0.5f / scale, SpriteEffects.None, Layers.Text);

                //Draw name
                spriteBatch.DrawString(font, lm.player.Name, new Vector2((int)lm.player.Position.X, (int)lm.player.Position.Y - 39), Color.Black, 0.0f, Vector2.One, 0.5f / scale, SpriteEffects.None, Layers.Text);

                //Draw map level
                string s = lm.player.getItems() + " \nLevel " + (lm.player.CurrentMapLevel + 1);
                spriteBatch.DrawString(font, s, new Vector2(tempX, tempY3), Color.White, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
            }
            else
            {
                spriteBatch.DrawString(font, "Game Over", new Vector2(gameOverX, gameOverY), Color.White, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
            }
            tempY += (int)(0.8 * Global.Camera.ViewportWorldBoundry().Height);
            string tempString = console[0] + "\n" + console[1] + "\n" + console[2] + "\n" + console[3];
            spriteBatch.DrawString(font, tempString, new Vector2(tempX, tempY), Color.White, 0.0f, Vector2.One, 0.7f * (1 / scale), SpriteEffects.None, Layers.Text);
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



            spriteBatch.Draw(HealthBarBackgroundTexture, new Rectangle((int)character.Position.X, (int)character.Position.Y - 25, HealthBarOverCharacterBackgroundWidth, HealthBarOverCharacterHeight), Color.White);
            spriteBatch.Draw(HealthBarCurrentHealthTexture, new Rectangle((int)character.Position.X, (int)character.Position.Y - 25, HealthBarCurrentHealthWidth, HealthBarOverCharacterHeight), Color.White);

            HealthBarBackgroundTexture.Dispose();
            HealthBarCurrentHealthTexture.Dispose();
        }

        private void DrawPlayerStatBars(SpriteBatch spriteBatch, Player player, int hpX, int hpY, int manaX, int manaY, int xpX, int xpY, int width, int height, float scale)
        {
            int HealthBarCurrentHealthWidth = (int)((double)player.CurrentHealthPercent / 100 * width);
            int ManaBarCurrentManaWidth = (int)((double)player.CurrentManaPercent / 100 * width);

            Texture2D HealthBarCurrentHealthTexture = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Texture2D HealthBarBackgroundTexture = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            HealthBarBackgroundTexture.SetData<Color>(new Color[] { Color.Black });

            Texture2D ManaBarCurrentManaTexture = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Texture2D ManaBarBackgroundTexture = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            ManaBarBackgroundTexture.SetData<Color>(new Color[] { Color.Black });

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
                HealthBarCurrentHealthTexture.SetData<Color>(new Color[] { Color.Red });
            }

            ManaBarCurrentManaTexture.SetData<Color>(new Color[] { Color.Blue });

            spriteBatch.Draw(HealthBarBackgroundTexture, new Rectangle(hpX, hpY, width, height), Color.White * 0.5f);
            spriteBatch.Draw(HealthBarCurrentHealthTexture, new Rectangle(hpX, hpY, HealthBarCurrentHealthWidth, height), Color.White * 0.7f);

            spriteBatch.Draw(ManaBarBackgroundTexture, new Rectangle(manaX, manaY, width, height), Color.White * 0.5f);
            spriteBatch.Draw(ManaBarCurrentManaTexture, new Rectangle(manaX, manaY, ManaBarCurrentManaWidth, height), Color.White * 0.7f);

            //Draw HP ratio
            string HealthText = player.CurrentHealth + "/" + player.Health;
            Vector2 HealthTextOffsetFromCenter = font.MeasureString(HealthText);
            spriteBatch.DrawString(font, HealthText, new Vector2(hpX + (float)width / 2 - HealthTextOffsetFromCenter.X / 2 * 0.75f / scale, hpY), Color.White, 0.0f, Vector2.One, 0.75f / scale, SpriteEffects.None, Layers.Text);

            //Draw mana ratio
            string ManaText = (int)player.CurrentMana + "/" + player.Mana;
            Vector2 ManaTextOffsetFromCenter = font.MeasureString(ManaText);
            spriteBatch.DrawString(font, ManaText, new Vector2(manaX + (float)width / 2 - ManaTextOffsetFromCenter.X / 2 * 0.75f / scale, manaY), Color.White, 0.0f, Vector2.One, 0.75f / scale, SpriteEffects.None, Layers.Text);

            HealthBarBackgroundTexture.Dispose();
            HealthBarCurrentHealthTexture.Dispose();
            ManaBarBackgroundTexture.Dispose();
            ManaBarCurrentManaTexture.Dispose();
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
