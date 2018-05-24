using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Dungeon_Crawler
{
    class GUI
    {
        LevelManager lm;
        private readonly Player _player;
        private int playerCurrentLevel;
        private int health;
        private int mana;
        private SpriteFont font;
        private string[] console = { "", "", "", "" };
        private double gameTime;
        private double lastMsgGametime=0;
        private double afterHowLongClearHighestMsg = 4; //sec

        public GUI(Player player, SpriteFont f)
        {
            _player = player;
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
            if (_player.Health > 0)
            {
                string healthString = "Health " + health;
                string manaString = "Mana " + mana;
                string s = _player.getItems() + " \nLevel " + (playerCurrentLevel + 1);
                spriteBatch.DrawString(font, healthString, new Vector2(tempX, tempY), Color.OrangeRed, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
                spriteBatch.DrawString(font, manaString, new Vector2(tempX2, tempY), Color.DeepSkyBlue, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
                spriteBatch.DrawString(font, s, new Vector2(tempX, tempY2), Color.White, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
                if (lm.levels[playerCurrentLevel].isBossLevel)
                {
                    if(lm.levels[playerCurrentLevel].enemies.Count>0){
                        string bossHealth = "Boss Health: " + lm.levels[playerCurrentLevel].enemies[0].Health;
                        spriteBatch.DrawString(font, bossHealth, new Vector2(tempX, tempY3), Color.White, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
                    }
                    else{
                        string bossHealth = "Boss is dead";
                        spriteBatch.DrawString(font, bossHealth, new Vector2(tempX, tempY3), Color.White, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
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
            health = _player.Health;
            mana = (int)_player.Mana;
            playerCurrentLevel = _player.CurrentMapLevel;
            if (health <= 0)
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
