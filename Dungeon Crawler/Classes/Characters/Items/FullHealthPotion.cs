using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class FullHealthPotion : Item, IUsableUpdatableItem
    {
        public bool IsCurrentlyInUse { get; private set; }
        public bool HasRecentUsageFinished { get; set; }
        public int RemainingUsages { get; set; }

        private float GivenHealth;

        public FullHealthPotion(ContentManager content, Vector2 position) : base(content, position)
        {
            Initialize(content);
        }
        public FullHealthPotion(ContentManager content) : base(content)
        {
            Initialize(content);
        }
        private void Initialize(ContentManager content)
        {
            Name = "Full HP Potion";
            Description = "This potion will slowly yet fully restore your health.";
            Category = "Potion";
            RemainingUsages = 1;

            TextureName = "potion4";
            LoadTexture(content);


        }
        public void Use(Character owner)
        {
            IsCurrentlyInUse = true;
            HasRecentUsageFinished = false;
        }

        public void Update(GameTime gameTime, Character character)
        {
            if (GivenHealth < character.Health && character.CurrentHealth < character.Health)
            {
                float CurrentHealthBefore = character.CurrentHealth;
                character.CurrentHealthPercent += 0.06f;
                float CurrentHealthAfter = character.CurrentHealth;
                GivenHealth += CurrentHealthAfter - CurrentHealthBefore;
            }
            else
            {
                IsCurrentlyInUse = false;
                HasRecentUsageFinished = true;
            }
        }
    }
}
