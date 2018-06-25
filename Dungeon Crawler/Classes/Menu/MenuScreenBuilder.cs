using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon_Crawler.Classes
{
    public class MenuScreenBuilder : IMenuScreenBuilder
    {
        public List<Button> buttonsInMenu { get ; set ; }
        public List<Inputbox> inputBoxesInMenu { get; set; }
        public List<Checkbox> checkBoxesInMenu { get; set; }
        public List<Label> labelsInMenu { get; set; }
        public List<Sprite> spriteInMenu { get; set; }

        private Texture2D button;
        private SpriteFont buttonFont;
        private SpriteFont smallButtonFont;

        public MenuScreenBuilder(ContentManager Content, int screenWidth, int screenHeight) {
            button = Content.Load<Texture2D>("Controls/button");
            buttonFont = Content.Load<SpriteFont>("fonts/Chiller");
            smallButtonFont = Content.Load<SpriteFont>("fonts/smallChiller");
        }



        public void addButton(Vector2 Position, string text)
        {
            buttonsInMenu.Add(new Button(button, buttonFont)
            {
                Position = Position,
                Text = text,
            }
            );
        }

        public void addCheckbox(Vector2 Position, string text)
        {
            throw new NotImplementedException();
        }

        public void addIputbox(Vector2 Position, string text)
        {
            throw new NotImplementedException();
        }

        public void addLabel(Vector2 Position, string text)
        {
            throw new NotImplementedException();
        }

        public void addSprite(Vector2 Position, Texture2D texture)
        {
            throw new NotImplementedException();
        }
    }
}
