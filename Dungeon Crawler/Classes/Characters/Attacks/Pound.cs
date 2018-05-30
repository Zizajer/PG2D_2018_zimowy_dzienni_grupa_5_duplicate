using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class Pound : ICharacterTargetedAttack
    {
        public string Name { get; set; }
        public int Power { get; set; }
        public int Accuracy { get; set; }
        public int CriticalHitProbability { get; set; }

        public bool IsSpecial { get; set; }
        public int ManaCost { get; set; }

        private readonly string AnimationName;

        public Pound()
        {
            Name = "Pound";
            Power = 25;
            Accuracy = 100;
            CriticalHitProbability = 15;
            IsSpecial = false;
            ManaCost = 0;

            AnimationName = "baseAttackAnim";
        }

        public bool Use(Character attacker, Character defender)
        {
            if (Global.random.Next(100) >= 100 - Accuracy)
            {
                Global.CombatManager.SetAnimation(Name, AnimationName, defender.CellX, defender.CellY);
                Global.CombatManager.Attack(attacker, this, defender);
                return true; //Attack hit
            }
            else
            {
                return false; //Attack missed
            }
        }
    }
}
