using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using RogueSharp;
using Microsoft.Xna.Framework;

namespace Dungeon_Crawler
{
    class Level
    {
        private Map map;
        List<Item> items;
        List<Enemy> enemies;
        List<Obstacle> obstacles;

        private Texture2D floor;
        private Texture2D wall;
        private int cellSize;
        private Player player;
        public Level(Map map, List<Enemy> enemies, List<Item> items, List<Obstacle> obstacles, Texture2D floor, Texture2D wall,int cellSize,Player player)
        {
            this.map = map;
            this.enemies = enemies;
            this.items = items;
            this.obstacles = obstacles;
            this.floor = floor;
            this.wall = wall;
            this.cellSize = cellSize;
            this.player = player;
        }
        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(gameTime, map);
            }
            player.Update(gameTime, map);
        

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
                var position = new Vector2(cell.X * cellSize, cell.Y * cellSize);
                if (cell.IsWalkable)
                {
                    spriteBatch.Draw(floor, position, null, Color.White, 0.0f, Vector2.One, 1.0f, SpriteEffects.None, 0.9f);
                }
                else
                {
                    spriteBatch.Draw(wall, position, null, Color.White, 0.0f, Vector2.One, 1.0f, SpriteEffects.None, 0.9f);
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

            player.Draw(spriteBatch);
            foreach (var enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }

    }
}
