using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class SmallHealthPotion : Item, IUsableItem
    {
        public int RemainingUsages { get; set; }
        float healthGained;

        public SmallHealthPotion(ContentManager content, Vector2 position) : base(content, position)
        {
            Initialize(content);
        }
        public SmallHealthPotion(ContentManager content) : base(content)
        {
            Initialize(content);
        }
        private void Initialize(ContentManager content)
        {
            Name = "Small HP Potion";
            healthGained = 25;
            Description = "Drink this potion to heal yourself for maximum " + healthGained+ " health. Strongly increases your desire to urinate.";
            Category = "Potion";

            TextureName = "potion1";
            LoadTexture(content);

            
        }
        public void Use(Character owner)
        {
            if (owner.CurrentHealth == owner.Health)
            {
                Global.Gui.WriteToConsole("You drank the potion. It had no effect on you");
            }
            else
            {
                float healed=0;
                if (owner.CurrentHealth + healthGained > owner.Health)
                {
                    healed = owner.Health - owner.CurrentHealth;
                    owner.CurrentHealth = owner.Health;
                }
                else
                {
                    healed = healthGained;
                    owner.CurrentHealth += healthGained;
                }
                Global.Gui.WriteToConsole("The potion healed you for " + Math.Ceiling(healed));
            }   
        }
    }
}
