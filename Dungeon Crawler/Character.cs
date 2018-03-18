using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class Character
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Texture2D Sprite { get; set; }
        public int Damage { get; set; }
        public int Health { get; set; }
        public string Name { get; set; }
    }
}
