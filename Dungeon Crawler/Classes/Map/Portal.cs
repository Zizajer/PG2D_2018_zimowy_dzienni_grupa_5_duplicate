using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon_Crawler
{
    public class Portal:Sprite
    {
        public Portal(Vector2 pos, Texture2D tex) : base(pos, tex)
        {
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0.0f, Vector2.One, Size, SpriteEffects.None, Layers.Items);
        }
    }
}
