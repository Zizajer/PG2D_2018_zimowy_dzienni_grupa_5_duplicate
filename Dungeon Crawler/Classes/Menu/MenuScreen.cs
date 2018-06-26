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

    }
}
