using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using RoyT.AStar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon_Crawler
{
    public class Boss : Character
    {
        public new enum ActionState { Standby, Attacking};
        public new ActionState currentActionState;
        float actionTimer;
        float timeBetweenActions;

        Map map;

        //Attacks
        IPositionTargetedAttack ProjectileAttack;

        public Boss(Dictionary<string, Animation> _animations, int cellSize, int level, float timeBetweenActions, Map map)
        {
            Level = level;
            calculateStatistics();

            currentHealthState = HealthState.Normal;
            this._animations = _animations;
            this.timeBetweenActions = timeBetweenActions;
            _animationManager = new AnimationManager(_animations.First().Value);
            actionTimer = 0;

            this.map = map;
            CellX = (int)Math.Floor(Center.X / cellSize);
            CellY = (int)Math.Floor(Center.Y / cellSize);
            Name = "Demon Oak";

            //Set attacks
            ProjectileAttack = new BigFireballCanonade();
        }

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

            HandleHitState(gameTime);
            HandleHealthState(gameTime);

            actionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            CellX = (int)Math.Floor(Center.X / level.cellSize);
            CellY = (int)Math.Floor(Center.Y / level.cellSize);

            if (CurrentHealth <= 0)
            {
                level.grid.UnblockCell(new Position(5, 5));
                level.grid.UnblockCell(new Position(6, 5));
                level.grid.UnblockCell(new Position(7, 5));

                level.grid.UnblockCell(new Position(5, 6));
                level.grid.UnblockCell(new Position(6, 6));
                level.grid.UnblockCell(new Position(7, 6));

                level.grid.UnblockCell(new Position(5, 7));
                level.grid.UnblockCell(new Position(6, 7));
                level.grid.UnblockCell(new Position(7, 7));
            }

            if (currentHealthState != HealthState.Freeze)
            {
                if (currentActionState == ActionState.Standby)
                {
                    if (map.IsInFov(CellX, CellY))
                    {
                        currentActionState = ActionState.Attacking;
                    }

                }
                else if (currentActionState == ActionState.Attacking)
                {
                    if (!map.IsInFov(CellX, CellY))
                    {
                        currentActionState = ActionState.Standby;
                    }
                    UseProjectileAttack(level);
                }
                else
                {
                    Console.WriteLine("Error");
                }
                SetAnimations();
                _animationManager.Update(gameTime);
            }
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
