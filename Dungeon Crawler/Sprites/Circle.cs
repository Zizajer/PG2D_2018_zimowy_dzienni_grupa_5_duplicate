using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dungeon_Crawler.Models;

namespace Dungeon_Crawler.Sprites
{
    public class Circle
    {
        protected Texture2D texture2D;
        public Vector2 center;
        public Color color = Color.White;
        public int radius;

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle(Convert.ToInt32(center.X) - radius, Convert.ToInt32(center.Y) - radius, 2 * radius, 2 * radius);
            }
        }

        public Circle(Texture2D texture)
        {
            texture2D = texture;
        }

        public virtual void Update(GameTime gameTime, List<Sprite> sprites) { }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture2D, Rectangle, color);

        }

        #region Collision

        public bool Intersects(Rectangle rectangle)
        {
            Vector2 v = new Vector2(MathHelper.Clamp(center.X, rectangle.Left, rectangle.Right),
                                    MathHelper.Clamp(center.Y, rectangle.Top, rectangle.Bottom));

            Vector2 direction = center - v;
            float distanceSquared = direction.LengthSquared();

            return ((distanceSquared > 0) && (distanceSquared < radius * radius));
        }
        #endregion
    }
}
