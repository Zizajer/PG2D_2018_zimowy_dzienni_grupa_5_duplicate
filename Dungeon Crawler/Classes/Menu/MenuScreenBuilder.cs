using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon_Crawler
{
    public class MenuScreenBuilder : IMenuScreenBuilder
    {
        public List<Button> buttonsInMenu { get ; set ; }
        public List<Inputbox> inputBoxesInMenu { get; set; }
        public List<Checkbox> checkBoxesInMenu { get; set; }
        public List<Label> labelsInMenu { get; set; }
        public List<Art> artsInMenu { get; set; }

        private Texture2D buttonTexture;
        private Texture2D checkboxTexture;
        private SpriteFont font;
        private SpriteFont smallFont;

        public MenuScreenBuilder(ContentManager Content, int screenWidth, int screenHeight) {
            this.checkboxTexture = Content.Load<Texture2D>("Controls/checkbox");
            this.buttonTexture = Content.Load<Texture2D>("Controls/button");
            this.font = Content.Load<SpriteFont>("fonts/Chiller");
            this.smallFont = Content.Load<SpriteFont>("fonts/smallChiller");
            this.buttonsInMenu = new List<Button>();
            this.checkBoxesInMenu = new List<Checkbox>();
            this.inputBoxesInMenu = new List<Inputbox>();
            this.labelsInMenu = new List<Label>();
            this.artsInMenu = new List<Art>();
            
        }



        public void addButton(Vector2 Position, string text,EventHandler name)
        {
            var button = new Button(buttonTexture, font)
            {
                Position = Position,
                Text = text,
            };

            button.Click += name;
            buttonsInMenu.Add(button);
        }

        public void addCheckbox(Vector2 Position, EventHandler name)
        {
            var checkbox = new Checkbox(checkboxTexture, font)
            {
                Position = Position,
            };

            checkbox.Click += name;

            checkBoxesInMenu.Add(checkbox);
        }

        public void addInputbox(Vector2 Position)
        {
            inputBoxesInMenu.Add(new Inputbox(buttonTexture, font)
            {
                Position = Position,
            }
            );
        }

        public void addLabel(Vector2 Position, string text)
        {
            labelsInMenu.Add(new Label(font, text, Position));
        }

        public void addArt(Rectangle rectangle, Texture2D texture)
        {
            artsInMenu.Add(new Art(texture, rectangle));
        }

        public MenuScreen toBuild() {
            MenuScreen menuScreen =  new MenuScreen(buttonsInMenu, inputBoxesInMenu, checkBoxesInMenu, labelsInMenu, artsInMenu);
            clearLists();
            return menuScreen;
        }

        public void clearLists() {
            this.buttonsInMenu = new List<Button>();
            this.checkBoxesInMenu = new List<Checkbox>();
            this.inputBoxesInMenu = new List<Inputbox>();
            this.labelsInMenu = new List<Label>();
            this.artsInMenu = new List<Art>();
        }
    }
}
