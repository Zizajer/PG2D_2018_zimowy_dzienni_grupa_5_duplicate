using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon_Crawler
{
    public class Item : Obstacle
    {
        public string name { get; }
        public string description { get;}

        public Item(Vector2 pos, Texture2D tex, string name) : base(pos, tex)
        {
            this.name = name;

        }
        public Item(Vector2 pos, Texture2D tex, string name,string description) : base(pos, tex)
        {
            this.name = name;
            this.description = description;
        }
    }
}

