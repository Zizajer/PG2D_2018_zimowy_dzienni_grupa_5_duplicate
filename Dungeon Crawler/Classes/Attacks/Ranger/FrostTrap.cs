using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dungeon_Crawler
{
    public class FrostTrap : IPositionTargetedAttack
    {
        public string Name { get; set; }
        public int Power { get; set; }
        public int Accuracy { get; set; }
        public int CriticalHitProbability { get; set; }
        public int FreezeProbability { get; set; }
        public int BurnProbability { get; set; }

        public bool IsSpecial { get; set; }
        public int ManaCost { get; set; }

        private Texture2D ProjectileTexture;

        private Character Attacker;

        public FrostTrap()
        {
            Name = "Frost Trap";
            Power = 10;
            Accuracy = 100;
            CriticalHitProbability = 0;
            FreezeProbability = 100;
            BurnProbability = 0;
            IsSpecial = true;
            ManaCost = 20;
        }

        public bool Use(Character attacker, Vector2 position)
        {
            if (Global.random.Next(100) >= 100 - Accuracy)
            {
                //TODO: Move ContentManager to global because these workarounds are simply dumb
                //Also, move initializaition of this variable to constructor (for some reason it throws NullPointerException there)
                ProjectileTexture = Global.CombatManager.levelManager.Content.Load<Texture2D>("spells/FrostTrap");
                Attacker = attacker;

                Trap newTrap = new Trap(this, Attacker, Attacker.Center, ProjectileTexture);
                Global.CombatManager.PutProjectile(newTrap);
                return true; //Attack hit
            }
            else
            {
                Global.Gui.WriteToConsole(attacker.Name + " missed " + Name);
                return false; //Attack missed
            }
        }

        public void Notify(Character defender)
        {
            Global.CombatManager.Attack(Attacker, this, defender);
        }
    }
}
