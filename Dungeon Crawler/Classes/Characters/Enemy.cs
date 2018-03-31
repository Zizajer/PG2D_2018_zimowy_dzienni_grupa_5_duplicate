﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon_Crawler
{
    public class Enemy:Character
    {
        float actionTimer;
        float timeBetweenActions;
        int lastDirection;
        public int x { get; set; }
        public int y { get; set; }

        public Enemy(Dictionary<string, Animation> _animations, int cellSize, float speed, float timeBetweenActions)
        {
            this._animations = _animations;
            this.timeBetweenActions = timeBetweenActions;
            this.Speed = speed;
            this.cellSize = cellSize;
            _animationManager = new AnimationManager(_animations.First().Value);

            actionTimer=Global.random.Next(3);

            lastDirection = Global.random.Next(4);
        }


        public virtual void Move(Level level,bool change,int direction,GraphicsDevice graphicsDevice)
        {
            x = (int)Math.Floor(fixedPosition.X / cellSize);
            y = (int)Math.Floor(fixedPosition.Y / cellSize);
            Map map = level.map;
            Cell enemyCell = map.GetCell(x, y);
            int pixelPerfectTolerance = 8;

            if (change)
            {
                lastDirection = direction;
            }
            else
            {
                if (lastDirection == 0)
                {
                    _position.Y = _position.Y - pixelPerfectTolerance;
                    if (isColliding(this, level, graphicsDevice))
                        moveUp(map, x, y, direction);
                    else
                        lastDirection = direction;
                    _position.Y = _position.Y + pixelPerfectTolerance;
                }

                if (lastDirection == 1)
                {
                    _position.Y = _position.Y + pixelPerfectTolerance;
                    if (isColliding(this, level, graphicsDevice))
                        moveDown(map, x, y, direction);
                    else
                        lastDirection = direction;
                    _position.Y = _position.Y - pixelPerfectTolerance;
                }

                if (lastDirection == 2)
                {
                    _position.X = _position.X - pixelPerfectTolerance;
                    if (isColliding(this, level, graphicsDevice))
                        moveLeft(map, x, y, direction);
                    else
                        lastDirection = direction;
                    _position.X = _position.X + pixelPerfectTolerance;
                }

                if (lastDirection == 3)
                {
                    _position.X = _position.X + pixelPerfectTolerance;

                    if (isColliding(this, level, graphicsDevice))
                        moveRight(map, x, y, direction);
                   else
                        lastDirection = direction;

                    _position.X = _position.X - pixelPerfectTolerance;
                }
            }
        }
        public bool isColliding(Character character, Level level, GraphicsDevice graphicsDevice)
        {
            if (!Collision.checkCollision(this, level.player, graphicsDevice))
            {
                bool collision = false;
                foreach (Enemy enemy in level.enemies)
                {
                    if (enemy.Equals(this)) continue;
                    if ((Math.Abs(this.fixedPosition.X - enemy.fixedPosition.X) < this.getWidth() * 2) && (Math.Abs(this.fixedPosition.Y - enemy.fixedPosition.Y) < this.getWidth()))
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
            else
                return false;
        }

        public void moveUp(Map map, int x, int y, int direction)
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

        public void moveDown(Map map, int x, int y, int direction)
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

        public void moveLeft(Map map, int x, int y, int direction)
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

        public void moveRight(Map map, int x, int y, int direction)
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
        public virtual void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {

            actionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Math.Abs(this.Position.X - level.player.Position.X) < Global.Camera.ViewportWidth && Math.Abs(this.Position.Y - level.player.Position.Y) < Global.Camera.ViewportHeight)
            {
                if (actionTimer > timeBetweenActions)
                {
                    actionTimer = 0;
                    Move(level, true, Global.random.Next(4), graphicsDevice);
                }
                else
                {
                    Move(level, false, Global.random.Next(4), graphicsDevice);
                }

                SetAnimations();
                _animationManager.Update(gameTime);
                Position += Velocity;
                Velocity = Vector2.Zero;
            }
        }
    }
}
