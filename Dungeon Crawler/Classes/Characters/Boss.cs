using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using RoyT.AStar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon_Crawler
{
    public class Boss : Character
    {
        public enum State { Standby, Attacking};

        float actionTimer;
        float timeBetweenActions;

        Map map;

        State currentState;

        public Boss(Dictionary<string, Animation> _animations, int cellSize, float timeBetweenActions, Map map)
        {
            Health = 100;
            this._animations = _animations;
            this.timeBetweenActions = timeBetweenActions;
            Speed = 0f;
            _animationManager = new AnimationManager(_animations.First().Value);
            actionTimer = 0;

            this.map = map;
            x = (int)Math.Floor(Center.X / cellSize);
            y = (int)Math.Floor(Center.Y / cellSize);
        }

        public bool IsHitByProjectile(Level level, GraphicsDevice graphicsDevice)
        {
            PlayerProjectile projectile =null;
            for (int i = 0; i < level.playerProjectiles.Count; i++)
            {
                projectile = level.playerProjectiles[i];
                if (Collision.checkCollision(getRectangle(), this, projectile, graphicsDevice))
                {
                    projectile.isEnemyHit = true;
                    return true;
                }
            } 
            return false;
        }

        public override void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {
            actionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            x = (int)Math.Floor(Center.X / level.cellSize);
            y = (int)Math.Floor(Center.Y / level.cellSize);

            if (IsHitByProjectile(level, graphicsDevice))
            {
                Health -= 1;
            }

            if (currentState == State.Standby)
            {
                if (map.IsInFov(x, y))
                {
                    currentState = State.Attacking;
                }

            }
            else if (currentState == State.Attacking)
            {
                if (!map.IsInFov(x, y))
                {
                    currentState = State.Standby;
                }
                Fireball(level);
            }
            else
            {
                Console.WriteLine("Error");
            }
            SetAnimations();
            _animationManager.Update(gameTime);
        }
        public void Fireball(Level level)
        {
            if (actionTimer>timeBetweenActions)
            {
                actionTimer = 0;

                float distanceX = level.player.Center.X - Center.X;
                float distanceY = level.player.Center.Y - Center.Y;

                float rotation = (float)Math.Atan2(distanceY, distanceX);
                Vector2 tempVelocity = new Vector2((float)Math.Cos(rotation)*3f, ((float)Math.Sin(rotation)) * 3f);
                Vector2 tempPosition = Center;

                EnemyProjectile newProjectile = new EnemyProjectile(tempVelocity, tempPosition, level.fireballBoss, rotation);

                level.enemyProjectiles.Add(newProjectile);
            }
        }
        protected override void SetAnimations()
        {
             _animationManager.Play(_animations["BossAlive"]);
        }
    }
}
