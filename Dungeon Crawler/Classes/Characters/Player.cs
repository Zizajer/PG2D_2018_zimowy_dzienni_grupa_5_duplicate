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
    public class Player:Character
    {
        public List<Item> inventory { get; set; }

        public Player(ContentManager content, int cellSize)
        {
            this.Health = 100;
            _animations = new Dictionary<string, Animation>()
            {
                {"WalkUp",new Animation(content.Load<Texture2D>("player/Walkingup"),3 )},
                {"WalkDown",new Animation(content.Load<Texture2D>("player/WalkingDown"),3 )},
                {"WalkLeft",new Animation(content.Load<Texture2D>("player/WalkingLeft"),3 )},
                {"WalkRight",new Animation(content.Load<Texture2D>("player/WalkingRight"),3 )}
            };
            this.cellSize = cellSize;
            inventory = new List<Item>();
            _animationManager = new AnimationManager(_animations.First().Value);
        }

        public virtual void Move(Map map)
        {
            int x = (int)Math.Floor(fixedPosition.X / cellSize);
            int y = (int)Math.Floor(fixedPosition.Y / cellSize);
            Cell playerCell = map.GetCell(x, y);
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                moveUp(map, x, y);
                Global.Camera.CenterOn(fixedPosition);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                moveDown(map, x, y);
                Global.Camera.CenterOn(fixedPosition);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                moveLeft(map, x, y);
                Global.Camera.CenterOn(fixedPosition);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                moveRight(map, x, y);
                Global.Camera.CenterOn(fixedPosition);
            }
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
                if (fixedPosition.Y > cellAbove.Y * cellSize + cellSize + getHeight() / 2)
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

        public virtual void Update(GameTime gameTime, Map map)
        {
            Move(map);

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
