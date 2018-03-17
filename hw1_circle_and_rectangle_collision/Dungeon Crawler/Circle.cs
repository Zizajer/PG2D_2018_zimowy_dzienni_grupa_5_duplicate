using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    class Circle
    {
        public int x;
        public int y;
        public int radius;

        public Circle(int x, int y, int radius)
        {
            this.x = x;
            this.y = y;
            this.radius = radius;
        }

        public Boolean Intersects(Rectangle rectangle)
        {
            float closestX = Clamp(this.x, rectangle.X, rectangle.X + rectangle.Width);
            float closestY = Clamp(this.y, rectangle.Y, rectangle.Y + rectangle.Height);

            float distanceX = this.x - closestX;
            float distanceY = this.y - closestY;

            return Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2) < Math.Pow(this.radius, 2);
        }

        private float Clamp(float value, float min, float max)
        {
            float x = value;
            if (x < min)
            {
                x = min;
            }
            else if (x > max)
            {
                x = max;
            }
            return x;
        }

        public Rectangle Bounds()
        {
            return new Rectangle(this.x - radius, this.y - radius, 2 * this.radius, 2 * this.radius);
        }
    }
}
