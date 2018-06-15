using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class Axe1 : Item
    {
        public Axe1(ContentManager content, Vector2 position) : base(content, position)
        {
            Initialize(content);
        }
        public Axe1(ContentManager content) : base(content)
        {
            Initialize(content);
        }
        private void Initialize(ContentManager content)
        {
            Name = "Axe of Doom";
            Description = "Mighty axe that increases your damage but decreases your defense";
            Category = "Axe";

            TextureName = "axe1";
            LoadTexture(content);

            AttackMultiplier = 1.2f;
            DefenseMultiplier = 0.9f;
        }
    }
}
