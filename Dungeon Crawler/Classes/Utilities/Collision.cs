using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RoyT.AStar;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dungeon_Crawler
{
    public static class Collision
    {
        public static bool checkCollisionInGivenCell(RogueSharp.Cell futureNextCell, Level level, GraphicsDevice graphicsDevice)
        {
            if (level.grid.GetCellCost(new Position(futureNextCell.X, futureNextCell.Y)) > 1.0f)
                return true;

            return false;
        }

        public static RogueSharp.Cell getCellFromDirection(RogueSharp.Cell currentCell, Character.Directions currentDirection, Level level)
        {
            if (currentDirection == Character.Directions.Top)
                return level.map.GetCell(currentCell.X, currentCell.Y-1);

            if (currentDirection == Character.Directions.Bottom)
                return level.map.GetCell(currentCell.X, currentCell.Y+1);

            if (currentDirection == Character.Directions.Left)
                return level.map.GetCell(currentCell.X-1, currentCell.Y);

            if (currentDirection == Character.Directions.Right)
                return level.map.GetCell(currentCell.X+1, currentCell.Y);

            if (currentDirection == Character.Directions.TopLeft)
                return level.map.GetCell(currentCell.X-1, currentCell.Y-1);

            if (currentDirection == Character.Directions.TopRight)
                return level.map.GetCell(currentCell.X+1, currentCell.Y-1);

            if (currentDirection == Character.Directions.BottomLeft)
                return level.map.GetCell(currentCell.X-1, currentCell.Y+1);

            if (currentDirection == Character.Directions.BottomRight)
                return level.map.GetCell(currentCell.X+1, currentCell.Y+1);

            return null;
        }

        public static List<Character.Directions> checkIfOneOfDoubleDirectionsIsOk(RogueSharp.Cell currentCell, Character.Directions currentDirection, Level level, GraphicsDevice graphicsDevice)
        {
            List<Character.Directions> dirList = new List<Character.Directions>(2);

            if (currentDirection == Character.Directions.TopLeft)
            {
                Character.Directions top = Character.Directions.Top;
                Character.Directions left = Character.Directions.Left;

                if (!checkCollisionInGivenCell(getCellFromDirection(currentCell, top, level), level, graphicsDevice))
                    dirList.Add(top);
                if (!checkCollisionInGivenCell(getCellFromDirection(currentCell, left, level), level, graphicsDevice))
                    dirList.Add(left);

                return dirList;
            }

            if (currentDirection == Character.Directions.TopRight)
            {
                Character.Directions top = Character.Directions.Top;
                Character.Directions right = Character.Directions.Right;

                if (!checkCollisionInGivenCell(getCellFromDirection(currentCell, top, level), level, graphicsDevice))
                    dirList.Add(top);
                if (!checkCollisionInGivenCell(getCellFromDirection(currentCell, right, level), level, graphicsDevice))
                    dirList.Add(right);

                return dirList;
            }

            if (currentDirection == Character.Directions.BottomLeft)
            {
                Character.Directions bottom = Character.Directions.Bottom;
                Character.Directions left = Character.Directions.Left;

                if (!checkCollisionInGivenCell(getCellFromDirection(currentCell, bottom, level), level, graphicsDevice))
                    dirList.Add(bottom);
                if (!checkCollisionInGivenCell(getCellFromDirection(currentCell, left, level), level, graphicsDevice))
                    dirList.Add(left);

                return dirList;
            }

            if (currentDirection == Character.Directions.BottomRight)
            {
                Character.Directions bottom = Character.Directions.Bottom;
                Character.Directions right = Character.Directions.Right;

                if (!checkCollisionInGivenCell(getCellFromDirection(currentCell, bottom, level), level, graphicsDevice))
                    dirList.Add(bottom);
                if (!checkCollisionInGivenCell(getCellFromDirection(currentCell, right, level), level, graphicsDevice))
                    dirList.Add(right);

                return dirList;
            }
            return dirList;
        }

        internal static void deleteCollidingRock(PiercingProjectile piercingProjectile, Level level, GraphicsDevice graphicsDevice)
        {
            Rock tempRock;
            for(int i = 0; i < level.rocks.Count; i++)
            {
                tempRock = level.rocks[i];
                if (Vector2.Distance(piercingProjectile.Center, tempRock.Center) < level.cellSize)
                {
                    if (checkCollision(piercingProjectile, tempRock, graphicsDevice)){
                        int CellX = (int)Math.Floor(tempRock.Center.X / level.cellSize);
                        int CellY = (int)Math.Floor(tempRock.Center.Y / level.cellSize);
                        level.grid.SetCellCost(new Position(CellX, CellY), 1.0f);
                        level.rocks.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public static bool isCollidingWithPortal(Sprite sprite, Level level, GraphicsDevice graphicsDevice)
        {
            Portal portal = level.portal;
            {
                if (Vector2.Distance(sprite.Center, portal.Center) < level.cellSize)
                {
                    if (checkCollision(sprite, portal, graphicsDevice))
                        return true;
                }
            }
            return false;
        }

        public static bool isCollidingWithRocks(Rectangle characterRectangle, Character character, Level level, GraphicsDevice graphicsDevice)
        {
            foreach (Rock rock in level.rocks)
            {
                if (Vector2.Distance(characterRectangle.Center.ToVector2(), rock.Center) < level.cellSize)
                {
                    if (checkCollision(characterRectangle, character, rock, graphicsDevice))
                        return true;
                }
            }
            return false;
        }

        public static bool isCollidingWithRocks(Sprite sprite, Level level, GraphicsDevice graphicsDevice)
        {
            foreach (Rock rock in level.rocks)
            {
                if (Vector2.Distance(sprite.Center, rock.Center) < level.cellSize)
                {
                    if (checkCollision(sprite, rock, graphicsDevice))
                        return true;
                }
            }
            return false;
        }

        public static bool checkCollision(Rectangle character1Rectangle, Character character, Sprite sprite, GraphicsDevice graphicsDevice)
        {
            if (character == null || sprite == null) return false;
            Level level = Global.levelmanager.levels[Global.levelmanager.player.CurrentMapLevel];
            if (Vector2.Distance(new Vector2(character1Rectangle.Center.X, character1Rectangle.Center.Y), sprite.Center) > 2.5 * level.cellSize)
                return false;

            Color[] characterTextureData = character.getCurrentTextureData(graphicsDevice);
            Rectangle characterRectangle = character1Rectangle;

            Rectangle spriteRectangle = sprite.getRectangle();

            if (characterRectangle.Intersects(spriteRectangle))
            {
                if (IntersectPixels(characterRectangle, characterTextureData, spriteRectangle, sprite.TextureData))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool checkCollision(Rectangle rectangle1, Rectangle rectangle2)
        {
            return rectangle1.Intersects(rectangle2);
        }

        public static bool checkCollision(Sprite sprite1, Sprite sprite2, GraphicsDevice graphicsDevice)
        {
            if (sprite1 == null || sprite2 == null) return false;

            Level level=Global.levelmanager.levels[Global.levelmanager.player.CurrentMapLevel];
            if (Vector2.Distance(sprite1.Center, sprite2.Center) > 1.5 * level.cellSize)
                return false;
            
            Rectangle sprite1Rectangle = sprite1.getRectangle();

            Rectangle sprite2Rectangle = sprite2.getRectangle();

            if (sprite1Rectangle.Intersects(sprite2Rectangle))
            {
                return true;
            }
            return false;
        }


        // source: http://xbox.create.msdn.com/en-US/education/catalog/tutorial/collision_2d_perpixel_transformed
        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels
        /// between two sprites.
        /// </summary>
        /// <param name="rectangleA">Bounding rectangle of the first sprite</param>
        /// <param name="dataA">Pixel data of the first sprite</param>
        /// <param name="rectangleB">Bouding rectangle of the second sprite</param>
        /// <param name="dataB">Pixel data of the second sprite</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static bool IntersectPixels(Rectangle rectangleA, Color[] dataA,
                                           Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }
            // No intersection found
            return false;
        }
    }
}
