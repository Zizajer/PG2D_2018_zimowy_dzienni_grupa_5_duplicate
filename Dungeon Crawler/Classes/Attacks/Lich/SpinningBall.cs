using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dungeon_Crawler
{
    public class SpinningBall : IPositionTargetedAttack
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

        private Vector2 centerOfRotation;
        private Character Attacker;

        public SpinningBall(Vector2 center)
        {
            Name = "Spinning Ball";
            Power = 75;
            Accuracy = 100;
            CriticalHitProbability = 5;
            FreezeProbability = 0;
            BurnProbability = 0;
            IsSpecial = true;
            ManaCost = 90;

            Range = 10;
            VanishDelay = 0;

            centerOfRotation = center;
        }

        public bool Use(Character attacker, Vector2 position)
        {
            if (Global.random.Next(100) >= 100 - Accuracy)
            {
                //TODO: Move ContentManager to global because these workarounds are simply dumb
                //Also, move initializaition of this variable to constructor (for some reason it throws NullPointerException there)
                ProjectileTexture = Global.CombatManager.levelManager.Content.Load<Texture2D>("spells/Lichball");
                Attacker = attacker;

                float rotation = MathHelper.PiOver2;
                Vector2 tempPosition = attacker.Center;

                Vector2 tempVelocity;
                Projectile newProjectile;

                tempVelocity = new Vector2((float)Math.Cos(rotation) * 1.7f, ((float)Math.Sin(rotation)) * 1.7f);
                newProjectile = new LichProjectile(this, attacker, tempVelocity, tempPosition, ProjectileTexture, rotation, Range, VanishDelay, centerOfRotation);

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
