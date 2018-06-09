using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dungeon_Crawler
{
    public class DragonAttack : IPositionTargetedAttack
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
        private int Range;
        private readonly float VanishDelay;

        private Character Attacker;

        public DragonAttack()
        {
            Name = "Big Dragon Attack";
            Power = 150;
            Accuracy = 80;
            CriticalHitProbability = 15;
            FreezeProbability = 0;
            BurnProbability = 100;
            IsSpecial = true;
            ManaCost = 90;

            Range = 10;
            VanishDelay = 0;
        }

        public bool Use(Character attacker, Vector2 position)
        {
            if (Global.random.Next(100) >= 100 - Accuracy)
            {
                //TODO: Move ContentManager to global because these workarounds are simply dumb
                //Also, move initializaition of this variable to constructor (for some reason it throws NullPointerException there)
                ProjectileTexture = Global.CombatManager.levelManager.Content.Load<Texture2D>("spells/BigFireball");
                Attacker = attacker;

                float distanceX = position.X - attacker.Center.X;
                float distanceY = position.Y - attacker.Center.Y;

                float rotation = (float)Math.Atan2(distanceY, distanceX);
                Vector2 tempPosition = attacker.Center;

                Vector2 tempVelocity;
                Projectile newProjectile;

                tempVelocity = new Vector2((float)Math.Cos(rotation)*2.5f, ((float)Math.Sin(rotation)*2.5f));
                newProjectile = new Projectile(this, attacker, tempVelocity, tempPosition, ProjectileTexture, rotation, Range, VanishDelay);

                Global.CombatManager.PutProjectile(newProjectile);

                return true; //Attack hit
            }
            else
            {
                Global.Gui.WriteToConsole(attacker.Name + " missed " + Name);
                return false; // Attack missed
            }
        }

        public void Notify(Character defender)
        {
            if (defender != Attacker)
            {
                Global.CombatManager.Attack(Attacker, this, defender);
            }
        }
    }
}
