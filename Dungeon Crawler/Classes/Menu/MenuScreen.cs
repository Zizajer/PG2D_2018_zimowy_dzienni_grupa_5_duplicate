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
    public abstract class MenuScreen : IMenuScreen
    {
        public List<Button> buttonsInMenu { get; set; }
        public List<Inputbox> inputBoxesInMenu { get; set; }
        public List<Checkbox> checkBoxesInMenu { get; set; }
        public List<Label> labelsInMenu { get; set; }
        public List<Sprite> spriteInMenu { get; set; }

        public void addButton(Vector2 Position, string text)
        {
            throw new NotImplementedException();
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
