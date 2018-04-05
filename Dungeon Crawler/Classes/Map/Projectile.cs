using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dungeon_Crawler
{
    public class Projectile
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Origin {
            get
            {
                Rectangle rec = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
                return new Vector2(rec.Width / 2, rec.Height / 2);
            }
            set { }
        }
        public float rotation;
        public bool isVisible;

        public Color[] TextureData { get; }
        public Projectile(Vector2 vel, Vector2 pos, Texture2D tex,float rot)
        {
            Position = pos;
            Texture = tex;
            Velocity = vel;
            rotation = rot;
            isVisible = true;
            Rectangle rec = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            Origin = new Vector2(rec.Width/2, rec.Height/2);
            TextureData =
               new Color[Texture.Width * Texture.Height];
            Texture.GetData(TextureData);
        }
        public Rectangle getRectangle()
        {
            //return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            throw new Exception("not implemented");
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, rotation, Origin, 0.5f, SpriteEffects.None, Layers.Projectiles);
        }
    }
}
