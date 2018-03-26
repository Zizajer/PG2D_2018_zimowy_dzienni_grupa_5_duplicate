using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon_Crawler
{
    public class Enemy
    {
        float actionTimer;
        float timeBetweenActions;
        int lastDirection;
        Random rand;
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
            get { return new Vector2(_position.X + getWidth() / 2, _position.Y + getHeight()); }
        }
        public float Speed;
        public Vector2 Velocity;

        public Enemy(Dictionary<string, Animation> animations, int cellSize, float speed,float timeBetweenActions)
        {
            this.timeBetweenActions = timeBetweenActions;
            this.Speed = speed;
            this.cellSize = cellSize;
            _animations = animations;
            _animationManager = new AnimationManager(_animations.First().Value);

            rand = new Random();
            actionTimer=rand.Next(3);

            lastDirection = rand.Next(4);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            _animationManager.Draw(spriteBatch);
        }
        public virtual void Move(Map map,bool change,int direction)
        {
            int x = (int)Math.Floor(fixedPosition.X / cellSize);
            int y = (int)Math.Floor(fixedPosition.Y / cellSize);
            Cell enemyCell = map.GetCell(x, y);

            if (change)
            {
                lastDirection = direction;
            }
            else
            {
                if (lastDirection == 0)
                {
                    Cell cellAbove = map.GetCell(x, y - 1);
                    if (cellAbove.IsWalkable)
                    {
                        Velocity.Y = -Speed;
                    }
                    else
                    {
                        if (fixedPosition.Y > cellAbove.Y * cellSize + cellSize + getHeight() / 2)
                            Velocity.Y = -Speed;
                        else
                            lastDirection = direction;
                    }
                }

                if (lastDirection == 1)
                {
                    Cell cellBelow = map.GetCell(x, y + 1);
                    if (cellBelow.IsWalkable)
                    {
                        Velocity.Y = +Speed;
                    }
                    else
                    {
                        if (fixedPosition.Y + 4 < cellBelow.Y * cellSize)
                            Velocity.Y = +Speed;
                        else
                            lastDirection = direction;
                    }
                }

                if (lastDirection == 2)
                {
                    Cell cellOnLeft = map.GetCell(x - 1, y);
                    if (cellOnLeft.IsWalkable)
                    {
                        Velocity.X = -Speed;
                    }
                    else
                    {
                        if (fixedPosition.X > cellOnLeft.X * cellSize + cellSize + getWidth() / 2)
                            Velocity.X = -Speed;
                        else
                            lastDirection = direction;
                    }
                }

                if (lastDirection == 3)
                {
                    Cell cellOnRight = map.GetCell(x + 1, y);
                    if (cellOnRight.IsWalkable)
                    {
                        Velocity.X = +Speed;
                    }
                    else
                    {
                        if (fixedPosition.X + getWidth() / 2 < cellOnRight.X * cellSize)
                            Velocity.X = +Speed;
                        else
                            lastDirection = direction;
                    }
                }
            }
        }
        public virtual void Update(GameTime gameTime, Map map)
        {

            actionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            Random superRand = new Random(rand.Next(1000));
            if (actionTimer > timeBetweenActions)
            {
                actionTimer = 0;
                Move(map, true, superRand.Next(4));
            }
            else
            {
                Move(map, false, superRand.Next(4));
            }
            
            SetAnimations();
            _animationManager.Update(gameTime);
            Position += Velocity;
            Velocity = Vector2.Zero;
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

        public Microsoft.Xna.Framework.Rectangle getRectangle()
        {
            return new Microsoft.Xna.Framework.Rectangle((int)Position.X, (int)Position.Y,
                _animationManager._animation.FrameWidth, _animationManager._animation.FrameHeight);
        }
        public Color[] getCurrentTextureData(GraphicsDevice graphicsDevice)
        {
            Texture2D multipleSpriteTexture = _animationManager._animation.Texture;

            Microsoft.Xna.Framework.Rectangle toCropRectangle = _animationManager.getCurrentFrameRectangle();

            Color[] singleTextureData = new Color[toCropRectangle.Width * toCropRectangle.Height];
            multipleSpriteTexture.GetData(0, toCropRectangle, singleTextureData, 0, singleTextureData.Length);

            return singleTextureData;
        }
    }
}
