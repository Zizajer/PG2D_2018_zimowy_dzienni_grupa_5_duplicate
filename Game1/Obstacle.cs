using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    public class Obstacle
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Color[] TextureData { get; set; }
        public Obstacle(Vector2 pos,Texture2D tex)
        {
            Position = pos;
            Texture = tex;
            TextureData =
               new Color[Texture.Width * Texture.Height];
            Texture.GetData(TextureData);
        }
        public Rectangle getRectangle()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
