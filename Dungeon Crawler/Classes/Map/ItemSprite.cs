using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dungeon_Crawler
{
    [Obsolete]
    public class ItemSprite : Sprite
    {
        public string name { get; }
        public string description { get;}

        public ItemSprite(Vector2 pos, Texture2D tex, string name) : base(pos, tex)
        {
            this.name = name;
        }
        public ItemSprite(Vector2 pos, Texture2D tex, string name,string description) : base(pos, tex)
        {
            this.name = name;
            this.description = description;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0.0f, Vector2.One, Size, SpriteEffects.None, Layers.Items);
        }

    }
}

