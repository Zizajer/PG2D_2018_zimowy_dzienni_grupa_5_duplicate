using Microsoft.Xna.Framework.Graphics;
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
        public Portal portal;
        public Player player;

        public bool finished { get; set; }
        public Level(Map map, List<Enemy> enemies, List<Item> items, List<Obstacle> obstacles, Texture2D floor, Texture2D wall, Portal portal, Player player, List<Cell> occupiedCells)
        {
            this.map = map;
            this.enemies = enemies;
            this.items = items;
            this.obstacles = obstacles;
            this.floor = floor;
            this.wall = wall;
            this.portal = portal;
            this.player = player;
            this.occupiedCells = occupiedCells;
            finished = false;
        }
        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            if (Collision.checkCollision(player, portal, graphicsDevice))
            {
                finished = true;
                return;
            }
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

            portal.Draw(spriteBatch);
            player.Draw(spriteBatch);
        }
        public Vector2 GetRandomEmptyCell()
        {
            int x, y;
            Cell tempCell;
            do
            {
                x = Global.random.Next(map.Width);
                y = Global.random.Next(map.Height);
                tempCell = map.GetCell(x, y);
            } while (!tempCell.IsWalkable || occupiedCells.Contains(tempCell));
            return new Vector2(tempCell.X*floor.Width+ floor.Width/3, tempCell.Y * floor.Width +floor.Width / 3);
        }
    }
}
