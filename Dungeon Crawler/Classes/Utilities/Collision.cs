using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using System;

namespace Dungeon_Crawler
{
    public static class Collision
    {
        public static bool isInBounds(Character character, Level level)
        {
            Cell playerCell;
            int x, y;

            //check most common scenario first
            x = (int)Math.Floor(character.Position.X / character.cellSize);
            y = (int)Math.Floor(character.Position.Y / character.cellSize);
            playerCell = level.map.GetCell(x, y);
            Microsoft.Xna.Framework.Rectangle cellRectangle = new Microsoft.Xna.Framework.Rectangle(playerCell.X * character.cellSize, playerCell.Y * character.cellSize, character.cellSize, character.cellSize);
            Microsoft.Xna.Framework.Rectangle playerRectangle = character.getRectangle();

            if (cellRectangle.Contains(playerRectangle) && playerCell.IsWalkable)
                return true;

            //check if topleft corner is in walkable cell
            x = (int)Math.Floor(character.Position.X / character.cellSize);
            y = (int)Math.Floor(character.Position.Y / character.cellSize);
            playerCell = level.map.GetCell(x, y);
            if (!playerCell.IsWalkable)
                return false;
            //check if bottomright corner is in walkable cell
            x = (int)Math.Floor((character.Position.X + character.getWidth()) / character.cellSize);
            y = (int)Math.Floor((character.Position.Y + character.getHeight()) / character.cellSize);
            playerCell = level.map.GetCell(x, y);
            if (!playerCell.IsWalkable)
                return false;
            //check if topright corner is in walkable cell
            x = (int)Math.Floor((character.Position.X + character.getWidth()) / character.cellSize);
            y = (int)Math.Floor(character.Position.Y / character.cellSize);
            playerCell = level.map.GetCell(x, y);
            if (!playerCell.IsWalkable)
                return false;
            //check if bottomleft corner is in walkable cell
            x = (int)Math.Floor(character.Position.X / character.cellSize);
            y = (int)Math.Floor((character.Position.Y + character.getHeight()) / character.cellSize);
            playerCell = level.map.GetCell(x, y);
            if (!playerCell.IsWalkable)
                return false;

            return true;
        }

        public static void getInBounds(Character character, Level level)
        {
            Vector2 originalPosition = character.Position;

            for(int i = 1; i < 100; i++)
            {
                //top
                character._position.Y -= i;
                if (isInBounds(character, level))
                    return;
                else
                    character.Position = originalPosition;
                //bottom
                character._position.Y += i;
                if (isInBounds(character, level))
                    return;
                else
                    character.Position = originalPosition;
                //left
                character._position.X -= i;
                if (isInBounds(character, level))
                    return;
                else
                    character.Position = originalPosition;
                //right
                character._position.X += i;
                if (isInBounds(character, level))
                    return;
                else
                    character.Position = originalPosition;

                //topleft
                character._position.Y -= i;
                character._position.X -= i;
                if (isInBounds(character, level))
                    return;
                else
                    character.Position = originalPosition;
                //topright
                character._position.Y -= i;
                character._position.X += i;
                if (isInBounds(character, level))
                    return;
                else
                    character.Position = originalPosition;
                //bottomleft
                character._position.Y += i;
                character._position.X -= i;
                if (isInBounds(character, level))
                    return;
                else
                    character.Position = originalPosition;
                //bottomright
                character._position.Y += i;
                character._position.X += i;
                if (isInBounds(character, level))
                    return;
                else
                    character.Position = originalPosition;

            }
        }

        public static bool checkCollisionInGivenDirection(int currentDirection,float speed, Level level)
        {
            throw new NotImplementedException();
        }

        public static bool checkCollision(Character character1, Character character2, GraphicsDevice graphicsDevice)
        {
            if (character1 == null || character2 == null) return false;
            Microsoft.Xna.Framework.Rectangle character1Rectangle = character1.getRectangle();
            Color[] character1TextureData = character1.getCurrentTextureData(graphicsDevice);

            Microsoft.Xna.Framework.Rectangle character2Rectangle = character2.getRectangle();
            Color[] character2TextureData = character2.getCurrentTextureData(graphicsDevice);

            if (character1Rectangle.Intersects(character2Rectangle))
            {
                if (Collision.IntersectPixels(character1Rectangle, character1TextureData,
                                    character2Rectangle, character2TextureData))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool checkCollision(Character character,Obstacle obstacle,GraphicsDevice graphicsDevice)
        {
            if (character == null || obstacle == null) return false;
            Color[] characterTextureData = character.getCurrentTextureData(graphicsDevice);

            Microsoft.Xna.Framework.Rectangle characterRectangle = character.getRectangle();

            Microsoft.Xna.Framework.Rectangle obstacleRectangle = obstacle.getRectangle();

            if (characterRectangle.Intersects(obstacleRectangle))
            {
                if (Collision.IntersectPixels(characterRectangle, characterTextureData,
                                    obstacleRectangle, obstacle.TextureData))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool checkCollision(Character character, Projectile projectile, GraphicsDevice graphicsDevice)
        {
            if (character == null || projectile == null) return false;
            Color[] characterTextureData = character.getCurrentTextureData(graphicsDevice);

            Microsoft.Xna.Framework.Rectangle characterRectangle = character.getRectangle();

            Microsoft.Xna.Framework.Rectangle projectileRectangle = projectile.getRectangle();

            if (characterRectangle.Intersects(projectileRectangle))
            {
                if (Collision.IntersectPixels(characterRectangle, characterTextureData,
                                    projectileRectangle, projectile.TextureData))
                {
                    return true;
                }
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
        public static bool IntersectPixels(Microsoft.Xna.Framework.Rectangle rectangleA, Color[] dataA,
                                           Microsoft.Xna.Framework.Rectangle rectangleB, Color[] dataB)
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
