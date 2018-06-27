using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon_Crawler
{
    public class MenuScreen 
    {
        public List<Button> buttonsInMenu { get; set; }
        public List<Inputbox> inputBoxesInMenu { get; set; }
        public List<Checkbox> checkBoxesInMenu { get; set; }
        public List<Label> labelsInMenu { get; set; }
        public List<Art> artsInMenu { get; set; }

        public MenuScreen(List<Button> buttonsInMenu, List<Inputbox> inputBoxesInMenu, List<Checkbox> checkBoxesInMenu, List<Label> labelsInMenu, List<Art> artsInMenu)
        {
            this.buttonsInMenu = buttonsInMenu;
            this.inputBoxesInMenu = inputBoxesInMenu;
            this.checkBoxesInMenu = checkBoxesInMenu;
            this.labelsInMenu = labelsInMenu;
            this.artsInMenu = artsInMenu;
        }


        public void Update(GameTime gameTime)
        {
            foreach (Button button in buttonsInMenu)
                button.Update(gameTime);
            foreach (Inputbox inputBox in inputBoxesInMenu)
                inputBox.Update(gameTime);
            foreach (Checkbox checkBox in checkBoxesInMenu)
                checkBox.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, GameTime gameTime)
        {
            graphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            foreach (Inputbox inputBox in inputBoxesInMenu)
                inputBox.Draw(gameTime, spriteBatch);

            foreach (Label label in labelsInMenu)
                label.Draw(gameTime, spriteBatch);

            foreach(Art art in artsInMenu)
                art.Draw(gameTime, spriteBatch);

            foreach (Button button in buttonsInMenu)
                button.Draw(gameTime, spriteBatch);

            foreach (Checkbox checkBox in checkBoxesInMenu)
                checkBox.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }



    }
}
