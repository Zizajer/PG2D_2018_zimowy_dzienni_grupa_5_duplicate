using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Dungeon_Crawler
{
    public class Label
    {
        private SpriteFont _font;
        public Color PenColour { get; set; }
        public Vector2 Position;
        public String Text { get; set; }

        public Label(SpriteFont font, String txt, Vector2 pos)
        {
            Position = pos;
            _font = font;
            Text = txt;
            PenColour = Color.Black;
        }

        public void Draw(GameTime gametime, SpriteBatch spriteBatch)
        {
            if (!String.IsNullOrEmpty(Text))
            {
                spriteBatch.DrawString(_font, Text, Position, PenColour);
            }
        }
    }
}
