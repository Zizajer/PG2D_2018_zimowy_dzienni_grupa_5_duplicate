using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon_Crawler
{
    public class Enemy : Character
    {
        public int x { get; set; }
        public int y { get; set; }

        Map map;

        int currentDirection;
        //Random movement vars
        float actionTimer;
        float timeBetweenActions;

        //Pathfinding vars
        PathFinder PathFinder;
        Cell CurrentCell;
        Cell NextCell;

        public Enemy(Dictionary<string, Animation> _animations, int cellSize, float speed, float timeBetweenActions)
        {
            Health = 100;
            this._animations = _animations;
            this.timeBetweenActions = timeBetweenActions;
            Speed = speed;
            _animationManager = new AnimationManager(_animations.First().Value);
            actionTimer = 0;
            currentDirection= Global.random.Next(4) + 1;
        }

        public bool IsHitByProjectile(Level level, GraphicsDevice graphicsDevice)
        {
            Projectile projectile;
            for (int i = 0; i < level.projectiles.Count; i++)
            {
                projectile = level.projectiles[i];
                if (Collision.checkCollision(this, projectile, graphicsDevice))
                {
                    projectile.isEnemyHit = true;
                    return true;
                }
            }
            return false;
        }

        public virtual void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {
            CurrentCell = level.map.GetCell((int)Math.Floor(Origin.X / level.cellSize), (int)Math.Floor(Origin.Y / level.cellSize));

            actionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsHitByProjectile(level, graphicsDevice))
            {
                this.Health = 0;
            }
            
            if (!Collision.isCharacterInBounds(this, level))
            {
                Collision.Unstuck(this, level);
            }
            

            if (actionTimer > timeBetweenActions)
            {
                currentDirection = Global.random.Next(4) + 1;
                actionTimer = 0;
            }
                
            if (map != level.map)
            {
                PathFinder = new PathFinder(level.map);
            }
            map = level.map;

            // Random movement when player is not in enemies fov
            if (!map.IsInFov(x, y))
            {
                if (!Collision.checkCollisionInGivenDirection(currentDirection, this, level, graphicsDevice))
                {
                    Move(currentDirection, Speed, level, graphicsDevice);
                }
                else
                {
                    currentDirection = Opposite(currentDirection);
                    if (!Collision.checkCollisionInGivenDirection(currentDirection, this, level, graphicsDevice))
                    {
                        Move(currentDirection, Speed, level, graphicsDevice);
                    }
                }
            }
            else
            {
                if (CurrentCell.X == NextCell.X && CurrentCell.Y == NextCell.Y)
                {
                    if (CurrentCell.X != level.player.CurrentCell.X || CurrentCell.Y != level.player.CurrentCell.Y)
                    {
                        Path path;
                        try
                        {
                            path = PathFinder.ShortestPath(level.map.GetCell(CurrentCell.X, CurrentCell.Y), level.map.GetCell(level.player.CurrentCell.X, level.player.CurrentCell.Y));
                        }
                        catch (PathNotFoundException)
                        {
                            if (!Collision.checkCollisionInGivenDirection(currentDirection, this, level, graphicsDevice))
                            {
                                Move(currentDirection, Speed, level, graphicsDevice);
                            }
                            else
                            {
                                currentDirection = Opposite(currentDirection);
                                if (!Collision.checkCollisionInGivenDirection(currentDirection, this, level, graphicsDevice))
                                {
                                    Move(currentDirection, Speed, level, graphicsDevice);
                                }
                            }
                        }
                        NextCell = path.StepForward();
                        CellToReach = path.Start;

                    }
                }
                else
                {
                    //attack
                }
                
            }

            SetAnimations();
            _animationManager.Update(gameTime);
            Position += Velocity;
            Velocity = Vector2.Zero;
        }

        private int Opposite(int currentDirection)
        {
            if (currentDirection == 1)
            {
                return 2;
            }
            else if (currentDirection == 2)
            {
                return 1;
            }
            else if (currentDirection == 3)
            {
                return 4;
            }
            else return 3;
        }

        private void Move(int currentDirection, float speed, Level level, GraphicsDevice graphicsDevice)
        {
            if (currentDirection == (int)Directions.Top)
                Velocity.Y = -Speed;

            if (currentDirection == (int)Directions.Bottom)
                Velocity.Y = +Speed;

            if (currentDirection == (int)Directions.Left)
                Velocity.X = -Speed;

            if (currentDirection == (int)Directions.Right)
                Velocity.X = +Speed;

            if (currentDirection == (int)Directions.TopLeft)
            {
                Velocity.X = -Speed;
                Velocity.Y = -Speed;
            }

            if (currentDirection == (int)Directions.TopRight)
            {
                Velocity.X = +Speed;
                Velocity.Y = -Speed;
            }

            if (currentDirection == (int)Directions.BottomLeft)
            {
                Velocity.X = -Speed;
                Velocity.Y = +Speed;
            }

            if (currentDirection == (int)Directions.BottomRight)
            {
                Velocity.X = +Speed;
                Velocity.Y = +Speed;
            }
        }
    }
}
