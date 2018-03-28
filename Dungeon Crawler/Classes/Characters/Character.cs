﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;


namespace Dungeon_Crawler
{
    public class Character
    {
        public int Damage { get; set; }
        public int Health { get; set; }
        public string Name { get; set; }
        public int cellSize;

        public AnimationManager _animationManager;
        protected Dictionary<String, Animation> _animations;
        protected Vector2 _position;
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
        public Vector2 fixedPosition
        {
            get { return new Vector2(Position.X + getWidth() / 2, Position.Y + getHeight()); }
        }

        public float Speed = 1.0f;
        public Vector2 Velocity;

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            _animationManager.Draw(spriteBatch);
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
            return _animationManager._animation.FrameWidth;
        }
        public int getHeight()
        {
            return _animationManager._animation.FrameHeight;
        }

        public Rectangle getRectangle()
        {
            return new Rectangle((int)Position.X, (int)Position.Y,
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
    }
}
