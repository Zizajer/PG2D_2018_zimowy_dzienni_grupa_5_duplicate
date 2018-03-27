using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon_Crawler
{
    public class Player
    {
        public List<Item> inventory { get; set; }
        public CameraManager camera;
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
        public float Speed = 1.0f;
        public Vector2 Velocity;

        public Player(ContentManager content, CameraManager camera, int cellSize)
        {
            _animations = new Dictionary<string, Animation>()
            {
                {"WalkUp",new Animation(content.Load<Texture2D>("player/Walkingup"),3 )},
                {"WalkDown",new Animation(content.Load<Texture2D>("player/WalkingDown"),3 )},
                {"WalkLeft",new Animation(content.Load<Texture2D>("player/WalkingLeft"),3 )},
                {"WalkRight",new Animation(content.Load<Texture2D>("player/WalkingRight"),3 )}
            };
            this.cellSize = cellSize;
            this.camera = camera;
            inventory = new List<Item>();
            _animationManager = new AnimationManager(_animations.First().Value);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            _animationManager.Draw(spriteBatch);
        }
        public virtual void Move(Map map)
        {
            int x = (int)Math.Floor(fixedPosition.X / cellSize);
            int y = (int)Math.Floor(fixedPosition.Y / cellSize);
            Cell playerCell = map.GetCell(x, y);
            if (Keyboard.GetState().IsKeyDown(Keys.W))
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
                }
                camera.CenterOn(fixedPosition);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Cell cellBelow = map.GetCell(x, y + 1);
                if (cellBelow.IsWalkable)
                {
                    Velocity.Y = +Speed;
                }
                else
                {
                    if (fixedPosition.Y + 1 < cellBelow.Y * cellSize)
                        Velocity.Y = +Speed;
                }
                camera.CenterOn(fixedPosition);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
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
                }
                camera.CenterOn(fixedPosition);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
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
                }
                camera.CenterOn(fixedPosition);
            }
        }
        public virtual void Update(GameTime gameTime, Map map)
        {
            Move(map);

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

        public string getItems()
        {
            string temp = "items= ";
            Item[] itemArray = inventory.ToArray();
            for (int i = 0; i < inventory.Count; i++)
            {
                temp += itemArray[i].name + " ";
            }

            return temp;
        }
    }
}
