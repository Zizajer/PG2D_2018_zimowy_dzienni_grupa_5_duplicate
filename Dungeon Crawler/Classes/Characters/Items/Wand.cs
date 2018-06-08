using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class Wand : Item
    {
        public Wand(ContentManager content, Vector2 position) : base(content, position)
        {
            Initialize(content);
        }
        public Wand(ContentManager content) : base(content)
        {
            Initialize(content);
        }
        private void Initialize(ContentManager content)
        {
            Name = "Wand";
            Description = "";
            Category = "Wand";

            TextureName = "wand1";
            LoadTexture(content);

            SpAttackMultiplier = 1.2f;
        }
    }
}
