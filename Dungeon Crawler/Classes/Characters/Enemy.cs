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

        int currentDirection;
        //Random movement vars
        float actionTimer;
        float timeBetweenActions;

        //Pathfinding vars
        PathFinder PathFinder;
        Path path;
        Cell NextCell;
        Map map;

        public Enemy(Dictionary<string, Animation> _animations, int cellSize, float speed, float timeBetweenActions,Map map)
        {
            Health = 100;
            this._animations = _animations;
            this.timeBetweenActions = timeBetweenActions;
            Speed = speed;
            _animationManager = new AnimationManager(_animations.First().Value);
            actionTimer = 0;
            currentDirection= Global.random.Next(4) + 1;
            this.map = map;
            PathFinder = new PathFinder(map);
            x = (int)Math.Floor(Origin.X / cellSize);
            y = (int)Math.Floor(Origin.Y / cellSize);
            CurrentCell = map.GetCell(x, y);
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

        //Bounces character off the object in clockwise direction (if not clockwise, will try to bounce off another enemy by moving either left or right when direction is up/down
        //or either up or down when direction is left/right
        public int BounceOffObject(int direction, Boolean clockwise)
        {
            if (clockwise)
            {
                if (direction == 1)
                {
                    return 4;
                }
                else if (direction == 2)
                {
                    return 3;
                }
                else if (direction == 3)
                {
                    return 1;
                }
                else return 2;
            }
            else
            {
                if (direction == 1 || direction == 2)
                {
                    return Global.random.Next(1) + 3;
                }
                else
                {
                    return Global.random.Next(1) + 1;
                }
            }
        }

        public virtual void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {
            if (map != level.map)
            {
                PathFinder = new PathFinder(level.map);
            }

            x = (int)Math.Floor(Origin.X / level.cellSize);
            y = (int)Math.Floor(Origin.Y / level.cellSize);
            CurrentCell = level.map.GetCell(x, y);
            //Console.WriteLine("{0} {1}", x, y);

            actionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsHitByProjectile(level, graphicsDevice))
            {
                Health = 0;
            }

            if (actionTimer > timeBetweenActions)
            {
                currentDirection = Global.random.Next(4) + 1;
                actionTimer = 0;
            }

            // Random movement when player is not in enemies fov
            if (!map.IsInFov(x, y) && Vector2.Distance(Origin, level.player.Origin) > 3* level.cellSize)
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
                if (Vector2.Distance(Origin, level.player.Origin) > level.cellSize*2/3)
                {
                    //move
                    if (Vector2.Distance(Origin, level.player.Origin) > 2* level.cellSize)
                    {
                        //Console.WriteLine("Moving using algorithm");

                        if (x != level.player.x || y != level.player.y)
                        {
                            if (NextCell == null || x == NextCell.X && y == NextCell.Y)
                            {
                                try
                                {
                                    //path = PathFinder.ShortestPath(CurrentCell, level.player.CurrentCell);
                                    path = PathFinder.ShortestPath(level.map.GetCell(x, y), level.map.GetCell(level.player.x, level.player.y));
                                }
                                catch (PathNotFoundException)
                                {
                                }
                            
                                if (path.Length > 1)
                                    NextCell = path.Start;
                                else
                                    NextCell = level.player.CurrentCell;
                            }
                            
                            if (NextCell.Y < y)
                                if (!Collision.checkCollisionInGivenDirection(1, this, level, graphicsDevice))
                                    Move(1, Speed, level, graphicsDevice);

                            if (NextCell.Y > y)
                                if (!Collision.checkCollisionInGivenDirection(2, this, level, graphicsDevice))
                                    Move(2, Speed, level, graphicsDevice);
                            
                            if (NextCell.X < x)
                                if (!Collision.checkCollisionInGivenDirection(3, this, level, graphicsDevice))
                                    Move(3, Speed, level, graphicsDevice);

                            if (NextCell.X > x)
                                if (!Collision.checkCollisionInGivenDirection(4, this, level, graphicsDevice))
                                    Move(4, Speed, level, graphicsDevice);
                            
                        }
                    }
                    else
                    {
                        //Console.WriteLine("Moving in cell");
                        if (level.player.Origin.Y < Origin.Y)
                        {
                            if (!Collision.checkCollisionInGivenDirection(1, this, level, graphicsDevice))
                                Move(1, Speed, level, graphicsDevice);
                        }
                        if (level.player.Origin.Y > Origin.Y)
                        {
                            if (!Collision.checkCollisionInGivenDirection(2, this, level, graphicsDevice))
                                Move(2, Speed, level, graphicsDevice);
                        }
                        if (level.player.Origin.X < Origin.X)
                        {
                            if (!Collision.checkCollisionInGivenDirection(3, this, level, graphicsDevice))
                                Move(3, Speed, level, graphicsDevice);
                        }
                        if (level.player.Origin.X > Origin.X)
                        {
                            if (!Collision.checkCollisionInGivenDirection(4, this, level, graphicsDevice))
                                Move(4, Speed, level, graphicsDevice);
                        }
                    }
                        
                }
                else
                {
                    Console.WriteLine("Attacking"); 
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
