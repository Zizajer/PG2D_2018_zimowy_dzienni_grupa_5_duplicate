using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon_Crawler
{
    class GUI
    {
        private readonly Player _player;
        private int playerCurrentLevel;
        private int health;
        private int mana;
        private SpriteFont font;
        private string[] console = { "", "", "", "" };

        public GUI(Player player, SpriteFont f)
        {
            _player = player;
            font = f;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            int tempX = Global.Camera.ViewportWorldBoundry().X;
            int tempY = Global.Camera.ViewportWorldBoundry().Y;
            float scale = Global.Camera.Zoom;
            if (_player.Health > 0)
            {
                string healthString = "Health " + health + "%";
                string manaString = "Mana " + mana + "%";
                string s = _player.getItems() + " \nLevel " + (playerCurrentLevel + 1);
                spriteBatch.DrawString(font, healthString, new Vector2(tempX, tempY), Color.OrangeRed, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
                spriteBatch.DrawString(font, manaString, new Vector2(tempX+180, tempY), Color.DeepSkyBlue, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
                spriteBatch.DrawString(font, s, new Vector2(tempX, tempY+30), Color.White, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
            }
            else
            {
                spriteBatch.DrawString(font, "Game Over", new Vector2(tempX, tempY), Color.White, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
            }
            tempY += (int)(0.8 * Global.Camera.ViewportWorldBoundry().Height);
            string tempString = console[0] + "\n" + console[1] + "\n" + console[2] + "\n" + console[3];
            spriteBatch.DrawString(font, tempString, new Vector2(tempX, tempY), Color.White, 0.0f, Vector2.One, 0.7f * (1 / scale), SpriteEffects.None, Layers.Text);

        }
        public void Update()
        {
            health = _player.Health;
            mana = (int)_player.Mana;
            playerCurrentLevel = _player.CurrentLevel;
        }
        public void WriteToConsole(string msg)
        {
            console[0] = console[1];
            console[1] = console[2];
            console[2] = console[3];
            console[3] = msg;
        }
    }
}
