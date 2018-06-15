using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class Wand2 : Item
    {
        public Wand2(ContentManager content, Vector2 position) : base(content, position)
        {
            Initialize(content);
        }
        public Wand2(ContentManager content) : base(content)
        {
            Initialize(content);
        }
        private void Initialize(ContentManager content)
        {
            Name = "Wand";
            Description = "Magic wand that increases your magic damage but decreases your magic defense";
            Category = "Wand";

            TextureName = "wand2";
            LoadTexture(content);

            SpAttackMultiplier = 1.2f;
            SpDefenseMultiplier = 0.9f;
        }
    }
}
