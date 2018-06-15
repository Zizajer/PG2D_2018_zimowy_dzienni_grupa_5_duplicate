using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class Shield2 : Item
    {
        public Shield2(ContentManager content, Vector2 position) : base(content, position)
        {
            Initialize(content);
        }
        public Shield2(ContentManager content) : base(content)
        {
            Initialize(content);
        }
        private void Initialize(ContentManager content)
        {
            Name = "Uber Shield";
            Description = "Magic Shield that increases your defense and magic defense but decreases your attack and magic attack";
            Category = "Shield";

            TextureName = "shield2";
            LoadTexture(content);

            AttackMultiplier = 0.9f;
            SpAttackMultiplier = 0.9f;
            DefenseMultiplier = 1.2f;
            SpDefenseMultiplier = 1.2f;
        }
    }
}
