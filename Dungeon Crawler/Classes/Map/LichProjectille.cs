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
        float lifeValue = 8f;
        double time=10f;
        float randomFactor;
        float radius = Global.random.Next(250) + 128;
        public LichProjectile(IPositionTargetedAttack attack, Character attacker, Vector2 velocity, Vector2 position, Texture2D texture, float rotation, int range, float vanishDelay,Vector2 center) : base(attack, attacker, velocity, position, texture, rotation, range, vanishDelay)
        {
            centerOfRotation = center + new Vector2(320, 320);
            wasHitByThisProjectille = new List<Character>();
            do
            {
                randomFactor = (float)Global.random.NextDouble()+0.7f;
            } while (randomFactor > 1.3f);
        }

        public override void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {           
            time += gameTime.ElapsedGameTime.TotalSeconds;

            Rotation += (float)( radius/ 100);

            Position = new Vector2((float)(Math.Cos(time * randomFactor) * radius + centerOfRotation.X), (float)(Math.Sin(time * randomFactor) * radius + centerOfRotation.Y));

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
            
            //Destroy when hit a wall
            x = (int)Math.Floor(Position.X / level.cellSize);
            y = (int)Math.Floor(Position.Y / level.cellSize);
            if (x > 0 && x < level.map.Width && y > 0 && y < level.map.Height)
            {
                if (!level.map.GetCell(x, y).IsWalkable)
                    isMarkedToDelete = true;
            }
            else
            {
                isMarkedToDelete = true;
            }
                
            
            
        }
    }
}
