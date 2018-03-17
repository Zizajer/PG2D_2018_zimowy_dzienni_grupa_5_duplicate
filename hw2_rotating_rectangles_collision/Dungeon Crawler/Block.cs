using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    class Block
    {
        public Vector2 Position;
        public float Rotation;
        public Block(Vector2 pos, float rot)
        {
            Position = pos;
            Rotation = rot;
        }
    }
    
}
