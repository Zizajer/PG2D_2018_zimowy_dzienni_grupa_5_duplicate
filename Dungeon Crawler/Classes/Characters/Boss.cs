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
        public new enum State { Standby, Attacking};
        public new State currentState;
        float actionTimer;
        float timeBetweenActions;

        Map map;

        public Boss(Dictionary<string, Animation> _animations, int cellSize, int level, float timeBetweenActions, Map map)
        {
            Level = level;
            calculateStatistics();

            this._animations = _animations;
            this.timeBetweenActions = timeBetweenActions;
            _animationManager = new AnimationManager(_animations.First().Value);
            actionTimer = 0;

            this.map = map;
            CellX = (int)Math.Floor(Center.X / cellSize);
            CellY = (int)Math.Floor(Center.Y / cellSize);
            Name = "Demon Oak";
        }

        public override void calculateStatistics()
        {
            Health = CurrentHealth = 210 + Level * 10;
            Defense = 210 + Level * 3;
            SpDefense = 210 + Level * 5;
            Attack = (int)Math.Floor(210 + Level * 2.5);
            SpAttack = 210 + Level * 3;
            Speed = 0f;
        }

        public bool IsHitByProjectile(Level level, GraphicsDevice graphicsDevice)
        {
            PlayerProjectile projectile =null;
            for (int i = 0; i < level.playerProjectiles.Count; i++)
            {
                projectile = level.playerProjectiles[i];
                if (Collision.checkCollision(getRectangle(), this, projectile, graphicsDevice))
                {
                    isHitShaderOn = true;
                    projectile.isMarkedToDelete = true;
                    return true;
                }
            } 
            return false;
        }

        public override void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {
            if (isHitShaderOn)
            {
                hitTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (hitTimer > howLongShouldShaderApply)
                {
                    hitTimer = 0;
                    isHitShaderOn = false;
                }
            }

            actionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            CellX = (int)Math.Floor(Center.X / level.cellSize);
            CellY = (int)Math.Floor(Center.Y / level.cellSize);

            if (IsHitByProjectile(level, graphicsDevice))
            {
                int damage = 20;
                CurrentHealth -= damage;
                string tempString = "Player's fireball hit " + Name+ " for " + damage;
                Global.Gui.WriteToConsole(tempString);
                if (CurrentHealth <= 0)
                {
                    level.grid.UnblockCell(new Position(5, 5));
                    level.grid.UnblockCell(new Position(6, 5));
                    level.grid.UnblockCell(new Position(7, 5));

                    level.grid.UnblockCell(new Position(5, 6));
                    level.grid.UnblockCell(new Position(6, 6));
                    level.grid.UnblockCell(new Position(7, 6));

                    level.grid.UnblockCell(new Position(5, 7));
                    level.grid.UnblockCell(new Position(6, 7));
                    level.grid.UnblockCell(new Position(7, 7));
                }

            }

            if (currentState == State.Standby)
            {
                if (map.IsInFov(CellX, CellY))
                {
                    currentState = State.Attacking;
                }

            }
            else if (currentState == State.Attacking)
            {
                if (!map.IsInFov(CellX, CellY))
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
