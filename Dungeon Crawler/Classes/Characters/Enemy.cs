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
        public enum State { Moving, Standing };

        public int prevX { get; set; }
        public int prevY { get; set; }

        Directions currentDirection;
        float timeBetweenActions;

        Position[] path;
        Map map;

        State currentState;

        public Enemy(Dictionary<string, Animation> _animations, int cellSize, float speed, float timeBetweenActions, Map map)
        {
            Health = 100;
            this._animations = _animations;
            this.timeBetweenActions = timeBetweenActions;
            Speed = speed;
            _animationManager = new AnimationManager(_animations.First().Value);
            currentDirection= (Directions)Global.random.Next(4) + 1;
            this.map = map;
            x = (int)Math.Floor(Center.X / cellSize);
            y = (int)Math.Floor(Center.Y / cellSize);
            CurrentCell = map.GetCell(x, y);
            currentState = State.Standing;
            prevX = x;
            prevY = y;
        }

        public bool IsHitByProjectile(Level level, GraphicsDevice graphicsDevice)
        {
            PlayerProjectile projectile = null;
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
            x = (int)Math.Floor(Center.X / level.cellSize);
            y = (int)Math.Floor(Center.Y / level.cellSize);
            if (x > 0 && x < level.map.Width && y > 0 && y < level.map.Height)
            {
                CurrentCell = level.map.GetCell(x, y);
            }

            if (currentState == State.Standing)
            {
                Cell futureNextCell;
                if (Vector2.Distance(Center, level.player.Center) > level.cellSize * 1.5f)
                {
                    if (map.IsInFov(x, y))
                    {//can see player
                        futureNextCell = getNextCellFromPath(level);
                        if (futureNextCell != null && x > 0 && x < level.map.Width && y > 0 && y < level.map.Height)
                        {
                            if (!Collision.checkCollisionInGivenCell(futureNextCell, level, graphicsDevice))
                            {
                                NextCell = futureNextCell;
                                currentState = State.Moving;
                                level.grid.SetCellCost(new Position(CurrentCell.X, CurrentCell.Y), 1.0f);
                                level.grid.SetCellCost(new Position(NextCell.X, NextCell.Y), 5.0f);
                            }
                        }
                    }
                    else
                    {//cant see player
                        futureNextCell = getRandomEmptyCell(level, graphicsDevice);
                        if(futureNextCell != null)
                        {
                            NextCell = futureNextCell;
                            currentState = State.Moving;
                            level.grid.SetCellCost(new Position(CurrentCell.X, CurrentCell.Y), 1.0f);
                            level.grid.SetCellCost(new Position(NextCell.X, NextCell.Y), 5.0f);
                        }
                            
                    }
                }
                else
                {
                    //attack
                }
            }
            else //Moving
            {
                if (isCenterOfGivenCell(NextCell, level, graphicsDevice))
                {
                    currentState = State.Standing;
                }
                else
                {
                    MoveToCenterOfGivenCell(NextCell, level, graphicsDevice);
                }
            }

            if (IsHitByProjectile(level, graphicsDevice))
            {
                Health = 0;
                level.grid.SetCellCost(new Position(CurrentCell.X, CurrentCell.Y), 1.0f);
                if (NextCell != null)
                    level.grid.SetCellCost(new Position(NextCell.X, NextCell.Y), 1.0f);
            }

            SetAnimations();
            _animationManager.Update(gameTime);
            Position += Velocity;
            Velocity = Vector2.Zero;
        }

        private Cell getNextCellFromPath(Level level)
        {
            if (x != level.player.x || y != level.player.y)
            {
                if (NextCell == null || x == NextCell.X && y == NextCell.Y)
                { 
                    path = level.grid.GetPath(new Position(x, y), new Position(level.player.x, level.player.y), MovementPatterns.Full);
                    if (path == null)
                    {
                        currentState = State.Standing;
                        return null;
                    }
                    else
                    {
                        if (path.Length>0)
                        {
                            int x = path[1].X;
                            int y = path[1].Y;
                            return level.map.GetCell(x, y);
                        }
                    }
                }
            }
            return null;
        }

        private Cell getRandomEmptyCell(Level level, GraphicsDevice graphicsDevice)
        {
            Directions tempDirection = (Directions)Global.random.Next(4) + 1;
            RogueSharp.Cell futureNextCell = Collision.getCellFromDirection(CurrentCell, tempDirection, level);
            if (!Collision.checkCollisionInGivenCell(futureNextCell, level, graphicsDevice))
            {
                return futureNextCell;
            }
            return null;
        }
 

        private bool isCenterOfGivenCell(RogueSharp.Cell NextCell, Level level, GraphicsDevice graphicsDevice)
        {
            int PosX = NextCell.X * level.cellSize + level.cellSize / 2;
            int PosY = NextCell.Y * level.cellSize + level.cellSize / 2;

            if (Math.Abs(Center.Y - PosY) <= Speed && Math.Abs(Center.X - PosX) <= Speed)
                return true;
            else
                return false;
        }

        private void MoveToCenterOfGivenCell(RogueSharp.Cell NextCell, Level level, GraphicsDevice graphicsDevice)
        {
            int PosX = NextCell.X * level.cellSize + level.cellSize / 2;
            int PosY = NextCell.Y * level.cellSize + level.cellSize / 2;

            if (Math.Abs(Center.Y - PosY) > Speed)
            {
                if (Center.Y - PosY > Speed)
                {
                    Move(Directions.Top, level, graphicsDevice);
                }
                if (Center.Y - PosY < Speed)
                {
                    Move(Directions.Bottom, level, graphicsDevice);
                }
            }

            if (Math.Abs(Center.X - PosX) > Speed)
            {
                if (Center.X - PosX > Speed)
                {
                    Move(Directions.Left, level, graphicsDevice);
                }
                if (Center.X - PosX < Speed)
                {
                    Move(Directions.Right, level, graphicsDevice);
                }

            }
        }

        private void Move(Directions currentDirection, Level level, GraphicsDevice graphicsDevice)
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
