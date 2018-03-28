﻿using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using RogueSharp;
using Microsoft.Xna.Framework;

namespace Dungeon_Crawler
{
    public class Level
    {
        public Map map;
        public List<Item> items;
        public List<Enemy> enemies;
        public List<Obstacle> obstacles;
        public List<Cell> occupiedCells;

        public Texture2D floor;
        public Texture2D wall;
        public Player player;
        public Level(Map map, List<Enemy> enemies, List<Item> items, List<Obstacle> obstacles, Texture2D floor, Texture2D wall,Player player, List<Cell> occupiedCells)
        {
            this.map = map;
            this.enemies = enemies;
            this.items = items;
            this.obstacles = obstacles;
            this.floor = floor;
            this.wall = wall;
            this.player = player;
            this.occupiedCells = occupiedCells;
        }
        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(gameTime, this, graphicsDevice);
            }
            player.Update(gameTime, this, graphicsDevice);
        

            Item[] itemArray = items.ToArray();
            for(int i = 0; i<items.Count;i++)
            {
                if (Collision.checkCollision(player, itemArray[i], graphicsDevice))
                {
                    player.inventory.Add(itemArray[i]);
                    items.Remove(itemArray[i]);
                }
            }
        }
        public void Draw(GameTime gameTime,SpriteBatch spriteBatch)
        {
            foreach (Cell cell in map.GetAllCells())
            {
                var position = new Vector2(cell.X * floor.Width, cell.Y * floor.Width);
                if (cell.IsWalkable)
                {
                    spriteBatch.Draw(floor, position, null, Color.White, 0.0f, Vector2.One, 1.0f, SpriteEffects.None, Layers.Cells);
                }
                else
                {
                    spriteBatch.Draw(wall, position, null, Color.White, 0.0f, Vector2.One, 1.0f, SpriteEffects.None, Layers.Cells);
                }
            }
            foreach (var item in items)
            {
                item.Draw(spriteBatch);
            }

            foreach (var obstacle in obstacles)
            {
                obstacle.Draw(spriteBatch);
            }

            foreach (var enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }

            player.Draw(spriteBatch);
        }

    }
}
