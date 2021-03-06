﻿using System.Collections.Generic;

namespace Dungeon_Crawler
{
    public class UltimateExplosion : IUnTargetedAttack
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

        public UltimateExplosion()
        {
            Name = "Ultimate Explosion";
            Power = 850;
            Accuracy = 100;
            CriticalHitProbability = 70;
            FreezeProbability = 0;
            BurnProbability = 60;
            IsSpecial = true;
            ManaCost = 100;

            Distance = 4;
            AnimationName = "UltimateExplosionAttackAnim";
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
                Global.Gui.WriteToConsole(attacker.Name + " missed " + Name);
                return false; //Attack missed
            }
        }
    }
}
