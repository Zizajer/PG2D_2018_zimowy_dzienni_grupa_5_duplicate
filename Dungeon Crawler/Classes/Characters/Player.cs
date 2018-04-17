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
                    Rectangle tempRec = new Rectangle((int)mousePos.X, (int)mousePos.Y, getWidth(), getHeight());
                    if (x < 0 || x >= level.map.Width || y < 0 || y >= level.map.Height || Collision.isCollidingWithEnemies(tempRec,this,level,graphicsDevice) || Collision.isCollidingWithRocks(tempRec, this, level, graphicsDevice))
                        return;
                    if (level.map.GetCell(x, y).IsWalkable && !level.occupiedCells.Contains(level.map.GetCell(x, y)) && Collision.isCharacterInBounds(tempRec, level) && level.grid.GetCellCost(new Position(x,y))!= float.PositiveInfinity)
                    {
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
            if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.D))
                return Directions.None;

            if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.A))
                return Directions.TopLeft;

            if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.D))
                return Directions.TopRight;

            if (Keyboard.GetState().IsKeyDown(Keys.S) && Keyboard.GetState().IsKeyDown(Keys.A))
                return Directions.BottomLeft;

            if (Keyboard.GetState().IsKeyDown(Keys.S) && Keyboard.GetState().IsKeyDown(Keys.D))
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
            if (IsHitByProjectile(level, graphicsDevice))
            {
                Health -= 20;
            }
            x = (int)Math.Floor(Center.X / level.cellSize);
            y = (int)Math.Floor(Center.Y / level.cellSize);
            CurrentCell = level.map.GetCell(x, y);

            if (Mana < 100) Mana = Mana + 0.95f; //0.15
            if (!Collision.isCharacterInBounds(this, level))
            {
                Collision.getPlayerInBounds(this, level, graphicsDevice);
            }
            
            currentDirection = GetDirection();
            if (currentDirection != (int)Directions.None)
            {
                if (!Collision.checkCollisionInGivenDirection(currentDirection, this, level, graphicsDevice))
                {
                    Move(currentDirection, Speed, level, graphicsDevice);
                    level.map.ComputeFov(x, y, 15, true);
                    Global.Camera.CenterOn(Center);
                }
                else
                {
                    //this allows sliding when one of diagonal directions is blocked eg. cant go topleft but can go left
                    if (currentDirection == Directions.TopLeft || currentDirection == Directions.TopRight || currentDirection == Directions.BottomLeft || currentDirection == Directions.BottomRight)
                    {
                        Directions fixedDirection = Collision.checkIfOneOfDoubleDirectionsIsOk(currentDirection, this, level, graphicsDevice);
                        if (!Collision.checkCollisionInGivenDirection(fixedDirection, this, level, graphicsDevice))
                        {
                            Move(fixedDirection, Speed, level, graphicsDevice);
                            level.map.ComputeFov(x, y, 15, true);
                            Global.Camera.CenterOn(Center);
                        }
                    }
                }
            }

            Fireball(level);
            Teleport(level,graphicsDevice);
            SetAnimations();
            _animationManager.Update(gameTime);
            Position += Velocity;
            Velocity = Vector2.Zero;
        }

        private void Move(Directions currentDirection, float speed, Level level, GraphicsDevice graphicsDevice)
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
    }
}
