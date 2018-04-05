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
        public List<Item> items;
        public List<Enemy> enemies;
        public List<Obstacle> obstacles;
        public List<Cell> occupiedCells;
        public List<Projectile> projectiles;

        public Texture2D floor;
        public Texture2D wall;
        public Texture2D fireball;
        public Portal portal;
        public Player player;

        public bool finished { get; set; }
        public Level(Map map, List<Enemy> enemies, List<Item> items, List<Obstacle> obstacles, Texture2D floor, Texture2D wall, Portal portal, Player player, List<Cell> occupiedCells, Texture2D fireball)
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
            this.fireball = fireball;
            projectiles = new List<Projectile>();
            finished = false;
        }
        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            if ((Math.Abs(player.Position.X - player.Position.X) < player.getWidth() + portal.Texture.Width) && (Math.Abs(player.Position.Y - player.Position.Y) < player.getHeight() + portal.Texture.Height))
            {
                if (Collision.checkCollision(player, portal, graphicsDevice))
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
                }
            }
        

            Item[] itemArray = items.ToArray();
            for(int i = 0; i<items.Count;i++)
            {
                if ((Math.Abs(player.Position.X - player.Position.X) < player.getWidth() + itemArray[i].Texture.Width) && (Math.Abs(player.Position.Y - player.Position.Y) < player.getHeight() + itemArray[i].Texture.Height))
                {
                    if (Collision.checkCollision(player, itemArray[i], graphicsDevice))
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

            foreach (var obstacle in obstacles)
            {
                obstacle.Draw(spriteBatch);
            }

            foreach (var enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }

            foreach (var projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
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
