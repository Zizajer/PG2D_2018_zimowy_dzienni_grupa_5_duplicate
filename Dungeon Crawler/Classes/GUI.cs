using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon_Crawler
{
    public class GUI
    {
        private readonly Player _player;
        private SpriteFont font;
        private int health { get; set; }
        private string[] console = { "", "", "", "" };

        public GUI(Player player,SpriteFont f)
        {
            _player = player;
            font = f;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            int tempX = GlobalVariables.Camera.ViewportWorldBoundry().X;
            int tempY = GlobalVariables.Camera.ViewportWorldBoundry().Y;
            float scale = GlobalVariables.Camera.Zoom;
            if (health > 0)
            {
                spriteBatch.DrawString(font, "Health= " + health + "%", new Vector2(tempX, tempY), Color.Red, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
            }
            else
            {
                spriteBatch.DrawString(font, "Game Over", new Vector2(tempX, tempY), Color.Red, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, Layers.Text);
                GlobalVariables.GameState = 0;
            }
            tempY += (int) (0.8 * GlobalVariables.Camera.ViewportWorldBoundry().Height);
            string tempString = console[0] + "\n" + console[1] + "\n" + console[2]+ "\n" + console[3];
            spriteBatch.DrawString(font, tempString, new Vector2(tempX, tempY), Color.Red, 0.0f, Vector2.One, 0.7f* (1 / scale), SpriteEffects.None, Layers.Text);

        }
        public void Update()
        {
            health = _player.Health;
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
