using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class Pendant : Item
    {
        public Pendant(ContentManager content, Vector2 position) : base(content, position)
        {
            Initialize(content);
        }
        public Pendant(ContentManager content) : base(content)
        {
            Initialize(content);
        }
        private void Initialize(ContentManager content)
        {
            Name = "Pendant of magic defense";
            Description = "Magic wand that increases your magic defense but decreases your magic attack";
            Category = "Pendant";

            TextureName = "pendant1";
            LoadTexture(content);

            SpDefenseMultiplier = 1.2f;
            SpAttackMultiplier = 0.9f;
        }
    }
}
