﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class HealthIncreasingPotion : Item, IUsableItem
    {
        public int RemainingUsages { get; set; }
        float healthGained;

        public HealthIncreasingPotion(ContentManager content, Vector2 position) : base(content, position)
        {
            Initialize(content);
        }
        public HealthIncreasingPotion(ContentManager content) : base(content)
        {
            Initialize(content);
        }
        private void Initialize(ContentManager content)
        {
            Name = "Perma HP Potion";
            healthGained = 10;
            Description = "Drink this potion to permamently increase your maximum health by " + healthGained + ".";
            Category = "Potion";
            RemainingUsages = 1;

            TextureName = "potion2";
            LoadTexture(content);

            
        }
        public void Use(Character owner)
        {
            owner.CurrentHealth += healthGained;
            owner.Health += healthGained;
                
            Global.Gui.WriteToConsole("The potion increased your health by "+ healthGained);
        }
    }
}
