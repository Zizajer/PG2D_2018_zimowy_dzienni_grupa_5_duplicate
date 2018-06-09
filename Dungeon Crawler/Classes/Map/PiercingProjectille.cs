using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Dungeon_Crawler
{
    public class PiercingProjectile : Projectile
    {
        List<Character> wasHitByThisProjectille;
        public PiercingProjectile(IPositionTargetedAttack attack, Character attacker, Vector2 velocity, Vector2 position, Texture2D texture, float rotation, int range, float vanishDelay) : base(attack, attacker, velocity, position, texture, rotation, range, vanishDelay)
        {
            wasHitByThisProjectille = new List<Character>();
        }

        public override void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {
            //Moving by specified velocity
            Position += Velocity;

            //Detect collision with character and notify parent attack about who took a hit. 
            //Ignore collision with projectile creator. (Character shoudn't hit himself)
            foreach (Character character in level.enemies)
            {
                if (!wasHitByThisProjectille.Contains(character))
                {
                    if (Collision.checkCollision(character.getRectangle(), character, this, graphicsDevice))
                    {
                        if (character != Attacker)
                        {
                            Attack.Notify(character);
                            wasHitByThisProjectille.Add(character);
                        }
                    }
                }            
            }

            //Vanish projectile when out of range
            if (Vector2.Distance(Center, OriginalPosition) > Range * level.cellSize)
            {
                isMarkedToDelete = true;
            }

            //Destroy when hit a wall
            x = (int)Math.Floor(Position.X / level.cellSize);
            y = (int)Math.Floor(Position.Y / level.cellSize);
            if (!level.map.GetCell(x, y).IsWalkable)
                isMarkedToDelete = true;

            //Destroy projectile and rock!!!!!!!!!!!!!!!
            if (Collision.isCollidingWithRocks(this, level, graphicsDevice))
            {
                isMarkedToDelete = true;
                Collision.deleteCollidingRock(this, level, graphicsDevice);
            }      

            if (Collision.isCollidingWithPortal(this, level, graphicsDevice))
                isMarkedToDelete = true;
        }
    }
}
