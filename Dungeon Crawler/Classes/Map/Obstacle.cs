using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon_Crawler
{
    public class Obstacle
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Color[] TextureData { get;}
        public Obstacle(Vector2 pos,Texture2D tex)
        {
            Position = pos;
            Texture = tex;
            TextureData =
               new Color[Texture.Width * Texture.Height];
            Texture.GetData(TextureData);
        }
        public Vector2 Origin
        {
            get { return new Vector2(Position.X + Texture.Width / 2, Position.Y + Texture.Height / 2); }
        }
        public Rectangle getRectangle()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0.0f, Vector2.One, 1.0f, SpriteEffects.None, Layers.Items);
        }
    }
}
