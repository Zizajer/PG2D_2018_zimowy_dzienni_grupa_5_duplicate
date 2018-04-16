using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon_Crawler
{
    public class Sprite
    {
        public float Size { get; set; }
        public Vector2 Origin { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Color[] TextureData { get; }
        public Sprite(Vector2 pos, Texture2D tex)
        {
            Position = pos;
            Texture = tex;
            TextureData =
               new Color[Texture.Width * Texture.Height];
            Texture.GetData(TextureData);
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            Size = 1.0f;
        }
        public Vector2 Center
        {
            get { return new Vector2(Position.X + Texture.Width / 2, Position.Y + Texture.Height / 2); }
        }
        public Rectangle getRectangle()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width*Size), (int)(Texture.Height*Size));
        }
    }
}
