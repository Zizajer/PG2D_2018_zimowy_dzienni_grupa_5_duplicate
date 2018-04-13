using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using RogueSharp;
using Microsoft.Xna.Framework;
using System;

namespace Dungeon_Crawler
{
    public class Level
    {
        public Map map;
        public int cellSize;
        public List<Item> items;
        public List<Enemy> enemies;
        public List<Rock> rocks;
        public List<Cell> occupiedCells;
        public List<Projectile> projectiles;

        List<Texture2D> allItems;
        List<String> allItemsNames;

        public Texture2D floor;
        public Texture2D wall;
        public Texture2D fireball;
        public Portal portal;
        public Player player;

        public bool finished { get; set; }
        public Level(Map map, int cellSize, List<Enemy> enemies, List<Texture2D> allItems, List<String> allItemsNames, List<Item> items, List<Rock> rocks, Texture2D floor, Texture2D wall, Portal portal, List<Cell> occupiedCells, Texture2D fireball)
        {
            this.map = map;
            this.cellSize = cellSize;
            this.enemies = enemies;
            this.items = items;
            this.allItems = allItems;
            this.allItemsNames = allItemsNames;
            this.rocks = rocks;
            this.floor = floor;
            this.wall = wall;
            this.portal = portal;
            this.occupiedCells = occupiedCells;
            this.fireball = fireball;
            projectiles = new List<Projectile>();
            finished = false;
        }
        public void addPlayer(Player player)
        {
            this.player = player;
        }
        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            if ((Math.Abs(player.Position.X - player.Position.X) < player.getWidth() + portal.Texture.Width) && (Math.Abs(player.Position.Y - player.Position.Y) < player.getHeight() + portal.Texture.Height) && enemies.Count == 0)
            {
                if (Collision.checkCollision(player.getRectangle(),player, portal, graphicsDevice))
                {
                    finished = true;
                    return;
                }
            }

            player.Update(gameTime, this, graphicsDevice);
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                Enemy enemy = enemies[i];
                enemy.Update(gameTime, this, graphicsDevice);
                if (enemy.Health == 0)
                {
                    enemies.RemoveAt(i);
                    if (Global.random.Next(10) > 6)
                    {
                        int rand = Global.random.Next(2) + 1;
                        Item tempItem = new Item(new Vector2(enemy.Position.X + cellSize / 3, enemy.Position.Y + cellSize / 3), allItems[rand], allItemsNames[rand]);
                        items.Add(tempItem);
                    }
                }
            }
        

            Item[] itemArray = items.ToArray();
            for(int i = 0; i<items.Count;i++)
            {
                if ((Math.Abs(player.Position.X - player.Position.X) < player.getWidth() + itemArray[i].Texture.Width) && (Math.Abs(player.Position.Y - player.Position.Y) < player.getHeight() + itemArray[i].Texture.Height))
                {
                    if (Collision.checkCollision(player.getRectangle(),player, itemArray[i], graphicsDevice))
                    {
                        player.inventory.Add(itemArray[i]);
                        items.Remove(itemArray[i]);
                    }
                }
            }

            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                Projectile projectile = projectiles[i];
                projectile.Update(gameTime, this, graphicsDevice);
                if (projectile.isMarkedToDelete)
                {
                    projectiles.RemoveAt(i);
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

            foreach (var rock in rocks)
            {
                rock.Draw(spriteBatch);
            }

            foreach (var enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }

            foreach (var projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
            }

            if (enemies.Count == 0)
            {
                portal.Draw(spriteBatch);
            }
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
