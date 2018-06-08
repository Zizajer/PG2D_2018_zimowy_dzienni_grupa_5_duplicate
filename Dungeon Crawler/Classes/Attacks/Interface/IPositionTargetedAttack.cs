using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    /*
     * Interface for attacks targeted at specified position
     */
    public interface IPositionTargetedAttack : IAttack
    {
        bool Use(Character attacker, Vector2 position);
        void Notify(Character defender);
    }
}
