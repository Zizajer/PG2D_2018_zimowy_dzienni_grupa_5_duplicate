using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class FullHPPotion : Item, IUsableUpdatableItem
    {
        public bool IsCurrentlyInUse { get; private set; }
        public int RemainingUsages { get; set; }
        float healthGained;

        public FullHPPotion(ContentManager content, Vector2 position) : base(content, position)
        {
            Initialize(content);
        }
        public FullHPPotion(ContentManager content) : base(content)
        {
            Initialize(content);
        }
        private void Initialize(ContentManager content)
        {
            Name = "Full HP Potion";
            Description = "This potion will slowly yet fully restore your health.";
            Category = "Potion";
            RemainingUsages = 1;

            TextureName = "potion1";
            LoadTexture(content);


        }
        public void Use(Character owner)
        {
            IsCurrentlyInUse = true;
        }

        public void Update(GameTime gameTime, Character character)
        {
            if (character.CurrentHealthPercent < 100)
            {
                character.CurrentHealthPercent += 0.06f;
            }
        }
    }
}
