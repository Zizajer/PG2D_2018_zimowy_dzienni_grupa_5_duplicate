﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;


namespace Dungeon_Crawler
{
    public abstract class Character
    {
        public enum ActionState { Moving, Standing };
        public enum HealthState { Normal, Freeze, Burn };
        public enum Directions { None, Top, Bottom, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight};
        public Directions currentDirection;
        public ActionState currentActionState;
        public HealthState currentHealthState;

        public string Name { get; set; }

        public int Level { get; set; }

        public int Health { get; set; }
        public int CurrentHealth { get; set; }
        public int CurrentHealthPercent { get { return ((int)(CurrentHealth / (double)Health * 100)); } }

        public int Defense { get; set; }
        public int SpDefense { get; set; }
        public int Attack { get; set; }
        public int SpAttack { get; set; }
        public float Speed { get; set; }

        public bool isHitShaderOn = false;
        public float hitTimer=0;
        public float howLongShouldHitShaderApply = 0.25f;

        public float healthStateTimer = 0;
        public float howLongShouldHealthStateLast = Global.random.Next(5, 15);

        public int CellX { get; set; }
        public int CellY { get; set; }
        public AnimationManager _animationManager;
        protected Dictionary<String, Animation> _animations;
        public Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                if (_animationManager != null)
                    _animationManager.Position = _position;
            }
        }
        public Vector2 Center
        {
            get { return new Vector2(Position.X + getWidth() / 2, Position.Y + getHeight() / 2); }
        }

        public RogueSharp.Cell CurrentCell;
        public RogueSharp.Cell NextCell;
        public Vector2 Velocity;

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);
            if (isHitShaderOn) {
                if(this is Player)
                {
                    Global.Effects.hitPlayerEffect.CurrentTechnique.Passes[0].Apply();
                }
                if(this is Enemy || this is Boss)
                {
                    Global.Effects.hitEnemyEffect.CurrentTechnique.Passes[0].Apply();
                }
                
            }
            _animationManager.Draw(spriteBatch);
            spriteBatch.End();
        }

        protected virtual void SetAnimations()
        {
            if (Velocity.X > 0)
                _animationManager.Play(_animations["WalkRight"]);
            else if (Velocity.X < 0)
                _animationManager.Play(_animations["WalkLeft"]);
            else if (Velocity.Y < 0)
                _animationManager.Play(_animations["WalkUp"]);
            else if (Velocity.Y > 0)
                _animationManager.Play(_animations["WalkDown"]);
            else _animationManager.Stop();
        }
        public int getWidth()
        {
            return _animationManager._animation.FrameWidth-1;
        }
        public int getHeight()
        {
            return _animationManager._animation.FrameHeight-1;
        }

        public Rectangle getRectangle()
        {
            return new Rectangle((int)Math.Floor(Position.X), (int)Math.Floor(Position.Y),
               getWidth(), getHeight());
        }
        public Color[] getCurrentTextureData(GraphicsDevice graphicsDevice)
        {
            Texture2D multipleSpriteTexture = _animationManager._animation.Texture;

            Rectangle toCropRectangle = _animationManager.getCurrentFrameRectangle();

            Color[] singleTextureData = new Color[toCropRectangle.Width * toCropRectangle.Height];
            multipleSpriteTexture.GetData(0, toCropRectangle, singleTextureData, 0, singleTextureData.Length);

            return singleTextureData;
        }
        public abstract void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice);

        public bool isCenterOfGivenCell(RogueSharp.Cell NextCell, Level level, GraphicsDevice graphicsDevice)
        {
            int PosX = NextCell.X * level.cellSize + level.cellSize / 2;
            int PosY = NextCell.Y * level.cellSize + level.cellSize / 2;

            if (Math.Abs(Center.Y - PosY) <= Speed && Math.Abs(Center.X - PosX) <= Speed)
                return true;
            else
                return false;
        }

        public void MoveToCenterOfGivenCell(RogueSharp.Cell NextCell, Level level, GraphicsDevice graphicsDevice)
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

        public abstract void calculateStatistics();

        public void Move(Directions currentDirection, Level level, GraphicsDevice graphicsDevice)
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
