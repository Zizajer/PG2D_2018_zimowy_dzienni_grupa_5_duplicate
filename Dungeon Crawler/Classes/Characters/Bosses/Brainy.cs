using System.Collections.Generic;
using RogueSharp;
using System;
using RoyT.AStar;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Dungeon_Crawler
{
    public class Brainy : Boss
    {
        public new enum ActionState { Standing };
        public new ActionState currentActionState;
        public float teleportTimer;
        public float explosionTimer;
        public float teleportCooldown = 6f;
        public float explosionCooldown= 1.8f;

        public Brainy(Dictionary<string, Animation> _animations, int cellSize, int level, float timeBetweenActions, Map map, List<Cell> cells) : base(_animations, cellSize, level, timeBetweenActions, map, cells)
        {
            CellX = (int)Math.Floor(Center.X / cellSize);
            CellY = (int)Math.Floor(Center.Y / cellSize);
            CurrentCell = map.GetCell(CellX, CellY);
            currentActionState = ActionState.Standing;
            currentHealthState = HealthState.Normal;
        }

        public override void calculateBaseStatistics()
        {
            if (Global.hardMode == false)
            {
                Health = CurrentHealth = 80 + Level * 10;
                Defense = 210 + Level * 3;
                SpDefense = 210 + Level * 5;
                Attack = (int)Math.Floor(210 + Level * 2.5);
                SpAttack = 210 + Level * 3;
                Speed = 0f;
                Experience = 230 + Level * 5;
            }
            else
            {
                Health = CurrentHealth = 120 + Level * 10;
                Defense = 250 + Level * 3;
                SpDefense = 250 + Level * 5;
                Attack = (int)Math.Floor(210 + Level * 2.5);
                SpAttack = 210 + Level * 3;
                Speed = 0f;
                Experience = 220 + Level * 5;
            }
        }
        public override void SetAttack()
        {
            TargetedAttack = new AoeAttack();
        }
        public override void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {
            HandleHitState(gameTime);
            HandleHealthState(gameTime);

            teleportTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            explosionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            CellX = (int)Math.Floor(Center.X / level.cellSize);
            CellY = (int)Math.Floor(Center.Y / level.cellSize);
            if (CellX > 0 && CellX < level.map.Width && CellY > 0 && CellY < level.map.Height)
            {
                CurrentCell = level.map.GetCell(CellX, CellY);
            }

            if (currentHealthState != HealthState.Freeze)
            {
                if (level.map.IsInFov(CellX, CellY))
                {//can see player
                    if (explosionTimer > explosionCooldown)
                    {
                        explosionTimer = 0;
                        TargetedAttack.Use(this);
                    }
                    if (teleportTimer > teleportCooldown)
                    {
                        teleportTimer = 0;
                        Teleport(level, graphicsDevice);

                    }
                }
            }
            
            SetAnimations();
        }

        public void Teleport(Level level, GraphicsDevice graphicsDevice)
        {
            
            Vector2 tempPos = getRandomEmptyCell(level);

            int mx = (int)tempPos.X;
            int my = (int)tempPos.Y;

            if (mx < 0 || mx >= level.map.Width || my < 0 || my >= level.map.Height)
                return;

            if (level.grid.GetCellCost(new Position(mx, my)) == 1.0f)
            {
                //play blue particle
                Global.CombatManager.SetAnimation("Teleportation", "MagicAnim", CellX, CellY);
                level.grid.SetCellCost(new Position(CurrentCell.X, CurrentCell.Y), 1.0f);
                level.grid.SetCellCost(new Position(mx, my), 5.0f);

                Vector2 pos= new Vector2(mx * level.cellSize + level.cellSize / 2 - getWidth() / 2, my * level.cellSize + level.cellSize / 2 - getHeight() / 2);

                Position = pos;

                //change facing when ,,leaving tp"
                if (mx < level.player.CellX)
                {
                    _animationManager.Play(_animations["WalkRight"]);
                    currentFaceDirection = FaceDirections.Right;
                }
                else if (mx > level.player.CellX)
                {
                    _animationManager.Play(_animations["WalkLeft"]);
                    currentFaceDirection = FaceDirections.Left;
                }
                else
                {
                    if (my < level.player.CellY)
                    {
                        _animationManager.Play(_animations["WalkDown"]);
                        currentFaceDirection = FaceDirections.Down;
                    }
                    else
                    {
                        _animationManager.Play(_animations["WalkUp"]);
                        currentFaceDirection = FaceDirections.Up;
                    }
                }
                //play red particle
                Global.CombatManager.SetAnimation("Teleportation2", "MagicAnim2", mx, my);
            }
        }

        private Vector2 getRandomEmptyCell(Level level)
        {
            return level.GetRandomEmptyCellWithoutSetingCost();
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

