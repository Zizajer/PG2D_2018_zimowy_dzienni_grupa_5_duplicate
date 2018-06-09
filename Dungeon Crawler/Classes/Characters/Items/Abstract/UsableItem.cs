using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public abstract class UsableItem : Item
    {
        public UsableItem(ContentManager content, Vector2 position) : base(content, position) { }
        public UsableItem(ContentManager content) : base (content) { }
        public abstract void Use(Character owner);
    }
}
