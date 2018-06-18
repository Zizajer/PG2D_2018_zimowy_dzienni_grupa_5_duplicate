using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public interface IUpdatableItem : IItem
    {
        void Update(GameTime gameTime, Character character);
    }
}
