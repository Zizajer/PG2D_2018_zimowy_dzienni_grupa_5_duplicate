using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using RogueSharp;
using Microsoft.Xna.Framework;
using System;
using RoyT.AStar;
using System.Diagnostics;

namespace Dungeon_Crawler
{
    public class Level
    {
        public bool isBossLevel;
        public Map map;
        public Grid grid;
        public int cellSize;
        public List<Item> items;
        public List<Character> enemies;
        public List<Rock> rocks;
        public List<Cell> occupiedCells;
        public List<PlayerProjectile> playerProjectiles;
        public List<EnemyProjectile> enemyProjectiles;
        public List<AttackAnimation> attackAnimations;
        List<Texture2D> allItems;
        List<String> allItemsNames;

        public Texture2D floor;
        public Texture2D wall;
        public Texture2D fireball;
        public Portal portal;
        public Player player;
        public Texture2D fireballBoss;

        public bool finished { get; set; }
        public Level(Map map, Grid grid, int cellSize, List<Character> enemies, List<Texture2D> allItems, List<String> allItemsNames, List<Item> items, List<Rock> rocks, Texture2D floor, Texture2D wall, Portal portal, List<Cell> occupiedCells, Texture2D fireball)
        {
            this.map = map;
            this.grid = grid;
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
            playerProjectiles = new List<PlayerProjectile>();
            enemyProjectiles = new List<EnemyProjectile>();
            attackAnimations = new List<AttackAnimation>();
            finished = false;
            isBossLevel = false;
        }

        public Level(Map map, Grid grid, int cellSize, List<Character> enemies1, List<Texture2D> allItems, List<String> allItemsNames, Texture2D floor, Texture2D wall, Portal portal, List<Cell> occupiedCells, Texture2D fireball, Texture2D fireballBoss)
        {
            this.map = map;
            this.grid = grid;
            this.cellSize = cellSize;
            enemies = enemies1;
            items = new List<Item>();
            this.allItems = allItems;
            this.allItemsNames = allItemsNames;
            rocks = new List<Rock>();
            this.floor = floor;
            this.wall = wall;
            this.portal = portal;
            this.occupiedCells = occupiedCells;
            this.fireball = fireball;
            this.fireballBoss = fireballBoss;
            playerProjectiles = new List<PlayerProjectile>();
            enemyProjectiles = new List<EnemyProjectile>();
            attackAnimations = new List<AttackAnimation>();
            finished = false;
            isBossLevel = true;
        }

        public void addPlayer(Player player)
        {
            this.player = player;
        }

        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            if (enemies.Count == 0)
            {
                if (portal.Position.X == 0 && portal.Position.Y == 0)
                {
                    Cell portalcell = GetRandomEmptyCellNearCoord(new Vector2(player.CellX, player.CellY), 3);
                    occupiedCells.Add(portalcell);
                    portal.Position = new Vector2(portalcell.X * cellSize, portalcell.Y * cellSize);
                    Global.SoundManager.playPortalActivated();
                }
                if (Collision.checkCollision(player.getRectangle(), player, portal, graphicsDevice))
                {
                    finished = true;
                    return;
                }
            }

            player.Update(gameTime, this, graphicsDevice);

