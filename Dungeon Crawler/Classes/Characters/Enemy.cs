using Microsoft.Xna.Framework;
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

        Map map;
        PathFinder PathFinder;
        //Next cell to move in by enemy (returned by pathfinder)
        Cell CellToReach;
        bool BeginningOfPathFinding = true;

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


        public virtual void Move(Level level,int direction,GraphicsDevice graphicsDevice)
        {
            Cell enemyCell = map.GetCell(x, y);
            int pixelPerfectTolerance = 8;

           
            if (direction == 0)
            {
                _position.Y = _position.Y - pixelPerfectTolerance;
                if (isColliding(this, level, graphicsDevice))
                    moveUp(map, x, y, direction);
                _position.Y = _position.Y + pixelPerfectTolerance;
            }

            if (direction == 1)
            {
                _position.Y = _position.Y + pixelPerfectTolerance;
                if (isColliding(this, level, graphicsDevice))
                    moveDown(map, x, y, direction);
                _position.Y = _position.Y - pixelPerfectTolerance;
            }

            if (direction == 2)
            {
                _position.X = _position.X - pixelPerfectTolerance;
                if (isColliding(this, level, graphicsDevice))
                    moveLeft(map, x, y, direction);
                _position.X = _position.X + pixelPerfectTolerance;
            }

            if (direction == 3)
            {
                _position.X = _position.X + pixelPerfectTolerance;

                if (isColliding(this, level, graphicsDevice))
                    moveRight(map, x, y, direction);

                _position.X = _position.X - pixelPerfectTolerance;
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
                Velocity.Y = -1;
            }
            else
            {
                if (fixedPosition.Y > cellAbove.Y * cellSize + cellSize + getHeight() / 2)
                    Velocity.Y = -1;
            }
        }

        public void moveDown(Map map, int x, int y, int direction)
        {
            Cell cellBelow = map.GetCell(x, y + 1);
            if (cellBelow.IsWalkable)
            {
                Velocity.Y = +1;
            }
            else
            {
                if (fixedPosition.Y + 4 < cellBelow.Y * cellSize)
                    Velocity.Y = +1;
            }
        }

        public void moveLeft(Map map, int x, int y, int direction)
        {
            Cell cellOnLeft = map.GetCell(x - 1, y);
            if (cellOnLeft.IsWalkable)
            {
                Velocity.X = -1;
            }
            else
            {
                if (fixedPosition.X > cellOnLeft.X * cellSize + cellSize + getWidth() / 2)
                    Velocity.X = -1;
            }
        }

        public void moveRight(Map map, int x, int y, int direction)
        {
            Cell cellOnRight = map.GetCell(x + 1, y);
            if (cellOnRight.IsWalkable)
            {
                Velocity.X = +1;
            }
            else
            {
                if (fixedPosition.X + getWidth() / 2 < cellOnRight.X * cellSize)
                    Velocity.X = +1;
            }
        }
        public virtual void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {

            actionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Math.Abs(this.Position.X - level.player.Position.X) < Global.Camera.ViewportWidth && Math.Abs(this.Position.Y - level.player.Position.Y) < Global.Camera.ViewportHeight)
            {

                x = (int)Math.Floor(fixedPosition.X / cellSize);
                y = (int)Math.Floor(fixedPosition.Y / cellSize);

                //Pathfinder is reinitialized every time an enemy can move. We could initialize it once and keep it but in future development we could put new obstacles during gameplay
                //(thus changing property of some cells on the map) so we must take it into account.
                //Note: for some speedup, map references will be checked so if input map is the same as one in this class, we won't reinitialize pathfinder.
                if (this.map != level.map)
                {
                    PathFinder = new PathFinder(level.map);
                }
                this.map = level.map;

                
                // WYKOMENTOWANE RANDOMOWE PORUSZANIE SIE GDY BOT NIE WIDZI GRACZA

                //if (!map.IsInFov(this.x, this.y))
                //{
                    /*
                    if (actionTimer > timeBetweenActions)
                    {
                        actionTimer = 0;
                        Move(level, true, Global.random.Next(4), graphicsDevice);
                    }
                    else
                    {
                        Move(level, false, Global.random.Next(4), graphicsDevice);
                    }
                    */
                //}
                //else
                {

                    if (this.x != level.player.x || this.y != level.player.y) // Wchodzi w petle, gdy gracz nie stoi na tej samej celce co bot
                    {
                        if (BeginningOfPathFinding || (this.x == CellToReach.X && this.y == CellToReach.Y)) // Wchodzi w pętlę wtedy gdy bot doszedł do celki, wtedy pobiera kolejną. (Wchodzi w pętlę też za pierwszym razem, dlatego sprawdza boola)
                        {
                            BeginningOfPathFinding = false;
                            Path path = PathFinder.ShortestPath(level.map.GetCell(x, y), level.map.GetCell(level.player.x, level.player.y));
                            CellToReach = path.Start;

                        }

                        //W tych ifach kaze botowi isc w strone cellki

                        if (CellToReach.Y < y)
                        {
                            Move(level, 0, graphicsDevice);
                            Console.WriteLine("Up");
                        }
                        if (CellToReach.Y > y)
                        {
                            Move(level, 1, graphicsDevice);
                            Console.WriteLine("Down");
                        }
                        if (CellToReach.X < x)
                        {
                            Move(level, 2, graphicsDevice);
                            Console.WriteLine("Left");
                        }
                        if (CellToReach.X > x)
                        {
                            Move(level, 3, graphicsDevice);
                            Console.WriteLine("Right");
                        }
                        Console.WriteLine("CellToReach: {0}, {1} :: This.cell {2}, {3}", CellToReach.X, CellToReach.Y, this.x, this.y);
                    }


                }
                SetAnimations();
                _animationManager.Update(gameTime);
                Position += Velocity;
                Velocity = Vector2.Zero;
            }
        }
    }
}
