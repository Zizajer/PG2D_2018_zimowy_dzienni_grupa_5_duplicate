﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dungeon_Crawler
{
    public class MultiShot : IPositionTargetedAttack
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

        public MultiShot()
        {
            Name = "MultiShot";
            Power = 150;
            Accuracy = 100;
            CriticalHitProbability = 15;
            FreezeProbability = 0;
            BurnProbability = 0;
            IsSpecial = false;
            ManaCost = 60;

            Range = 3;
            VanishDelay = 0;
        }

        public bool Use(Character attacker, Vector2 position)
        {
            if (Global.random.Next(100) >= 100 - Accuracy)
            {
                //TODO: Move ContentManager to global because these workarounds are simply dumb
                //Also, move initializaition of this variable to constructor (for some reason it throws NullPointerException there)
                ProjectileTexture = Global.CombatManager.levelManager.Content.Load<Texture2D>("spells/Arrow");
                Attacker = attacker;

                float distanceX = position.X - attacker.Center.X;
                float distanceY = position.Y - attacker.Center.Y;

                float rotation = (float)Math.Atan2(distanceY, distanceX);
                Vector2 tempPosition = attacker.Center;

                float rotationIncrement = 0.1f;
                float newrotationClockwise = rotation;
                float newrotationCounterClockwise = rotation;
                Vector2 tempVelocity;
                Projectile newProjectile;

                float additionalSpeed = 4f;

                tempVelocity = new Vector2((float)Math.Cos(rotation) * additionalSpeed, ((float)Math.Sin(rotation)) * additionalSpeed);
                newProjectile = new Projectile(this, attacker, tempVelocity, tempPosition, ProjectileTexture, rotation, Range, VanishDelay);

                Global.CombatManager.PutProjectile(newProjectile);

                for (int i = 0; i < 2; i++)
                {
                    newrotationClockwise += rotationIncrement;
                    tempVelocity = new Vector2((float)Math.Cos(newrotationClockwise) * additionalSpeed, ((float)Math.Sin(newrotationClockwise)) * additionalSpeed);
                    newProjectile = new Projectile(this, attacker, tempVelocity, tempPosition, ProjectileTexture, newrotationClockwise, Range, VanishDelay);

                    Global.CombatManager.PutProjectile(newProjectile);

                    newrotationCounterClockwise -= rotationIncrement;
                    tempVelocity = new Vector2((float)Math.Cos(newrotationCounterClockwise) * additionalSpeed, ((float)Math.Sin(newrotationCounterClockwise)) * additionalSpeed);
                    newProjectile = new Projectile(this, attacker, tempVelocity, tempPosition, ProjectileTexture, newrotationCounterClockwise, Range, VanishDelay);

                    Global.CombatManager.PutProjectile(newProjectile);
                }

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
