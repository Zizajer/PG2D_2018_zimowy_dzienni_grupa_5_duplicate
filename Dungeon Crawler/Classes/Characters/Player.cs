using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon_Crawler
{
    public class Player : Character
    {
        public float Mana;
        public MouseState mouse;
        public float rotation;
        public List<Item> inventory { get; set; }
        public int CurrentLevel { get; set; }
        KeyboardState pastKey;
        public int x;
        public int y;
        public Player(ContentManager content, int cellSize, int playerCurrentLevel)
        {
            Health = 100;
            _animations = new Dictionary<string, Animation>()
            {
                {"WalkUp",new Animation(content.Load<Texture2D>("player/Walkingup"),3 )},
                {"WalkDown",new Animation(content.Load<Texture2D>("player/WalkingDown"),3 )},
                {"WalkLeft",new Animation(content.Load<Texture2D>("player/WalkingLeft"),3 )},
                {"WalkRight",new Animation(content.Load<Texture2D>("player/WalkingRight"),3 )}
            };
            Speed = 5.5f;
            Mana = 100;
            CurrentLevel = playerCurrentLevel;
            inventory = new List<Item>();
            _animationManager = new AnimationManager(_animations.First().Value);
        }

        public void Fireball(Level level)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && pastKey.IsKeyUp(Keys.Space))
            {
                if (level.projectiles.Count() < 20 && Mana>30)
                {
                    MouseState mouse = Mouse.GetState();
                    Vector2 mousePos = Global.Camera.ScreenToWorld(mouse.X, mouse.Y);
                    float distanceX = mousePos.X - this.Position.X;
                    float distanceY = mousePos.Y - this.Position.Y;

                    rotation = (float)Math.Atan2(distanceY, distanceX);
                    Vector2 tempVelocity = new Vector2((float)Math.Cos(rotation) * 3f, ((float)Math.Sin(rotation)) * 5f);
                    Vector2 tempPosition = this.Origin + tempVelocity * 3;

                    Projectile newProjectile = new Projectile(tempVelocity, tempPosition, level.fireball, rotation);

                    level.projectiles.Add(newProjectile);
                    Mana = Mana - 30;
                }
            }
            pastKey = Keyboard.GetState();
        }

        public void Teleport(Level level)
        {
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                if (Mana > 10)
                {
                    MouseState mouse = Mouse.GetState();
                    Vector2 mousePos = Global.Camera.ScreenToWorld(mouse.X, mouse.Y);
                    x = (int)Math.Floor(mousePos.X / level.cellSize);
                    y = (int)Math.Floor(mousePos.Y / level.cellSize);
                    if (level.map.GetCell(x, y).IsWalkable && !level.occupiedCells.Contains(level.map.GetCell(x, y)))
                    {
                        Vector2 tempVector = new Vector2(mousePos.X, mousePos.Y);
                        Position = tempVector;
                        Mana = Mana - 10;
                        Global.Camera.CenterOn(fixedPosition);
                    }
                }
            }
        }

        public virtual int GetDirection()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.D))
                return (int)Directions.None;

            if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.A))
                return (int)Directions.TopLeft;

            if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.D))
                return (int)Directions.TopRight;

            if (Keyboard.GetState().IsKeyDown(Keys.S) && Keyboard.GetState().IsKeyDown(Keys.A))
                return (int)Directions.BottomLeft;

            if (Keyboard.GetState().IsKeyDown(Keys.S) && Keyboard.GetState().IsKeyDown(Keys.D))
                return (int)Directions.BottomRight;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                return (int)Directions.Up;

            if (Keyboard.GetState().IsKeyDown(Keys.S))
                return (int)Directions.Down;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
                return (int)Directions.Left;

            if (Keyboard.GetState().IsKeyDown(Keys.D))
                return (int)Directions.Right;

            return (int)Directions.None;
        }

        public virtual void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {
            if (Mana < 100) Mana = Mana + 0.15f;
            if (!Collision.isCharacterInBounds(this,level))
            {
                Console.WriteLine("player not in bounds");
                Collision.getCharacterInBounds(this,level);
            }
            
            int currentDirection = GetDirection();
            if (currentDirection != (int)Directions.None)
            {
                if (!Collision.checkCollisionInGivenDirection(currentDirection, this, level))
                {
                    Move(currentDirection, Speed, level, graphicsDevice);
                    level.map.ComputeFov(x, y, 15, true);
                    Global.Camera.CenterOn(fixedPosition);
                }
            }

            Fireball(level);
            Teleport(level);
            SetAnimations();
            _animationManager.Update(gameTime);
            Position += Velocity;
            Velocity = Vector2.Zero;
        }

        private void Move(int currentDirection, float speed, Level level, GraphicsDevice graphicsDevice)
        {
            if (currentDirection == (int)Directions.Up)
                Velocity.Y = -Speed;

            if (currentDirection == (int)Directions.Down)
                Velocity.Y = +Speed;

            if (currentDirection == (int)Directions.Left)
                Velocity.X = -Speed;

            if (currentDirection == (int)Directions.Right)
                Velocity.X = +Speed;

            if (currentDirection == (int)Directions.TopLeft)
            {
                Velocity.X = -Speed;
                Velocity.Y = -Speed;
            }

            if (currentDirection == (int)Directions.TopRight)
            {
                Velocity.X = +Speed;
                Velocity.Y = -Speed;
            }

            if (currentDirection == (int)Directions.BottomLeft)
            {
                Velocity.X = -Speed;
                Velocity.Y = +Speed;
            }
            if (currentDirection == (int)Directions.BottomRight)
            {
                Velocity.X = +Speed;
                Velocity.Y = +Speed;
            }
        }


        public string getItems()
        {
            if (inventory.Count == 0) return "Inventory is empty";
            string temp = "Inventory: ";
            Item[] itemArray = inventory.ToArray();
            for (int i = 0; i < inventory.Count; i++)
            {
                temp += itemArray[i].name + ", ";
            }

            return temp;
        }
    }
}
