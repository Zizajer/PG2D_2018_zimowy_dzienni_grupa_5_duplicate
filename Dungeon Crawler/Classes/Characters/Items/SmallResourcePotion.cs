using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class SmallResourcePotion : Item, IUsableItem
    {
        public int RemainingUsages { get; set; }
        float resource;

        public SmallResourcePotion(ContentManager content, Vector2 position) : base(content, position)
        {
            Initialize(content);
        }
        public SmallResourcePotion(ContentManager content) : base(content)
        {
            Initialize(content);
        }
        private void Initialize(ContentManager content)
        {
            Name = "Small Resource Potion";
            resource = 20;
            Description = "Drink this potion to gain mmaximum of " + resource + " resource. Strongly increases your desire to urinate.";
            Category = "Potion";
            RemainingUsages = 1;

            TextureName = "potion3";
            LoadTexture(content);

            
        }
        public void Use(Character owner)
        {
            Player player = (Player)owner;
            if (player.CurrentResource == player.Resource)
            {
                Global.Gui.WriteToConsole("You drank the potion. It had no effect on you");
            }
            else
            {
                float resourceGained = resource;
                if (player.CurrentResource + resource > player.Resource)
                {
                    resourceGained = player.Resource - player.CurrentResource;
                    player.CurrentResource += resourceGained;
                    Global.Gui.WriteToConsole("The potion added " + Math.Ceiling(resourceGained) + " resource");
                }
                else
                {
                    player.CurrentResource += resourceGained;
                }
                Global.Gui.WriteToConsole("The potion added " + Math.Ceiling(resourceGained) + " resource");
            }   
        }
    }
}
