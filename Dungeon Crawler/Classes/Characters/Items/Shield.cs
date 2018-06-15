using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class Shield : Item
    {
        public Shield(ContentManager content, Vector2 position) : base(content, position)
        {
            Initialize(content);
        }
        public Shield(ContentManager content) : base(content)
        {
            Initialize(content);
        }
        private void Initialize(ContentManager content)
        {
            Name = "Shield";
            Description = "Shield that increases your defense but decreases your attack";
            Category = "Shield";

            TextureName = "shield1";
            LoadTexture(content);

            AttackMultiplier = 0.9f;
            DefenseMultiplier = 1.2f;
        }
    }
}
