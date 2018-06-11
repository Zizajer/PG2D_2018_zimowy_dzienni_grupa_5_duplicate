using System.Collections.Generic;
using RogueSharp;
using System;
using RoyT.AStar;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Dungeon_Crawler
{
    public class BlackKnight : Boss
    {
        public new enum ActionState { Moving, Standing };
        public new ActionState currentActionState;
        Position[] path;

        public BlackKnight(Dictionary<string, Animation> _animations, int cellSize, int level, float timeBetweenActions, Map map, List<Cell> cells) : base(_animations, cellSize, level, timeBetweenActions, map, cells)
        {
            CellX = (int)Math.Floor(Center.X / cellSize);
            CellY = (int)Math.Floor(Center.Y / cellSize);
            CurrentCell = map.GetCell(CellX, CellY);
            currentActionState = ActionState.Standing;
            currentHealthState = HealthState.Normal;
            isBlackShaderOn = true;
        }

        public override void calculateBaseStatistics()
        {
            Health = CurrentHealth = 110 + Level * 10;
            Defense = 210 + Level * 3;
            SpDefense = 210 + Level * 5;
            Attack = (int)Math.Floor(210 + Level * 2.5);
            SpAttack = 210 + Level * 3;
            Speed = 1f;
            Experience = 250 + Level * 5;
        }
        public override void SetAttack()
        {
            TargetedAttack = new Exori();
            TargetedAttack2 = new DeathBeam();
        }
        public override void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {
            HandleHitState(gameTime);
            HandleHealthState(gameTime);

            CellX = (int)Math.Floor(Center.X / level.cellSize);
            CellY = (int)Math.Floor(Center.Y / level.cellSize);
            if (CellX > 0 && CellX < level.map.Width && CellY > 0 && CellY < level.map.Height)
            {
                CurrentCell = level.map.GetCell(CellX, CellY);
            }

            if (currentHealthState != HealthState.Freeze)
            {
                if (currentActionState == ActionState.Standing)
                {
                    Cell futureNextCell;
                    if (Vector2.Distance(Center, level.player.Center) > level.cellSize * 1.5f)
                    {
                        if (Global.random.Next(100) > 50) TargetedAttack2.Use(this);
                        if (Global.random.Next(100) > 75) TargetedAttack.Use(this);
                        if (level.map.IsInFov(CellX, CellY))
                        {//can see player
                            futureNextCell = getNextCellFromPath(level);
                            if (futureNextCell != null && CellX > 0 && CellX < level.map.Width && CellY > 0 && CellY < level.map.Height)
                            {
                                if (!Collision.checkCollisionInGivenCell(futureNextCell, level, graphicsDevice))
                                {
                                    NextCell = futureNextCell;
                                    currentActionState = ActionState.Moving;
                                    level.grid.SetCellCost(new Position(CurrentCell.X, CurrentCell.Y), 1.0f);
                                    level.grid.SetCellCost(new Position(NextCell.X, NextCell.Y), 5.0f);
                                }
                            }
                        }
                        else
                        {//cant see player
                            futureNextCell = getRandomEmptyCell(level, graphicsDevice);
                            if (futureNextCell != null)
                            {
                                NextCell = futureNextCell;
                                currentActionState = ActionState.Moving;
                                level.grid.SetCellCost(new Position(CurrentCell.X, CurrentCell.Y), 1.0f);
                                level.grid.SetCellCost(new Position(NextCell.X, NextCell.Y), 5.0f);
                            }
                        }
                    }
                    else
                    {
                        if (level.map.IsInFov(CellX, CellY))
                        {
                            if (Global.random.Next(100) > 98) TargetedAttack.Use(this);
                            if (Global.random.Next(100) > 98) TargetedAttack2.Use(this);
                        }
                        else
                        {
                            futureNextCell = getRandomEmptyCell(level, graphicsDevice);
                            if (futureNextCell != null)
                            {
                                NextCell = futureNextCell;
                                currentActionState = ActionState.Moving;
                                level.grid.SetCellCost(new Position(CurrentCell.X, CurrentCell.Y), 1.0f);
                                level.grid.SetCellCost(new Position(NextCell.X, NextCell.Y), 5.0f);
                            }
                        }
                    }
                }
                else //Moving
                {
                    if (isCenterOfGivenCell(NextCell, level, graphicsDevice))
                    {
                        currentActionState = ActionState.Standing;
                    }
                    else
                    {
                        MoveToCenterOfGivenCell(NextCell, level, graphicsDevice);
                    }
                }
            }
            
            SetAnimations();
            _animationManager.Update(gameTime);
            Position += Velocity;
            Velocity = Vector2.Zero;
        }

        private Cell getNextCellFromPath(Level level)
        {
            if (CellX != level.player.CellX || CellY != level.player.CellY)
            {
                if (NextCell == null || CellX == NextCell.X && CellY == NextCell.Y)
                {
                    path = level.grid.GetPath(new Position(CellX, CellY), new Position(level.player.CellX, level.player.CellY), MovementPatterns.Full);
                    if (path == null)
                    {
                        currentActionState = ActionState.Standing;
                        return null;
                    }
                    else
                    {
                        if (path.Length > 0)
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
        protected override void SetAnimations()
        {
            if (Velocity.X > 0)
            {
                _animationManager.Play(_animations["WalkRight"]);
                currentFaceDirection = FaceDirections.Right;
            }
            else if (Velocity.X < 0)
            {
                _animationManager.Play(_animations["WalkLeft"]);
                currentFaceDirection = FaceDirections.Left;
            }
            else if (Velocity.Y < 0)
            {
                _animationManager.Play(_animations["WalkUp"]);
                currentFaceDirection = FaceDirections.Up;
            }
            else if (Velocity.Y > 0)
            {
                _animationManager.Play(_animations["WalkDown"]);
                currentFaceDirection = FaceDirections.Down;
            }
            else
                _animationManager.Stop();
        }
    }
}

