using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using RoyT.AStar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Dungeon_Crawler
{
    public abstract class Boss : Character
    {
        public List<Cell> occupyingCells;
        public new enum ActionState { Standby, Attacking};
        public new ActionState currentActionState;
        public float actionTimer;
        public float timeBetweenActions;

        Map map;

        //Attacks
        public IPositionTargetedAttack ProjectileAttack;
        public IUnTargetedAttack TargetedAttack;
        public IUnTargetedAttack TargetedAttack2;

        public Boss(Dictionary<string, Animation> _animations, int cellSize, int level, float timeBetweenActions, Map map, List<Cell> cells)
        {
            Level = level;
            calculateStatistics();
            currentActionState = ActionState.Attacking;
            currentHealthState = HealthState.Normal;
            this._animations = _animations;
            this.timeBetweenActions = timeBetweenActions;
            _animationManager = new AnimationManager(_animations.First().Value);
            actionTimer = 0;

            this.map = map;
            CellX = (int)Math.Floor(Center.X / cellSize);
            CellY = (int)Math.Floor(Center.Y / cellSize);
            occupyingCells = cells;
            Name = "Demon Oak";

            Inventory = new List<Item>();

            SetAttack();
        }

        public abstract void SetAttack();

        public override void calculateStatistics()
        {
            Health = CurrentHealth = 210 + Level * 10;
            Defense = 210 + Level * 3;
            SpDefense = 210 + Level * 5;
            Attack = (int)Math.Floor(210 + Level * 2.5);
            SpAttack = 210 + Level * 3;
            Speed = 0f;
        }

        public override void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {
            CellX = (int)Math.Floor(Center.X / level.cellSize);
            CellY = (int)Math.Floor(Center.Y / level.cellSize);
            HandleHitState(gameTime);
            HandleHealthState(gameTime);

            actionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (CurrentHealth <= 0)
            {
                foreach(Cell cell in occupyingCells)
                {
                    level.grid.SetCellCost(new Position(cell.X, cell.Y),1.0f);
                }
            }

            if (currentHealthState != HealthState.Freeze)
            {
                if (level.map.IsInFov(CellX, CellY))
                    UseProjectileAttack(level);
            }

            SetAnimations();
            _animationManager.Update(gameTime);
        }
        public void UseProjectileAttack(Level level)
        {
            if (actionTimer>timeBetweenActions)
            {
                actionTimer = 0;
                ProjectileAttack.Use(this, level.player.Center);
            }
        }
        protected override void SetAnimations()
        {
             _animationManager.Play(_animations["BossAlive"]);
        }
    }
}