            if (isBossLevel)
            {
                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    Character character = enemies[i];
                    character.Update(gameTime, this, graphicsDevice);
                    if (character.CurrentHealth <= 0)
                    {
                        enemies.RemoveAt(i);
                        if (Global.random.Next(10) > 0) //90% of chance
                        {
                            int numberOfItems = Global.random.Next(2, 5);
                            int positionOffset = 0;
                            for (int j = 0; j < numberOfItems; j++)
                            {
                                int itemId = Global.random.Next(allItemsNames.Count);
                                Item tempItem = new Item(new Vector2(character.Center.X + positionOffset, character.Center.Y + positionOffset), allItems[itemId], allItemsNames[itemId]);
                                items.Add(tempItem);
                                positionOffset += cellSize / 2;
                            }
                        }
                    }
                } 
            }
            else
            {
                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    Character enemy = enemies[i];
                    enemy.Update(gameTime, this, graphicsDevice);
                    if (enemy.CurrentHealth <= 0)
                    {
                        grid.SetCellCost(new Position(enemy.CellX, enemy.CellY), 1.0f);
                        enemies.RemoveAt(i);
                        if (Global.random.Next(20) == 19) //5% of chance
                        {
                            int itemId = Global.random.Next(allItemsNames.Count);
                            Item tempItem = new Item(new Vector2(enemy.Center.X, enemy.Center.Y), allItems[itemId], allItemsNames[itemId]);
                            items.Add(tempItem);
                        }
                    }
                }
            }

            Item[] itemArray = items.ToArray();
            for (int i = 0; i < items.Count; i++)
            {
                if ((Math.Abs(player.Position.X - player.Position.X) < player.getWidth() + itemArray[i].Texture.Width) && (Math.Abs(player.Position.Y - player.Position.Y) < player.getHeight() + itemArray[i].Texture.Height))
                {
                    if (Collision.checkCollision(player.getRectangle(), player, itemArray[i], graphicsDevice))
                    {
                        player.inventory.Add(itemArray[i]);
                        items.Remove(itemArray[i]);
                    }
                }
            }


            for (int i = playerProjectiles.Count - 1; i >= 0; i--)
            {
                PlayerProjectile playerProjectile = playerProjectiles[i];
                playerProjectile.Update(gameTime, this, graphicsDevice);
                if (playerProjectile.isMarkedToDelete)
                {
                    playerProjectiles.RemoveAt(i);
                }
            }

            for (int i = enemyProjectiles.Count - 1; i >= 0; i--)
            {
                EnemyProjectile enemyProjectile = enemyProjectiles[i];
                enemyProjectile.Update(gameTime, this, graphicsDevice);
                if (enemyProjectile.isMarkedToDelete)
                {
                    enemyProjectiles.RemoveAt(i);
                }
            }

            foreach (AttackAnimation aa in attackAnimations)
            {
                aa.Update(gameTime);
            }

            for (int i = 0; i < attackAnimations.Count; i++)
            {
                if (attackAnimations[i].isActive == false)
                {
                    attackAnimations.Remove(attackAnimations[i]);
                    i--;
                }
            }

        }
        public void Draw(GameTime gameTime,SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);

            foreach (Cell cell in map.GetAllCells())
            {
                var position = new Vector2(cell.X * floor.Width, cell.Y * floor.Width);
                if (cell.IsWalkable)
                {
                    if(grid.GetCellCost(new Position(cell.X, cell.Y)) > 1.0f)
                    {
                        spriteBatch.Draw(floor, position, null, Color.Blue, 0.0f, Vector2.One, 1.0f, SpriteEffects.None, Layers.Cells);
                    }
                    else
                    {
                        spriteBatch.Draw(floor, position, null, Color.White, 0.0f, Vector2.One, 1.0f, SpriteEffects.None, Layers.Cells);
                    }
                    
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

            foreach (AttackAnimation aa in attackAnimations)
            {
                aa.Draw(spriteBatch);
            }

            if (!isBossLevel)
            {
                foreach (var rock in rocks)
                {
                    rock.Draw(spriteBatch);
                }
            }
            
            foreach (var playerProjectile in playerProjectiles)
            {
                playerProjectile.Draw(spriteBatch);
            }

            foreach (var enemyProjectile in enemyProjectiles)
            {
                enemyProjectile.Draw(spriteBatch);
            }

            if (enemies.Count == 0)
            {
                portal.Draw(spriteBatch);
            }
            
            spriteBatch.End();

            player.Draw(spriteBatch);

            foreach (var enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }

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
            } while (!tempCell.IsWalkable || occupiedCells.Contains(tempCell) || grid.GetCellCost(new Position(x, y)) > 1.0f);
            grid.SetCellCost(new Position(x, y), 5.0f);
            return new Vector2(tempCell.X*floor.Width+ floor.Width/3, tempCell.Y * floor.Width +floor.Width / 3);
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