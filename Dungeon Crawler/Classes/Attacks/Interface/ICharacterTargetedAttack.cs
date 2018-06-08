using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    /*
     * Interface for attacks targeted at specified character (defender)
     */
    public interface ICharacterTargetedAttack : IAttack
    {
        bool Use(Character attacker, Character defender);
    }
}
