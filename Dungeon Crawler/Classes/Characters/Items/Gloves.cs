using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class Gloves : Item
    {
        public Gloves(ContentManager content, Vector2 position) : base(content, position)
        {
            Initialize(content);
        }
        public Gloves(ContentManager content) : base(content)
        {
            Initialize(content);
        }
        private void Initialize(ContentManager content)
        {
            Name = "Speedy Gloves";
            Description = "Magic gloves that increases your magic attack speed. How? They provide good grip on your weapon";
            Category = "Gloves";

            TextureName = "gloves";
            LoadTexture(content);

            TimeBetweenActionsMultiplier = 0.95f;
        }
    }
}
