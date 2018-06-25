using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using RoyT.AStar;
using System.Collections.Generic;

namespace Dungeon_Crawler
{
    class NormalLevel : Level
    {
        public NormalLevel(Map map, Grid grid, int cellSize, List<Character> enemies, List<Item> items, List<Rock> rocks, Texture2D floor, Texture2D wall, Portal portal, List<Cell> occupiedCells)
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
            Finished = false;
        }
        public override void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
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
                    Finished = true;
                    return;
                }
            }
            portal.Update();
            player.Update(gameTime, this, graphicsDevice);

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
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
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

            foreach (var rock in rocks)
            {
                rock.Draw(spriteBatch);
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
    }
}
