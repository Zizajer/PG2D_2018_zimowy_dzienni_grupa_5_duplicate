﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dungeon_Crawler
{
    public class ThrowWeapon : IPositionTargetedAttack
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

        public ThrowWeapon()
        {
            Name = "Throw Weapon";
            Power = 300;
            Accuracy = 100;
            CriticalHitProbability = 10;
            FreezeProbability = 0;
            BurnProbability = 0;
            IsSpecial = false;
            ManaCost = 10;

            Range = 4;
            VanishDelay = 0; //useless since PiercingProjectille doesnt care
        }

        public bool Use(Character attacker, Vector2 position)
        {
            //TODO: Move ContentManager to global because these workarounds are simply dumb
            //Also, move initializaition of this variable to constructor (for some reason it throws NullPointerException there)
            ProjectileTexture = Global.CombatManager.levelManager.Content.Load<Texture2D>("spells/ThrowingWeapon");
            Attacker = attacker;

            float distanceX = position.X - attacker.Center.X;
            float distanceY = position.Y - attacker.Center.Y;

            float rotation = (float)Math.Atan2(distanceY, distanceX);
            Vector2 tempVelocity = new Vector2((float)Math.Cos(rotation) * 5f, ((float)Math.Sin(rotation)) * 5f) + attacker.Velocity / 3;
            Vector2 tempPosition = attacker.Center + tempVelocity * 10;

            Projectile newProjectile = new PiercingProjectile(this, Attacker, tempVelocity, tempPosition, ProjectileTexture, rotation, Range, VanishDelay);
            Global.CombatManager.PutProjectile(newProjectile);
            Global.SoundManager.playPew();

            return true; //Since accuracy of this attack is 100 and we want to implement this interface correctly.. maybe should change return type to void
        }

        public void Notify(Character defender)
        {
            Global.CombatManager.Attack(Attacker, this, defender);
        }
    }
}
