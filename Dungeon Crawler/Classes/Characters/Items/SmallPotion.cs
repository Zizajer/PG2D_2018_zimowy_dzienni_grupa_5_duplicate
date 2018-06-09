using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class SmallPotion : UsableItem
    {
        float Health;

        public SmallPotion(ContentManager content, Vector2 position) : base(content, position)
        {
            Initialize(content);
        }
        public SmallPotion(ContentManager content) : base(content)
        {
            Initialize(content);
        }
        private void Initialize(ContentManager content)
        {
            Name = "Small Potion";
            Description = "";
            Category = "Potion";

            TextureName = "potion1";
            LoadTexture(content);

            Health = 25;
        }
        public override void Use(Character owner)
        {
            if (owner.CurrentHealth + Health > owner.Health)
            {
                owner.CurrentHealth = owner.Health;
            }
            else
            {
                owner.CurrentHealth += Health;
            }          
        }
    }
}
