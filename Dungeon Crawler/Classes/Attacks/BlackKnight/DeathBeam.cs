using System.Collections.Generic;

namespace Dungeon_Crawler
{
    public class DeathBeam : IUnTargetedAttack
    {
        public string Name { get; set; }
        public int Power { get; set; }
        public int Accuracy { get; set; }
        public int CriticalHitProbability { get; set; }
        public int FreezeProbability { get; set; }
        public int BurnProbability { get; set; }

        public bool IsSpecial { get; set; }
        public int ManaCost { get; set; }

        private readonly int Distance;
        private readonly string AnimationName;

        public DeathBeam()
        {
            Name = "Death Beam";
            Power = 100;
            Accuracy = 100;
            CriticalHitProbability = 50;
            FreezeProbability = 0;
            BurnProbability = 0;
            IsSpecial = true;
            ManaCost = 60;

            Distance = 3;
            AnimationName = "MagicAnim2";
        }

        public bool Use(Character attacker)
        {
            if (Global.random.Next(100) >= 100 - Accuracy)
            {
                if (!Global.CombatManager.SetAnimationInAreaInFrontOfAttacker(attacker, Name, AnimationName, attacker.CellX, attacker.CellY, Distance))
                {
                    Global.Gui.WriteToConsole(attacker.Name + " failed " + Name + " and got his mana refunded");
                    return false;
                }
                    
                List<Character> listOfEnemiesAround = Global.CombatManager.GetEnemiesInAreaInFrontOfAttacker(attacker, attacker.CellX, attacker.CellY, Distance);
                if (listOfEnemiesAround.Count > 0)
                {
                    foreach (Character defender in listOfEnemiesAround)
                    {
                        Global.CombatManager.Attack(attacker, this, defender);
                    }
                }
                return true; //Attack hit
            }
            return false;
        }
    }
}
