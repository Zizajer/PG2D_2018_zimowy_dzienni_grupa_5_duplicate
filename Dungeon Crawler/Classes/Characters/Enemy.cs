using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using RoyT.AStar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon_Crawler
{
    public class Enemy : Character
    {
        public enum State { RandomMovement, CenterMovement, MovingToAnotherCell, Attacking, Unstucking};

        public int prevX { get; set; }
        public int prevY { get; set; }
        public int x { get; set; }
        public int y { get; set; }

        Directions currentDirection;
        //Random movement vars
        float actionTimer;
        float timeBetweenActions;
        float unstuckTimer;
        float timeBetweenUnstuck;

        //Pathfinding vars
        Position[] path;
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
            x = (int)Math.Floor(Center.X / cellSize);
            y = (int)Math.Floor(Center.Y / cellSize);
            CurrentCell = map.GetCell(x, y);
            currentState = State.CenterMovement;
            prevX = x;
            prevY = y;
            unstuckTimer=0;
            timeBetweenUnstuck = 2;
        }

        public bool IsHitByProjectile(Level level, GraphicsDevice graphicsDevice)
        {
            Projectile projectile;
            for (int i = 0; i < level.projectiles.Count; i++)
            {
                projectile = level.projectiles[i];
                if (Collision.checkCollision(getRectangle(),this, projectile, graphicsDevice))
                {
                    projectile.isEnemyHit = true;
                    return true;
                }
            }
            return false;
        }

        public virtual void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {
            x = (int)Math.Floor(Center.X / level.cellSize);
            y = (int)Math.Floor(Center.Y / level.cellSize);

            if(prevX!=x || prevY != y)
            {
                if (x >= 0 && x < level.map.Width && y >= 0 && y < level.map.Height)
                {
                    level.grid.SetCellCost(new Position(prevX, prevY), 1.0f);
                    level.grid.SetCellCost(new Position(x, y), 5.0f);
                    //level.grid.UnblockCell(new Position(prevX, prevY));
                    //level.grid.BlockCell(new Position(x, y));
                }

            }
            
            actionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsHitByProjectile(level, graphicsDevice))
            {
                Health = 0;
            }

            if (currentState == State.RandomMovement)
            {
               // if (!map.IsInFov(x, y))
                {
                    if (actionTimer > timeBetweenActions)
                    {
                        actionTimer = 0;
                        currentDirection = (Directions)Global.random.Next(3) + 1;
                    }
                    else
                    {
                        CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
                        
                        int newx = (int)Math.Floor((Center.X + Velocity.X) / level.cellSize);
                        int newy = (int)Math.Floor((Center.Y + Velocity.Y) / level.cellSize);

                        if (newx != x || newy != y)
                            currentState = State.CenterMovement;
                    }
                }
                //else
                {
                    //currentState = State.MovingToAnotherCell;
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
                //if (map.IsInFov(x, y))
                {
                    if (Vector2.Distance(Center, level.player.Center) > level.cellSize * 2 / 3)
                    {
                        MoveToNextCell(level, graphicsDevice);
                        int newx = (int)Math.Floor((Center.X + Velocity.X) / level.cellSize);
                        int newy = (int)Math.Floor((Center.Y + Velocity.Y) / level.cellSize);

                        if (newx != x || newy != y)
                            currentState = State.CenterMovement;

                    }
                    else
                    {
                        //atack
                    }
                }
                //else
                {
                   // currentState = State.RandomMovement;
                }
            }
            else if (currentState == State.Attacking)
            {
                if (Vector2.Distance(Center, level.player.Center) > level.cellSize * 2/3)
                {
                    currentState = State.MovingToAnotherCell;
                }
                else
                {
                    //attack
                }
            }
            else if (currentState == State.Unstucking)
            {
                if (Vector2.Distance(Center, level.player.Center) < level.cellSize * 2 / 3)
                {
                    currentState = State.Attacking;
                }

                currentDirection = (Directions)Global.random.Next(4) + 1;
                unstuckTimer += 0.2f;
                if (unstuckTimer < timeBetweenUnstuck)
                {
                    CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
                    if ((int)Math.Floor((Center.X + Velocity.X) / level.cellSize) != x || (int)Math.Floor((Center.Y + Velocity.Y) / level.cellSize) != y)
                        currentState = State.CenterMovement;
                }
                else
                {
                    currentDirection = (Directions)Global.random.Next(4) + 1;
                    unstuckTimer = 0;
                }
                
            }
            else
            {
                Console.WriteLine("Error");
            }

            prevX = x;
            prevY = y;
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
                    path = level.grid.GetPath(new Position(x, y), new Position(level.player.x, level.player.y), MovementPatterns.LateralOnly);
                    if (path == null)
                    {
                        currentState = State.RandomMovement;
                        return;
                    }
                    else
                    {
                        if (path.Length>0)
                        {
                            int x = path[1].X;
                            int y = path[1].Y;
                            NextCell = level.map.GetCell(x, y);
                        }
                    }
                }
                else
                {
                    if (NextCell.Y < y)
                    {
                        currentDirection = Directions.Top;
                    } 
                    if (NextCell.Y > y)
                    {
                        currentDirection = Directions.Bottom;
                    }
                    if (NextCell.X < x)
                    {
                        currentDirection = Directions.Left;
                    }
                    if (NextCell.X > x)
                    {
                        currentDirection = Directions.Right;
                    }
                    CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
                }
            }

        }

        private void CollisionAvoidingMove(Directions currentDirection, float speed, Level level, GraphicsDevice graphicsDevice)
        {
            if (!Collision.checkCollisionInGivenDirection(currentDirection, this, level, graphicsDevice))
            {
                Move(currentDirection, Speed, level, graphicsDevice);
            }
            else
            {
                if (unstuckTimer < timeBetweenUnstuck)
                {
                    unstuckTimer += 0.1f;
                }
                else
                {
                    unstuckTimer = 0;
                    currentState = State.Unstucking;
                }
            }
        }

        private bool isCenter(Level level, GraphicsDevice graphicsDevice)
        {
            int PosX = x * level.cellSize + level.cellSize / 2;
            int PosY = y * level.cellSize + level.cellSize / 2;

            if (Math.Abs(Center.Y - PosY) <= Speed && Math.Abs(Center.X - PosX) <= Speed)
                return true;
            else
                return false;
        }

        private void MoveToCenter(Level level, GraphicsDevice graphicsDevice)
        {
            int PosX = x * level.cellSize + level.cellSize / 2;
            int PosY = y * level.cellSize + level.cellSize / 2;

            if (Math.Abs(Center.Y - PosY) > Speed)
            {
                if (Center.Y >= y * level.cellSize + level.cellSize / 2)
                {
                    currentDirection = Directions.Top;
                }
                else
                {
                    currentDirection = Directions.Bottom;
                }
                    
            }
            CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
            if (Math.Abs(Center.X - PosX) > Speed)
            {
                if (Center.X >= x * level.cellSize + level.cellSize / 2)
                {
                    currentDirection = Directions.Left;
                }
                else
                {
                    currentDirection = Directions.Right;
                }  
            }
            CollisionAvoidingMove(currentDirection, Speed, level, graphicsDevice);
        }
        //Bounces character off the object in clockwise direction (if not clockwise, will try to bounce off another enemy by moving either left or right when direction is up/down
        //or either up or down when direction is left/right
        public Directions BounceOffObject(Directions direction, Level level, Boolean clockwise, GraphicsDevice graphicsDevice)
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
                for(Directions i = Directions.Top; i <= Directions.Right;i++)
                {
                    if (!Collision.checkCollisionInGivenDirection(currentDirection,this,level,graphicsDevice))
                    {
                        return i;
                    }
                }
            }
            return Directions.None;
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
