using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public interface IUsableItem : IItem
    {
        int RemainingUsages { get; set; }

        void Use(Character owner);
    }
}
