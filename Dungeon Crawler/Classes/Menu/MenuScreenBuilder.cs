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
        public List<Art> artsInMenu { get; set; }

        private Texture2D button;
        private SpriteFont buttonFont;
        private SpriteFont smallButtonFont;

        public MenuScreenBuilder(ContentManager Content, int screenWidth, int screenHeight) {
            this.button = Content.Load<Texture2D>("Controls/button");
            this.buttonFont = Content.Load<SpriteFont>("fonts/Chiller");
            this.smallButtonFont = Content.Load<SpriteFont>("fonts/smallChiller");
            this.buttonsInMenu = new List<Button>();
            this.checkBoxesInMenu = new List<Checkbox>();
            this.inputBoxesInMenu = new List<Inputbox>();
            this.labelsInMenu = new List<Label>();
            this.artsInMenu = new List<Art>();
            
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
            checkBoxesInMenu.Add(new Checkbox(button, buttonFont)
            {
                Position = Position,
            }
            );
        }

        public void addIputbox(Vector2 Position, string text)
        {
            inputBoxesInMenu.Add(new Inputbox(button, buttonFont)
            {
                Position = Position,
            }
            );
        }

        public void addLabel(Vector2 Position, string text)
        {
            labelsInMenu.Add(new Label(buttonFont, text, Position));
        }

        public void addSprite(Vector2 Position, Texture2D texture)
        {
            artsInMenu.Add(new Art(Position, texture));
        }

        public MenuScreen toBuild() {
            MenuScreen menuScreen =  new MenuScreen(buttonsInMenu, inputBoxesInMenu, checkBoxesInMenu, labelsInMenu, artsInMenu);
            clearLists();
            return menuScreen;
        }

        private void clearLists() {
            this.buttonsInMenu.Clear();
            this.checkBoxesInMenu.Clear();
            this.inputBoxesInMenu.Clear();
            this.labelsInMenu.Clear();
            this.artsInMenu.Clear();

        }
    }
}
