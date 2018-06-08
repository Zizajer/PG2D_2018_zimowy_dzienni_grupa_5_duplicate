using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    /*
     * Interface for untargeted attacks (e.g. Exori)
     */
    public interface IUnTargetedAttack : IAttack
    {
        bool Use(Character attacker);
    }
}
