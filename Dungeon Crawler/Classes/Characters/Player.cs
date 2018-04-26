using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RoyT.AStar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon_Crawler
{
    public class Player : Character
    {
        public enum State { Moving, Standing };
        public float Mana;
        public int teleportCost = 10;
        public int fireballCost = 10;
        public int maxFireballsOnScreen = 20;
        public MouseState mouse;
        public float rotation;
        public List<Item> inventory { get; set; }
        public int CurrentLevel { get; set; }
        KeyboardState pastKey;
        MouseState pastButton;
        Directions currentDirection;
        public State currentState;
        public Player(ContentManager content, int cellSize, int playerCurrentLevel)
        {
            currentState = State.Standing;
            Health = 100;
            _animations = new Dictionary<string, Animation>()
            {
                {"WalkUp",new Animation(content.Load<Texture2D>("player/Walkingup"),3 )},
                {"WalkDown",new Animation(content.Load<Texture2D>("player/WalkingDown"),3 )},
                {"WalkLeft",new Animation(content.Load<Texture2D>("player/WalkingLeft"),3 )},
                {"WalkRight",new Animation(content.Load<Texture2D>("player/WalkingRight"),3 )}
            };
            Speed = 4f;
            Mana = 100;
            CurrentLevel = playerCurrentLevel;
            inventory = new List<Item>();
            _animationManager = new AnimationManager(_animations.First().Value);


        }
        public bool IsHitByProjectile(Level level, GraphicsDevice graphicsDevice)
        {
            EnemyProjectile projectile = null;
            for (int i = 0; i < level.enemyProjectiles.Count; i++)
            {
                projectile = level.enemyProjectiles[i];
                if (Collision.checkCollision(getRectangle(), this, projectile, graphicsDevice))
                {
                    projectile.isPlayerHit = true;

                    return true;
                }
            }
            return false;
        }
        public void Fireball(Level level)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && pastKey.IsKeyUp(Keys.Space))
            {
                if (level.playerProjectiles.Count() < maxFireballsOnScreen && Mana> fireballCost)
                {
                    MouseState mouse = Mouse.GetState();
                    Vector2 tempVector = new Vector2(mouse.X, mouse.Y);
                    Vector2 mousePos = Global.Camera.ScreenToWorld(tempVector);
                    float distanceX = mousePos.X - this.Position.X;
                    float distanceY = mousePos.Y - this.Position.Y;

                    rotation = (float)Math.Atan2(distanceY, distanceX);
                    Vector2 tempVelocity = new Vector2((float)Math.Cos(rotation) * 5f, ((float)Math.Sin(rotation)) * 5f) +Velocity/3;
                    Vector2 tempPosition = Center + tempVelocity * 3;

                    PlayerProjectile newProjectile = new PlayerProjectile(tempVelocity, tempPosition, level.fireball, rotation);

                    level.playerProjectiles.Add(newProjectile);
                    Mana = Mana - fireballCost;
                }
            }
            pastKey = Keyboard.GetState();
        }

        public void Teleport(Level level,GraphicsDevice graphicsDevice)
        {
            if (Mouse.GetState().RightButton == ButtonState.Pressed && pastButton.RightButton==ButtonState.Released)
            {
                if (Mana > teleportCost)
                {
                    MouseState mouse = Mouse.GetState();
                    Vector2 tempVector = new Vector2(mouse.X, mouse.Y);
                    Vector2 mousePos = Global.Camera.ScreenToWorld(tempVector);
                    x = (int)Math.Floor(mousePos.X / level.cellSize);
                    y = (int)Math.Floor(mousePos.Y / level.cellSize);

                    if (x < 0 || x >= level.map.Width || y < 0 || y >= level.map.Height)
                        return;
                    if (level.grid.GetCellCost(new Position(x,y))==1.0f)
                    {
                        level.grid.SetCellCost(new Position(CurrentCell.X, CurrentCell.Y), 1.0f);
                        level.grid.SetCellCost(new Position(x, y), 5.0f);
                        mousePos.X = x * level.cellSize + level.cellSize / 2 - getWidth() / 2;
                        mousePos.Y = y * level.cellSize + level.cellSize / 2 - getHeight() / 2;
                        Position = mousePos;
                        Mana = Mana - teleportCost;
                        level.map.ComputeFov(x, y, 15, true);
                        Global.Camera.CenterOn(Center);
                    }
                }
            }
            pastButton = Mouse.GetState();
        }

        public virtual Directions GetDirection()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
                return Directions.TopLeft;

            if (Keyboard.GetState().IsKeyDown(Keys.E))
                return Directions.TopRight;

            if (Keyboard.GetState().IsKeyDown(Keys.Z))
                return Directions.BottomLeft;

            if (Keyboard.GetState().IsKeyDown(Keys.C))
                return Directions.BottomRight;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                return Directions.Top;

            if (Keyboard.GetState().IsKeyDown(Keys.S))
                return Directions.Bottom;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
                return Directions.Left;

            if (Keyboard.GetState().IsKeyDown(Keys.D))
                return Directions.Right;

            return Directions.None;
        }

        public override void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {
            x = (int)Math.Floor(Center.X / level.cellSize);
            y = (int)Math.Floor(Center.Y / level.cellSize);
            if (x > 0 && x < level.map.Width && y > 0 && y < level.map.Height)
            {
                CurrentCell = level.map.GetCell(x, y);
            }

            if (IsHitByProjectile(level, graphicsDevice))
            {
                Health -= 20;
            }

            if (Mana < 100) Mana = Mana + 0.95f; //0.15

            if (currentState == State.Standing)
            {
                currentDirection = GetDirection();
                if (currentDirection != (int)Directions.None)
                {
                    RogueSharp.Cell futureNextCell= Collision.getCellFromDirection(CurrentCell, currentDirection, level);
                    if (x > 0 && x < level.map.Width && y > 0 && y < level.map.Height)
                    {
                        if (!Collision.checkCollisionInGivenCell(futureNextCell, level, graphicsDevice))
                        {
                            NextCell = futureNextCell;
                            currentState = State.Moving;
                            level.grid.SetCellCost(new Position(CurrentCell.X, CurrentCell.Y), 1.0f);
                            level.grid.SetCellCost(new Position(NextCell.X, NextCell.Y), 5.0f);
                        }
                    }
                    else
                    {
                        Global.Gui.WriteToConsole("Cant go there");
                    }
                }
                Teleport(level, graphicsDevice);
            }
            else //Moving
            {
                if (isCenterOfGivenCell(NextCell, level, graphicsDevice))
                {
                    currentState = State.Standing;
                }
                else
                {
                    MoveToCenterOfGivenCell(NextCell, level, graphicsDevice);
                    level.map.ComputeFov(x, y, 15, true);
                    Global.Camera.CenterOn(Center);
                }
                
            }
            
            Fireball(level);
            SetAnimations();
            _animationManager.Update(gameTime);
            Position += Velocity;
            Velocity = Vector2.Zero;
        }

        private void Move(Directions currentDirection, Level level, GraphicsDevice graphicsDevice)
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

        private bool isCenterOfGivenCell(RogueSharp.Cell NextCell, Level level, GraphicsDevice graphicsDevice)
        {
            int PosX = NextCell.X * level.cellSize + level.cellSize / 2;
            int PosY = NextCell.Y * level.cellSize + level.cellSize / 2;

            if (Math.Abs(Center.Y - PosY) <= Speed && Math.Abs(Center.X - PosX) <= Speed)
                return true;
            else
                return false;
        }

        private void MoveToCenterOfGivenCell(RogueSharp.Cell NextCell, Level level, GraphicsDevice graphicsDevice)
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
    }
}
