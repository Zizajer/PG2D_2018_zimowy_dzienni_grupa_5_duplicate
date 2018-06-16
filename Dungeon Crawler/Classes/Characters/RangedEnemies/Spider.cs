using System.Collections.Generic;
using RogueSharp;
using System;
using RoyT.AStar;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon_Crawler
{
    public class Spider : Enemy
    {
        public Spider(Dictionary<string, Animation> _animations, int cellSize, int level, float speed, float timeBetweenActions, Map map, string name) : base(_animations, cellSize, level, speed, timeBetweenActions, map, name)
        {
        }
        public override void setAttacks()
        {
            RangeAttack = new WebAttack();
        }
        public override void calculateBaseStatistics()
        {
            Health = CurrentHealth = 10 + Level * 10;
            Defense = 30 + Level * 3;
            SpDefense = 50 + Level * 5;
            Attack = (int)Math.Floor(35 + Level * 2.5f);
            SpAttack = 50 + Level * 3;
            Experience = 50 + Level * 5;
            Speed = 2f;

            timeBetweenActions = 2f;
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
                    if (Vector2.Distance(Center, level.player.Center) > level.cellSize * 3.7f)
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
                                RangeAttack.Use(this, level.player.Center);
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
    }
}
