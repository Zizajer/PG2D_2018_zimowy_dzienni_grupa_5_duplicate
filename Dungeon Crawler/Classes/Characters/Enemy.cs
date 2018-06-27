using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using RoyT.AStar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon_Crawler
{
    public abstract class Enemy : Character
    {
        public float timeBetweenActions;
        public float timer;
        public Position[] path;

        public override IInventoryManager InventoryManager { get; }

        //Attacks
        public ICharacterTargetedAttack BaseAttack;
        public IPositionTargetedAttack RangeAttack;

        public Enemy(Dictionary<string, Animation> _animations, int cellSize, int level, float speed, float timeBetweenActions, Map map, String name)
        {
            Level = level;
            calculateBaseStatistics();

            this._animations = _animations;
            this.timeBetweenActions = timeBetweenActions;
            timer = 0;
            Speed = speed; // TODO: move it to calculateStatistics()
            _animationManager = new AnimationManager(_animations.First().Value);
            CellX = (int)Math.Floor(Center.X / cellSize);
            CellY = (int)Math.Floor(Center.Y / cellSize);
            CurrentCell = map.GetCell(CellX, CellY);
            currentActionState = ActionState.Standing;
            currentHealthState = HealthState.Normal;
            Name = name;
            setAttacks();

            InventoryManager = new InventoryManager(this, new List<Item>());
        }
        public abstract void setAttacks();

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
                            if (timer > timeBetweenActions)
                            {
                                BaseAttack.Use(this, level.player);
                                timer = 0;
                            }
                            else
                            {
                                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
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

        public Cell getNextCellFromPath(Level level)
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

        public Cell getRandomEmptyCell(Level level, GraphicsDevice graphicsDevice)
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
