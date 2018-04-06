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
            this.Health = 100;
            _animations = new Dictionary<string, Animation>()
            {
                {"WalkUp",new Animation(content.Load<Texture2D>("player/Walkingup"),3 )},
                {"WalkDown",new Animation(content.Load<Texture2D>("player/WalkingDown"),3 )},
                {"WalkLeft",new Animation(content.Load<Texture2D>("player/WalkingLeft"),3 )},
                {"WalkRight",new Animation(content.Load<Texture2D>("player/WalkingRight"),3 )}
            };
            Speed = 2.5f;
            Mana = 100;
            this.CurrentLevel = playerCurrentLevel;
            this.cellSize = cellSize;
            inventory = new List<Item>();
            _animationManager = new AnimationManager(_animations.First().Value);
        }

        public void Shoot(Level level)
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
                if (Mana > 60)
                {
                    MouseState mouse = Mouse.GetState();
                    Vector2 mousePos = Global.Camera.ScreenToWorld(mouse.X, mouse.Y);
                    x = (int)Math.Floor(mousePos.X / cellSize);
                    y = (int)Math.Floor(mousePos.Y / cellSize);
                    if (level.map.GetCell(x, y).IsWalkable && !level.occupiedCells.Contains(level.map.GetCell(x, y)))
                    {
                        Vector2 tempVector = new Vector2(mousePos.X, mousePos.Y);
                        this.Position = tempVector;
                        Mana = Mana - 60;
                        Global.Camera.CenterOn(fixedPosition);
                    }
                }
            }
        }

        public virtual void Move(Level level, GraphicsDevice graphicsDevice)
        {
            x = (int)Math.Floor(fixedPosition.X / cellSize);
            y = (int)Math.Floor(fixedPosition.Y / cellSize);
            Map map = level.map;
            Cell playerCell = map.GetCell(x, y);
            int pixelPerfectTolerance = 4;

            if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.D))
            {
                return;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                _position.Y = _position.Y - pixelPerfectTolerance;
                if (isColliding(this, level, graphicsDevice))
                {
                    moveUp(map, x, y);
                }
              _position.Y = _position.Y + pixelPerfectTolerance;

                Global.Camera.CenterOn(fixedPosition);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {

                _position.Y = _position.Y + pixelPerfectTolerance;
                if (isColliding(this, level, graphicsDevice))
                {
                    moveDown(map, x, y);
                }
                _position.Y = _position.Y - pixelPerfectTolerance;

                Global.Camera.CenterOn(fixedPosition);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                _position.X = _position.X - pixelPerfectTolerance;
                if (isColliding(this, level, graphicsDevice))
                {
                    moveLeft(map, x, y);
                }
                _position.X = _position.X + pixelPerfectTolerance;

                Global.Camera.CenterOn(fixedPosition);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                _position.X = _position.X + pixelPerfectTolerance;
                if (isColliding(this, level, graphicsDevice))
                {
                    moveRight(map, x, y);
                }
                _position.X = _position.X - pixelPerfectTolerance;

                Global.Camera.CenterOn(fixedPosition);
            }
        }

        public bool isColliding(Character character, Level level, GraphicsDevice graphicsDevice)
        {
            bool collision = false;
            foreach (Enemy enemy in level.enemies)
            {
                if ((Math.Abs(this.fixedPosition.X - enemy.fixedPosition.X) < this.getWidth() * 2) && (Math.Abs(this.fixedPosition.Y - enemy.fixedPosition.Y) < this.getHeight()))
                {
                    if (Collision.checkCollision(this, enemy, graphicsDevice))
                        collision = true;
                }
            }
            foreach (Obstacle obstacle in level.obstacles)
            {
                if ((Math.Abs(this.Position.X - obstacle.Position.X) < this.getWidth() + obstacle.Texture.Width) && (Math.Abs(this.Position.Y - obstacle.Position.Y) < this.getHeight() + obstacle.Texture.Height))
                {
                    if (Collision.checkCollision(this, obstacle, graphicsDevice))
                        collision = true;
                }
            }
            if (!collision)
                return true;
            else
                return false;
        }
        public void moveUp(Map map, int x, int y)
        {
            Cell cellAbove = map.GetCell(x, y - 1);
            if (cellAbove.IsWalkable)
            {
                Velocity.Y = -Speed;
            }
            else
            {
                if (fixedPosition.Y> cellAbove.Y * cellSize + cellSize + getHeight() / 2)
                    Velocity.Y = -Speed;
            }
        }

        public void moveDown(Map map, int x, int y)
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
        }

        public void moveLeft(Map map, int x, int y)
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
        }

        public void moveRight(Map map, int x, int y)
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
        }

        public virtual void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {
            if(Mana<100)Mana=Mana+0.15f;
            Move(level,graphicsDevice);
            //Compute fov of the player within range of 15 cells. 
            //This fov calculation will be used by all enemies (it's faster to calcluate fov only for player rather then for all enemies).
            //So basically, for enemies, we will be checking if player can see enemy, not if enemy can see player (isn't it the same?)
            //
            //Fov will be computed within every update call. It could be done only after player move and it surely would improve performance.. but what if we put some new obstacles during a gameplay ?
            //They wouldn't be tracked until the player moved!
            level.map.ComputeFov(x, y, 15, true);
            Shoot(level);
            Teleport(level);
            SetAnimations();
            _animationManager.Update(gameTime);
            Position += Velocity;
            Velocity = Vector2.Zero;
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
