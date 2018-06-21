using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class IceCube : Item
    {
        public IceCube(ContentManager content, Vector2 position) : base(content, position)
        {
            Initialize(content);
        }
        public IceCube(ContentManager content) : base(content)
        {
            Initialize(content);
        }
        private void Initialize(ContentManager content)
        {
            Name = "Ice Cube";
            Description = "Comin' straight from the underground. Gives you ability to use iceball. Prevents burns.";
            Category = "PositionTargetedAttackItem_Iceball";

            TextureName = "iceCube";
            LoadTexture(content);

            //SpeedMultiplier = 1.1f;
        }
    }
}