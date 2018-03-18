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
            int tempX = Global.Camera.ViewportWorldBoundry().X;
            int tempY = Global.Camera.ViewportWorldBoundry().Y;
            float scale = Global.Camera.Zoom;
            if (health > 0)
            {
                spriteBatch.DrawString(font, "Health= " + health + "%", new Vector2(tempX, tempY), Color.Green, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, LayerDepth.Text);
            }
            else
            {
                spriteBatch.DrawString(font, "Game Over", new Vector2(tempX, tempY), Color.Red, 0.0f, Vector2.One, 1 / scale, SpriteEffects.None, LayerDepth.Text);
                Global.GameState = 0;
            }
            tempY += (int) (0.8 * Global.Camera.ViewportWorldBoundry().Height);
            string tempString = console[0] + "\n" + console[1] + "\n" + console[2]+ "\n" + console[2];
            spriteBatch.DrawString(font, tempString, new Vector2(tempX, tempY), Color.Red, 0.0f, Vector2.One, 0.7f* (1 / scale), SpriteEffects.None, LayerDepth.Text);

        }
        public void Update()
        {
            health = _player.Health;
        }
        public void WriteToConsole(string msg)
        {
            console[0]= console[1];
            console[1] = console[2];
            console[2] = console[3];
            console[3] = msg;
        }
    }   
}
