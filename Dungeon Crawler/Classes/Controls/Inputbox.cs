using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Dungeon_Crawler
{
    public class Inputbox
    {
        private SpriteFont _font;
        private KeyboardState oldKeyState;
        private KeyboardState keyState;
        private Texture2D _texture;
        public Color PenColour { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            }
        }


        List<Keys> keysToCheck = new List<Keys> {
            Keys.A, Keys.B, Keys.C, Keys.D, Keys.E,
            Keys.F, Keys.G, Keys.H, Keys.I, Keys.J,
            Keys.K, Keys.L, Keys.M, Keys.N, Keys.O,
            Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T,
            Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y,
            Keys.Z, Keys.Back, Keys.Space, Keys.Back
        };
        public Inputbox(Texture2D texture, SpriteFont font)
        {
            _texture = texture;

            _font = font;

            keyState = Keyboard.GetState();

            PenColour = Color.Black;
        }


        public void Draw(GameTime gametime, SpriteBatch spriteBatch)
        {
            var color = Color.White;

            spriteBatch.Draw(_texture, Rectangle, color);

            if (!String.IsNullOrEmpty(Global.playerName))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Global.playerName).X / 2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Global.playerName).Y / 2);


                spriteBatch.DrawString(_font, Global.playerName, new Vector2(x, y), PenColour);
            }
        }

        public void Update(GameTime gametime)
        {
            oldKeyState = keyState;
            keyState = Keyboard.GetState();
            foreach (Keys key in keyState.GetPressedKeys())
            {
                if (oldKeyState.IsKeyUp(key))
                {
                    if (keysToCheck.Contains(key))
                    {
                        if(key == Keys.Back){
                            Global.playerName = "";
                        }
                        else if (key == Keys.Space)
                        {
                            if(Global.playerName.Length<10)
                                Global.playerName += " ";
                        }
                        else {
                            if (Global.playerName.Length < 10)
                            {
                                Global.playerName += key.ToString();
                                Global.playerName = MenuManager.ToTitleCase(Global.playerName);
                            } 
                        }
                    }
                }
            }
        }
    }
}
