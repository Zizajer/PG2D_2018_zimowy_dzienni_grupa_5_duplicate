using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler.Classes.Characters.Attacks.Interface
{
    /*
     * Interface for untargeted attacks (e.g. Exori)
     */
    public interface IUnTargetedAttack : IAttack
    {
        bool Use(Character attacker);
    }
}
