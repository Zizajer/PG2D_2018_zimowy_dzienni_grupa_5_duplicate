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
        public enum State { RandomMovement, CenterMovement, MovingToAnotherCell, Attacking };
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

        State currentState;

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
            currentState = State.CenterMovement;
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

            if (currentState == State.RandomMovement)
            {
                Console.WriteLine("RandomMovement");
            }
            else if (currentState == State.CenterMovement)
            {
                if (isCenter(level, graphicsDevice))
                {
                    currentState = State.MovingToAnotherCell;
                }
                else
                {
                    MoveToCenter(level, graphicsDevice);
                }
                
            }
            else if (currentState == State.MovingToAnotherCell)
            {
                if (Vector2.Distance(Origin, level.player.Origin) > level.cellSize * 2 / 3)
                {
                    MoveToNextCell(level, graphicsDevice);
                    int newx = (int)Math.Floor((Origin.X + Velocity.X) / level.cellSize);
                    int newy = (int)Math.Floor((Origin.Y + Velocity.Y) / level.cellSize);

                    if (newx != x || newy != y)
                        currentState = State.CenterMovement;
                        
                }
                else
                {
                    currentState = State.Attacking;
                }

            }
            else if (currentState == State.Attacking)
            {
                if (Vector2.Distance(Origin, level.player.Origin) > level.cellSize * 2/3)
                {
                    currentState = State.MovingToAnotherCell;
                }
                else
                {
                    //attack
                }
            }
            else
            {
                Console.WriteLine("Error");
            }          
            
            SetAnimations();
            _animationManager.Update(gameTime);
            Position += Velocity;
            Velocity = Vector2.Zero;
        }

        private void MoveToNextCell(Level level, GraphicsDevice graphicsDevice)
        {
            if (x != level.player.x || y != level.player.y)
            {
                if (NextCell == null || x == NextCell.X && y == NextCell.Y)
                {
                    try
                    {
                        path = PathFinder.ShortestPath(level.map.GetCell(x, y), level.map.GetCell(level.player.x, level.player.y));
                    }
                    catch (PathNotFoundException)
                    {
                        currentState = State.RandomMovement;
                    }

                    if (path.Length > 1)
                        NextCell = path.Start;
                    else
                        NextCell = level.player.CurrentCell;
                }
                
                if (NextCell.Y < y)
                {
                    currentDirection = 1;
                    CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
                    return;
                }
                    

                if (NextCell.Y > y)
                {
                    currentDirection = 2;
                    CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
                    return;
                }
                    

                if (NextCell.X < x)
                {
                    currentDirection = 3;
                    CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
                    return;
                }
                    

                if (NextCell.X > x)
                {
                    currentDirection = 4;
                    CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
                    return;
                }
                    
            }

        }

        private void CollisionAvoidingMove(int currentDirection, float speed, Level level, GraphicsDevice graphicsDevice)
        {
            Microsoft.Xna.Framework.Rectangle characterRectangle = getRectangle();
            if (currentDirection == (int)Character.Directions.Top)
                characterRectangle.Y -= (int)speed;
            if (currentDirection == (int)Character.Directions.Bottom)
                characterRectangle.Y += (int)speed;
            if (currentDirection == (int)Character.Directions.Left)
                characterRectangle.X -= (int)speed;
            if (currentDirection == (int)Character.Directions.Right)
                characterRectangle.X += (int)speed;


            if (!Collision.isCollidingWithObstacles(characterRectangle, this, level, graphicsDevice))
                if (!Collision.isCollidingWithPlayer(characterRectangle, this, level, graphicsDevice))
                    Move(currentDirection, Speed, level, graphicsDevice);
          
        }

        private bool isCenter(Level level, GraphicsDevice graphicsDevice)
        {
            int PosX = x * level.cellSize + level.cellSize / 2;
            int PosY = y * level.cellSize + level.cellSize / 2;

            if (Math.Abs(Origin.Y - PosY) <= Speed && Math.Abs(Origin.X - PosX) <= Speed)
                return true;
            else
                return false;
        }

        private void MoveToCenter(Level level, GraphicsDevice graphicsDevice)
        {
            int PosX = x * level.cellSize + level.cellSize / 2;
            int PosY = y * level.cellSize + level.cellSize / 2;

            if (Math.Abs(Origin.Y - PosY) > Speed)
            {
                if (Origin.Y >= y * level.cellSize + level.cellSize / 2)
                {
                    currentDirection = 1;
                    CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
                }
                else
                {
                    currentDirection = 2;
                    CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
                }
                    
            }

            if (Math.Abs(Origin.X - PosX) > Speed)
            {
                if (Origin.X >= x * level.cellSize + level.cellSize / 2)
                {
                    currentDirection = 3;
                    CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
                }
                else
                {
                    currentDirection = 4;
                    CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
                }
                    
            }
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
