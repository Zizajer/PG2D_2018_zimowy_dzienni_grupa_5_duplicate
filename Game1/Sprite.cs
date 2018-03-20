using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game1
{
    class Sprite
    {
        protected AnimationManager _animationManager;
        protected Dictionary<String, Animation> _animations;
        protected Vector2 _position;
        protected Texture2D _texture;
        public Input input;
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
        public float Speed = 1.0f;
        public Vector2 Velocity;
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (_texture != null)
                spriteBatch.Draw(_texture, Position, Color.White);
            else if (_animationManager != null)
                _animationManager.Draw(spriteBatch);
            else throw new Exception("This aint right");
        }
        public virtual void Move()
        {
            if (Keyboard.GetState().IsKeyDown(input.Up))
                Velocity.Y = -Speed;
            if (Keyboard.GetState().IsKeyDown(input.Down))
                Velocity.Y = +Speed;
            if (Keyboard.GetState().IsKeyDown(input.Left))
                Velocity.X = -Speed;
            if (Keyboard.GetState().IsKeyDown(input.Right))
                Velocity.X = +Speed;
        }
        public Sprite(Texture2D texture)
        {
            _texture = texture;
        }
        public Sprite(Dictionary<string,Animation> animations)
        {
            _animations = animations;
            _animationManager = new AnimationManager(_animations.First().Value);
        }
        public virtual void Update(GameTime gameTime,List<Sprite>sprites)
        {
            Move();

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
    }
}
