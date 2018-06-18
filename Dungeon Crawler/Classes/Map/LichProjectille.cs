using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dungeon_Crawler
{
    public class LichProjectile : Projectile
    {
        public List<Character> wasHitByThisProjectille;
        public Vector2 centerOfRotation;
        float liveTimer;
        float lifeValue = 5f;
        float radius = Global.random.Next(320) + 128;
        public LichProjectile(IPositionTargetedAttack attack, Character attacker, Vector2 velocity, Vector2 position, Texture2D texture, float rotation, int range, float vanishDelay,Vector2 center) : base(attack, attacker, velocity, position, texture, rotation, range, vanishDelay)
        {
            centerOfRotation = center + new Vector2(320, 320);
            wasHitByThisProjectille = new List<Character>();

            float Rotation = (float)Math.Atan2(centerOfRotation.X, centerOfRotation.Y) + MathHelper.PiOver2;
        }

        public override void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {           
            double time= gameTime.TotalGameTime.TotalSeconds;

            float speed = 1f; // in radians per second, this is 1/4 of a circle per second atm

            Rotation += (float)(MathHelper.Pi / radius);

            Vector2 vec2temp = new Vector2((float)(Math.Cos(time * speed) * radius + centerOfRotation.X), (float)(Math.Sin(time * speed) * radius + centerOfRotation.Y));
            Position = vec2temp;

            liveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!wasHitByThisProjectille.Contains(level.player))
            {
                if (Collision.checkCollision(level.player.getRectangle(), level.player, this, graphicsDevice))
                {
                    if (level.player != Attacker)
                    {
                        IsCharacterHit = true;
                        Attack.Notify(level.player);
                        wasHitByThisProjectille.Add(level.player);
                    }
                }
            }            
            

            //Vanish projectile when out of range
            if (Vector2.Distance(Center, OriginalPosition) > Range * level.cellSize)
            {
                isMarkedToDelete = true;
            }

            if(liveTimer>lifeValue)
                isMarkedToDelete = true;
            /*
            //Destroy when hit a wall
            x = (int)Math.Floor(Position.X / level.cellSize);
            y = (int)Math.Floor(Position.Y / level.cellSize);
            if (!level.map.GetCell(x, y).IsWalkable)
                isMarkedToDelete = true;   
            
             */
        }
    }
}
