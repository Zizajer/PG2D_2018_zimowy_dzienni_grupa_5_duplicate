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
    public interface IMenuScreenBuilder
    {
        List<Button> buttonsInMenu { get; set; }
        List<Inputbox>inputBoxesInMenu { get; set; }
        List<Checkbox> checkBoxesInMenu { get; set; }
        List<Label> labelsInMenu { get; set; }
        List<Art> artsInMenu { get; set; }

        void addButton(Vector2 Position, String text);

        void addInputbox(Vector2 Position);

        void addCheckbox(Vector2 Position);

        void addLabel(Vector2 Position, String text);

        void addArt(Rectangle rectangle, Texture2D texture);

        MenuScreen toBuild();

        void clearLists();


    }
}
