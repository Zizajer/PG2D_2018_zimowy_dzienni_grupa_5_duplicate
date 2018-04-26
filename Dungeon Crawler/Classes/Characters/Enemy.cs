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
        float timeBetweenActions;
        float timer;
        Position[] path;

        public Enemy(Dictionary<string, Animation> _animations, int cellSize, float speed, float timeBetweenActions, Map map)
        {
            Health = 100;
            this._animations = _animations;
            this.timeBetweenActions = timeBetweenActions;
            timer = 0;
            Speed = speed;
            _animationManager = new AnimationManager(_animations.First().Value);
            x = (int)Math.Floor(Center.X / cellSize);
            y = (int)Math.Floor(Center.Y / cellSize);
            CurrentCell = map.GetCell(x, y);
            currentState = State.Standing;
            Name = "Blob";
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
                    if (level.map.IsInFov(x, y))
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
                    
                    if (timer > timeBetweenActions)
                    {
                        Global.CombatManager.Attack(this, level.player);
                        timer = 0;
                    }
                    else
                    {
                        timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    
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
            Cell futureNextCell = Collision.getCellFromDirection(CurrentCell, tempDirection, level);
            if (!Collision.checkCollisionInGivenCell(futureNextCell, level, graphicsDevice))
            {
                return futureNextCell;
            }
            return null;
        }
    }
}
