using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public interface IAttack
    {
        string Name { get; set; }
        int Power { get; set; }
        int Accuracy { get; set; }
        int CriticalHitProbability { get; set; }

        bool IsSpecial { get; set; }
        int ManaCost { get; set; }
    }
}
