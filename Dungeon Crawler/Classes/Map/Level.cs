using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using RogueSharp;
using Microsoft.Xna.Framework;
using System;
using RoyT.AStar;

namespace Dungeon_Crawler
{
    public abstract class Level
    {
        public Map map;
        public Grid grid;
        public int cellSize;
        public List<Item> items;
        public List<Character> enemies;
        public List<Rock> rocks;
        public List<Cell> occupiedCells;
        public List<Projectile> Projectiles;

        public List<AttackAnimation> attackAnimations;

        public Texture2D floor;
        public Texture2D wall;
        public Portal portal;
        public Player player;

        public bool Finished { get; set; }

        public void AddPlayer(Player player)
        {
            this.player = player;
        }

        public abstract void Update(GameTime gameTime, GraphicsDevice graphicsDevice);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public Vector2 GetRandomEmptyCell()
        {
            int x, y;
            Cell tempCell;
            do
            {
                x = Global.random.Next(map.Width);
                y = Global.random.Next(map.Height);
                tempCell = map.GetCell(x, y);
            } while (!tempCell.IsWalkable || occupiedCells.Contains(tempCell) || grid.GetCellCost(new Position(x, y)) > 1.0f);
            grid.SetCellCost(new Position(x, y), 5.0f);
            return new Vector2(tempCell.X*floor.Width+ floor.Width/3, tempCell.Y * floor.Width +floor.Width / 3);
        }

        public Vector2 GetRandomEmptyCellWithoutSetingCost()
        {
            int x, y;
            Cell tempCell;
            do
            {
                x = Global.random.Next(map.Width);
                y = Global.random.Next(map.Height);
                tempCell = map.GetCell(x, y);
            } while (!tempCell.IsWalkable || occupiedCells.Contains(tempCell) || grid.GetCellCost(new Position(x, y)) > 1.0f);
            return new Vector2(tempCell.X, tempCell.Y);
        }

        public RogueSharp.Cell GetRandomEmptyCell(int dummy)
        {
            int x, y;
            Cell tempCell;
            do
            {
                x = Global.random.Next(map.Width);
                y = Global.random.Next(map.Height);
                tempCell = map.GetCell(x, y);
            } while (!tempCell.IsWalkable || occupiedCells.Contains(tempCell) || grid.GetCellCost(new Position(x, y)) > 1.0f);
            grid.SetCellCost(new Position(x, y), 5.0f);
            return tempCell;
        }

        public Cell GetRandomEmptyCellNearCoord(Vector2 origin, int distance)
        {
            Cell tempCell;
            for (int x = (int)origin.X - distance; x < (int)origin.X + distance; x++)
            {
                for (int y = (int)origin.Y - distance; y < (int)origin.Y + distance; y++)
                {
                    if (x >= 0 && y >= 0 && x < map.Width && y < map.Height)
                    {
                        tempCell = map.GetCell(x, y);
                        if (tempCell.IsWalkable && !occupiedCells.Contains(tempCell) && grid.GetCellCost(new Position(x, y)) == 1.0f)
                        {
                            return tempCell;
                        }
                    }
                }
            }
            throw new Exception("No empty cell was found in given area!");
        }

    }
}