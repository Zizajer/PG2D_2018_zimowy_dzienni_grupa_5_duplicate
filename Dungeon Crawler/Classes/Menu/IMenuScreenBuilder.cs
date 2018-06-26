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

        void addIputbox(Vector2 Position, String text);

        void addCheckbox(Vector2 Position, String text);

        void addLabel(Vector2 Position, String text);

        void addSprite(Vector2 Position, Texture2D texture);

        MenuScreen toBuild();

        void clearLists();


    }
}
