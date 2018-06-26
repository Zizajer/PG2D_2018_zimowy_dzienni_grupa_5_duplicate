using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon_Crawler
{
    public class Art
    {

        public Rectangle Rectangle { get; set; }
        public Texture2D Texture { get; set; }
        public Color Colour { get; set; }

        public Art(Texture2D tex, Rectangle rect)
        {
            Texture = tex;
            Rectangle = rect;
            Colour = Color.White;
        }

        public void Draw(GameTime gametime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Rectangle, Colour);
        }
    }
}
