﻿using Microsoft.Xna.Framework.Graphics;
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
        public List<Projectile> Projectiles;

        public List<AttackAnimation> attackAnimations;

        public Texture2D floor;
        public Texture2D wall;
        public Portal portal;
        public Player player;

        public bool finished { get; set; }
        public Level(Map map, Grid grid, int cellSize, List<Character> enemies, List<Item> items, List<Rock> rocks, Texture2D floor, Texture2D wall, Portal portal, List<Cell> occupiedCells)
        {
            this.map = map;
            this.grid = grid;
            this.cellSize = cellSize;
            this.enemies = enemies;
            this.items = items;
            this.rocks = rocks;
            this.floor = floor;
            this.wall = wall;
            this.portal = portal;
            this.occupiedCells = occupiedCells;
            Projectiles = new List<Projectile>();
            attackAnimations = new List<AttackAnimation>();
            finished = false;
            isBossLevel = false;
        }

        public Level(Map map, Grid grid, int cellSize, List<Character> enemies1, List<Item> items, Texture2D floor, Texture2D wall, Portal portal, List<Cell> occupiedCells)
        {
            this.map = map;
            this.grid = grid;
            this.cellSize = cellSize;
            enemies = enemies1;
            this.items = items;
            rocks = new List<Rock>();
            this.floor = floor;
            this.wall = wall;
            this.portal = portal;
            this.occupiedCells = occupiedCells;
            Projectiles = new List<Projectile>();
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
                    Global.SoundManager.portalactivated.Play();
                }
                if (Collision.checkCollision(player.getRectangle(), player, portal, graphicsDevice))
                {
                    Global.SoundManager.teleportLevel.Play();
                    finished = true;
                    return;
                }
            }
            portal.Update();
            player.Update(gameTime, this, graphicsDevice);

            if (isBossLevel)
            {
                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    Character character = enemies[i];
                    character.Update(gameTime, this, graphicsDevice);
                    if (character.CurrentHealth <= 0)
                    {
                        grid.SetCellCost(new Position(character.CellX, character.CellY), 1.0f);
                        if (character.NextCell != null)
                        grid.SetCellCost(new Position(character.NextCell.X, character.NextCell.Y), 1.0f);
                        character.DropAllItems(this);
                        enemies.RemoveAt(i);     
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
                        if (enemy.NextCell != null)
                        {
                            grid.SetCellCost(new Position(enemy.NextCell.X, enemy.NextCell.Y), 1.0f);
                        }
                        enemy.DropAllItems(this);
                        enemies.RemoveAt(i);
                    }
                }
            }

            for (int i = Projectiles.Count - 1; i >= 0; i--)
            {
                Projectile Projectile = Projectiles[i];
                Projectile.Update(gameTime, this, graphicsDevice);
                if (Projectile.isMarkedToDelete)
                {
                    Projectiles.RemoveAt(i);
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
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);

            foreach (Cell cell in map.GetAllCells())
            {
                var position = new Vector2(cell.X * floor.Width, cell.Y * floor.Width);
                if (cell.IsWalkable)
                {
                    /*
                    if(grid.GetCellCost(new Position(cell.X, cell.Y)) > 1.0f)
                    {
                        spriteBatch.Draw(floor, position, null, Color.Blue, 0.0f, Vector2.One, 1.0f, SpriteEffects.None, Layers.Cells);
                    }
                    else
                    {*/
                        spriteBatch.Draw(floor, position, null, Color.White, 0.0f, Vector2.One, 1.0f, SpriteEffects.None, Layers.Cells);
                    //}
                    
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

            foreach (var Projectile in Projectiles)
            {
                Projectile.Draw(spriteBatch);
            }

            Global.Gui.DrawEnemyPlates(spriteBatch, gameTime);

            spriteBatch.End();

            if (enemies.Count == 0)
            {
                portal.Draw(spriteBatch);
            }

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