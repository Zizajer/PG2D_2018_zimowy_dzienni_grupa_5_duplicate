using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class BigFireball : IPositionTargetedAttack
    {
        public string Name { get; set; }
        public int Power { get; set; }
        public int Accuracy { get; set; }
        public int CriticalHitProbability { get; set; }

        public bool IsSpecial { get; set; }
        public int ManaCost { get; set; }

        private Texture2D ProjectileTexture;
        private readonly float VanishDelay;
        private Character Attacker;

        public BigFireball()
        {
            Name = "Big Fireball";
            Power = 150;
            Accuracy = 80;
            CriticalHitProbability = 15;
            IsSpecial = true;
            ManaCost = 30;

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

                float distanceX = position.X - attacker.Position.X;
                float distanceY = position.Y - attacker.Position.Y;

                float rotation = (float)Math.Atan2(distanceY, distanceX);
                Vector2 tempVelocity = new Vector2((float)Math.Cos(rotation) * 5f, ((float)Math.Sin(rotation)) * 5f) + attacker.Velocity / 3;
                Vector2 tempPosition = attacker.Center + tempVelocity * 10;

                Projectile newProjectile = new Projectile(this, tempVelocity, tempPosition, ProjectileTexture, rotation, VanishDelay);
                Global.CombatManager.PutProjectile(newProjectile);
                Global.SoundManager.playPew();

                return true; //Attack hit
            }
            else
            {
                return false; //Attack missed
            }
        }

        public void Notify(Character defender)
        {
            Global.CombatManager.Attack(Attacker, this, defender);
        }
    }
}
