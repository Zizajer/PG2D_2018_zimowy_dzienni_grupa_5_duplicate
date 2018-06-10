using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class InvisibilityPotion : UsableItem
    {
        public InvisibilityPotion(ContentManager content, Vector2 position) : base(content, position)
        {
            Initialize(content);
        }
        public InvisibilityPotion(ContentManager content) : base(content)
        {
            Initialize(content);
        }
        private void Initialize(ContentManager content)
        {
            Name = "Invisibility Potion";
            Description = "Drink this potion to gain invisibility from every monster for short time. Warning: Invisibility works only as long as you have your resource available";
            Category = "Potion";

            TextureName = "invisPotion";
            LoadTexture(content);
        }
        public override void Use(Character owner)
        {
            Player p = (Player)owner;
            p.isRangerInvisible = true;
            Global.Gui.WriteToConsole("You are now invisible");
        }
    }
}
