using System.Collections.Generic;

namespace Dungeon_Crawler
{
    public class Exori : IUnTargetedAttack
    {
        public string Name { get; set; }
        public int Power { get; set; }
        public int Accuracy { get; set; }
        public int CriticalHitProbability { get; set; }

        public bool IsSpecial { get; set; }
        public int ManaCost { get; set; }
        public int FreezeProbability { get; set; }
        public int BurnProbability { get; set; }

        private readonly int Distance;
        private readonly string AnimationName;

        public Exori()
        {
            Name = "Exori";
            Power = 40;
            Accuracy = 80;
            CriticalHitProbability = 50;
            FreezeProbability = 0;
            BurnProbability = 0;
            IsSpecial = false;
            ManaCost = 70;

            Distance = 1;
            AnimationName = "baseAttackAnim";
        }

        public bool Use(Character attacker)
        {
            if (Global.random.Next(100) >= 100 - Accuracy)
            {
                Global.CombatManager.SetAnimationInArea(Name, AnimationName, attacker.CellX, attacker.CellY, Distance);

                List<Character> listOfEnemiesAround = Global.CombatManager.GetEnemiesInArea(attacker.CellX, attacker.CellY, Distance);
                if (listOfEnemiesAround.Count > 0)
                {
                    foreach (Character defender in listOfEnemiesAround)
                    {
                        Global.CombatManager.Attack(attacker, this, defender);
                    }
                }
                return true; //Attack hit
            }
            else
            {
                return false; //Attack missed
            }
        }
    }
}
