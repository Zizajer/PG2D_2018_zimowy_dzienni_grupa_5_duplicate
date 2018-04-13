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

        Directions currentDirection;
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
            currentDirection= (Directions)Global.random.Next(4) + 1;
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

            if (currentState == State.RandomMovement)
            {
                if (!map.IsInFov(x, y))
                {
                    if (actionTimer > timeBetweenActions)
                    {
                        actionTimer = 0;
                        currentDirection = (Directions)Global.random.Next(3) + 1;
                    }
                    else
                    {
                        if (!Collision.checkCollisionInGivenDirection(currentDirection, this, level, graphicsDevice))
                            Move(currentDirection, Speed, level, graphicsDevice);
                        else
                        {
                            currentDirection = Opposite(currentDirection);
                            if (!Collision.checkCollisionInGivenDirection(currentDirection, this, level, graphicsDevice))
                                Move(currentDirection, Speed, level, graphicsDevice);
                        }
                        int newx = (int)Math.Floor((Origin.X + Velocity.X) / level.cellSize);
                        int newy = (int)Math.Floor((Origin.Y + Velocity.Y) / level.cellSize);

                        if (newx != x || newy != y)
                            currentState = State.CenterMovement;
                    }
                }
                else
                {
                    currentState = State.MovingToAnotherCell;
                }
                   
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
                if (map.IsInFov(x, y))
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
                else
                {
                    currentState = State.RandomMovement;
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
                    catch (ArgumentNullException)
                    {
                        //next line should take care of that but just in case lets catch it
                    }

                    //this fixes Exception thrown by ShorthestPath method, not sure why (that method even throws it)
                    if (path.Length > 1)
                        NextCell = path.Start;
                    else
                        NextCell = level.player.CurrentCell;
                }
                if (NextCell.Y < y)
                {
                    currentDirection = Directions.Top;
                    CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
                    return;
                } 
                if (NextCell.Y > y)
                {
                    currentDirection = Directions.Bottom;
                    CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
                    return;
                }
                if (NextCell.X < x)
                {
                    currentDirection = Directions.Left;
                    CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
                    return;
                }
                if (NextCell.X > x)
                {
                    currentDirection = Directions.Right;
                    CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
                    return;
                }        
            }
        }

        private void CollisionAvoidingMove(Directions currentDirection, float speed, Level level, GraphicsDevice graphicsDevice)
        {
            Microsoft.Xna.Framework.Rectangle characterRectangle = getRectangle();
            if (currentDirection == Directions.Top)
                characterRectangle.Y -= (int)speed;
            if (currentDirection == Directions.Bottom)
                characterRectangle.Y += (int)speed;
            if (currentDirection == Directions.Left)
                characterRectangle.X -= (int)speed;
            if (currentDirection == Directions.Right)
                characterRectangle.X += (int)speed;


            if (!Collision.isCollidingWithObstacles(characterRectangle, this, level, graphicsDevice))
            {
                if (!Collision.isCollidingWithPlayer(characterRectangle, this, level, graphicsDevice))
                {
                    if(!Collision.isCollidingWithEnemies(characterRectangle, this, level, graphicsDevice))
                    {
                        Move(currentDirection, Speed, level, graphicsDevice);
                    }
                    else
                    {
                        currentDirection = BounceOffObject(currentDirection, false);
                        Move(currentDirection, Speed, level, graphicsDevice);
                    }
                }
            }
            else
            {
                //avoiding rocks logic
            }
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
                    currentDirection = Directions.Top;
                    CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
                }
                else
                {
                    currentDirection = Directions.Bottom;
                    CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
                }
                    
            }

            if (Math.Abs(Origin.X - PosX) > Speed)
            {
                if (Origin.X >= x * level.cellSize + level.cellSize / 2)
                {
                    currentDirection = Directions.Left;
                    CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
                }
                else
                {
                    currentDirection = Directions.Right;
                    CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
                }
                    
            }
        }
        //Bounces character off the object in clockwise direction (if not clockwise, will try to bounce off another enemy by moving either left or right when direction is up/down
        //or either up or down when direction is left/right
        public Directions BounceOffObject(Directions direction, Boolean clockwise)
        {
            if (clockwise)
            {
                if (direction == Directions.Top)
                {
                    return Directions.Right;
                }
                else if (direction == Directions.Bottom)
                {
                    return Directions.Left;
                }
                else if (direction == Directions.Left)
                {
                    return Directions.Top;
                }
                else return Directions.Bottom;
            }
            else
            {
                if (direction == Directions.Top || direction == Directions.Bottom)
                {
                    return (Directions)Global.random.Next(1) + 3;
                }
                else
                {
                    return (Directions)Global.random.Next(1) + 1;
                }
            }
        }

        private Directions Opposite(Directions currentDirection)
        {
            if (currentDirection == Directions.Top)
            {
                return Directions.Bottom;
            }
            else if (currentDirection == Directions.Bottom)
            {
                return Directions.Top;
            }
            else if (currentDirection == Directions.Left)
            {
                return Directions.Right;
            }
            else return Directions.Left;
        }

        private void Move(Directions currentDirection, float speed, Level level, GraphicsDevice graphicsDevice)
        {
            if (currentDirection == Directions.Top)
                Velocity.Y = -Speed;

            if (currentDirection == Directions.Bottom)
                Velocity.Y = +Speed;

            if (currentDirection == Directions.Left)
                Velocity.X = -Speed;

            if (currentDirection == Directions.Right)
                Velocity.X = +Speed;

            if (currentDirection == Directions.TopLeft)
            {
                Velocity.X = -Speed;
                Velocity.Y = -Speed;
            }

            if (currentDirection == Directions.TopRight)
            {
                Velocity.X = +Speed;
                Velocity.Y = -Speed;
            }

            if (currentDirection == Directions.BottomLeft)
            {
                Velocity.X = -Speed;
                Velocity.Y = +Speed;
            }

            if (currentDirection == Directions.BottomRight)
            {
                Velocity.X = +Speed;
                Velocity.Y = +Speed;
            }
        }
    }
}
