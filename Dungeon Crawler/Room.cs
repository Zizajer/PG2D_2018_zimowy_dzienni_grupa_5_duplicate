using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    class Room
    {
        public int X1;
        public int X2;
        public int Y1;
        public int Y2;

        public int Width;
        public int Height;

        public Point Center;

        public Room(int x, int y, int width, int height)
        {
            X1 = x;
            X2 = x + width;
            Y1 = y;
            Y2 = y + height;
            Width = width;
            Height = height;
            Center = new Point((X1 + X2) / 2, (Y1 + Y2) / 2);
        }

        public bool Intersects(Room room)
        {
            return (X1 <= room.X2 && X2 >= room.X2 &&
            Y1 <= room.Y2 && room.Y2 >= room.Y1);
        }
    }
}
