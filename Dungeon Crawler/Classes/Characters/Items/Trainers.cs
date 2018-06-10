using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class Trainers : Item
    {
        public Trainers(ContentManager content, Vector2 position) : base(content, position)
        {
            Initialize(content);
        }
        public Trainers(ContentManager content) : base(content)
        {
            Initialize(content);
        }
        private void Initialize(ContentManager content)
        {
            Name = "Trainers";
            Description = "Fashionable shoes with three white stripes, each of them boosting your speed significantly.";
            Category = "SpeedBooster";

            TextureName = "shoes";
            LoadTexture(content);

            SpeedMultiplier = 1.3f;
        }
    }
}
