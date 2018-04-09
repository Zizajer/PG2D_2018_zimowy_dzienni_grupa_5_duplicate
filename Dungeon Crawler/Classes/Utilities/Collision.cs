using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using System;

namespace Dungeon_Crawler
{
    public static class Collision
    {
        public static bool isCharacterInBounds(Character character, Level level)
        {
            //most common scenario first to optimise calculations
            if (isWholeCharacterInBounds(character, level))
                return true;

            Microsoft.Xna.Framework.Rectangle characterRectangle = character.getRectangle();
            //if not we check every corner
            if (!isTopLeftCornerInBounds(characterRectangle, level))
                return false;
            if (!isTopRightCornerInBounds(characterRectangle, level))
                return false;
            if (!isBottomLeftCornerInBounds(characterRectangle, level))
                return false;
            if (!isBottomRightCornerInBounds(characterRectangle, level))
                return false;

            //all corners are in bounds so we good
            return true;
        }

        public static bool isWholeCharacterInBounds(Character character, Level level)
        {
            int x = (int)Math.Floor(character.Position.X / level.cellSize);
            int y = (int)Math.Floor(character.Position.Y / level.cellSize);
            Cell characterCell = level.map.GetCell(x, y);
            Microsoft.Xna.Framework.Rectangle cellRectangle = new Microsoft.Xna.Framework.Rectangle(characterCell.X * level.cellSize, characterCell.Y * level.cellSize, level.cellSize, level.cellSize);
            Microsoft.Xna.Framework.Rectangle characterRectangle = character.getRectangle();

            if (cellRectangle.Contains(characterRectangle) && characterCell.IsWalkable)
                return true;
            else
                return false;
        }

        public static bool isTopLeftCornerInBounds(Microsoft.Xna.Framework.Rectangle characterRectangle, Level level)
        {
            int x = characterRectangle.Left / level.cellSize;
            int y = characterRectangle.Top / level.cellSize;
            Cell characterCell = level.map.GetCell(x, y);
            if (characterCell.IsWalkable)
                return true;
            else
                return false;
        }

        public static bool isTopRightCornerInBounds(Microsoft.Xna.Framework.Rectangle characterRectangle, Level level)
        {

            int x = characterRectangle.Right / level.cellSize;
            int y = characterRectangle.Top / level.cellSize;
            Cell characterCell = level.map.GetCell(x, y);
            if (characterCell.IsWalkable)
                return true;
            else
                return false;
        }

        public static bool isBottomLeftCornerInBounds(Microsoft.Xna.Framework.Rectangle characterRectangle, Level level)
        {
            int x = characterRectangle.Left/ level.cellSize;
            int y = characterRectangle.Bottom / level.cellSize;
            Cell characterCell = level.map.GetCell(x, y);
            if (characterCell.IsWalkable)
                return true;
            else
                return false;
        }

        public static bool isBottomRightCornerInBounds(Microsoft.Xna.Framework.Rectangle characterRectangle, Level level)
        {
            int x = characterRectangle.Right / level.cellSize;
            int y = characterRectangle.Bottom / level.cellSize;
            Cell characterCell = level.map.GetCell(x, y);
            if (characterCell.IsWalkable)
                return true;
            else
                return false;
        }

        public static int isStuck(Character character, Level level, GraphicsDevice graphicsDevice)
        {
            for (int i = 1; i < 9; i++)
            {
                if (!checkCollisionInGivenDirection(i, character, level, graphicsDevice))
                    return i;
            }
            return 0;
        }

        public static void unStuck(Character character, Level level, GraphicsDevice graphicsDevice)
        {
            Vector2 originalPosition = character.Position;

            for(int i = 1; i < 20; i++)
            {
                //top
                character._position.Y -= i;
                if (isCharacterInBounds(character, level))
                    return;
                else
                    character.Position = originalPosition;
                //bottom
                character._position.Y += i;
                if (isCharacterInBounds(character, level))
                    return;
                else
                    character.Position = originalPosition;
                //left
                character._position.X -= i;
                if (isCharacterInBounds(character, level))
                    return;
                else
                    character.Position = originalPosition;
                //right
                character._position.X += i;
                if (isCharacterInBounds(character, level))
                    return;
                else
                    character.Position = originalPosition;

                //topleft
                character._position.Y -= i;
                character._position.X -= i;
                if (isCharacterInBounds(character, level))
                    return;
                else
                    character.Position = originalPosition;
                //topright
                character._position.Y -= i;
                character._position.X += i;
                if (isCharacterInBounds(character, level))
                    return;
                else
                    character.Position = originalPosition;
                //bottomleft
                character._position.Y += i;
                character._position.X -= i;
                if (isCharacterInBounds(character, level))
                    return;
                else
                    character.Position = originalPosition;
                //bottomright
                character._position.Y += i;
                character._position.X += i;
                if (isCharacterInBounds(character, level))
                    return;
                else
                    character.Position = originalPosition;

            }
        }

        public static int checkIfOneOfDirectionsIsOk(int currentDirection, Character character, Level level, GraphicsDevice graphicsDevice)
        {
            if (currentDirection == (int)Character.Directions.TopLeft)
            {
                int top = (int)Character.Directions.Top;
                int left = (int)Character.Directions.Left;

                if (!checkCollisionInGivenDirection(top, character, level, graphicsDevice))
                    return top;
                else if (!checkCollisionInGivenDirection(left, character, level, graphicsDevice))
                    return left;
                else
                    return (int)Character.Directions.None;
            }

            if (currentDirection == (int)Character.Directions.TopRight)
            {
                int top = (int)Character.Directions.Top;
                int right = (int)Character.Directions.Right;

                if (!checkCollisionInGivenDirection(top, character, level, graphicsDevice))
                    return top;
                else if (!checkCollisionInGivenDirection(right, character, level, graphicsDevice))
                    return right;
                else
                    return (int)Character.Directions.None;
            }

            if (currentDirection == (int)Character.Directions.BottomLeft)
            {
                int bottom = (int)Character.Directions.Bottom;
                int left = (int)Character.Directions.Left;

                if (!checkCollisionInGivenDirection(bottom, character, level, graphicsDevice))
                    return bottom;
                else if (!checkCollisionInGivenDirection(left, character, level, graphicsDevice))
                    return left;
                else
                    return (int)Character.Directions.None;
            }

            if (currentDirection == (int)Character.Directions.BottomRight)
            {
                int bottom = (int)Character.Directions.Bottom;
                int right = (int)Character.Directions.Right;

                if (!checkCollisionInGivenDirection(bottom, character, level, graphicsDevice))
                    return bottom;
                else if (!checkCollisionInGivenDirection(right, character, level, graphicsDevice))
                    return right;
                else
                    return (int)Character.Directions.None;
            }
            return (int)Character.Directions.None;
        }

        public static bool checkCollisionInGivenDirection(int currentDirection,Character character, Level level, GraphicsDevice graphicsDevice)
        {
            if (currentDirection == (int)Character.Directions.Top)
            {
                Microsoft.Xna.Framework.Rectangle characterRectangle = character.getRectangle();
                characterRectangle.Y -= (int)character.Speed;
                if (isTopLeftCornerInBounds(characterRectangle, level) && isTopRightCornerInBounds(characterRectangle, level))
                    if (!isCollidingWithEverythingElse(currentDirection, characterRectangle, character, level, graphicsDevice))
                        return false;
                    else
                        return true;

                else
                    return true;
            }
                
            if (currentDirection == (int)Character.Directions.Bottom)
            {
                Microsoft.Xna.Framework.Rectangle characterRectangle = character.getRectangle();
                characterRectangle.Y += (int)character.Speed;
                if (isBottomLeftCornerInBounds(characterRectangle, level) && isBottomRightCornerInBounds(characterRectangle, level))
                    if (!isCollidingWithEverythingElse(currentDirection, characterRectangle, character, level, graphicsDevice))
                        return false;
                    else
                        return true;
                else
                    return true;
            }

            if (currentDirection == (int)Character.Directions.Left)
            {
                Microsoft.Xna.Framework.Rectangle characterRectangle = character.getRectangle();
                characterRectangle.X -= (int)character.Speed;
                if (isTopLeftCornerInBounds(characterRectangle, level) && isBottomLeftCornerInBounds(characterRectangle, level))
                    if (!isCollidingWithEverythingElse(currentDirection, characterRectangle, character, level, graphicsDevice))
                        return false;
                    else
                        return true;
                else
                    return true;
            }
 
            if (currentDirection == (int)Character.Directions.Right)
            {
                Microsoft.Xna.Framework.Rectangle characterRectangle = character.getRectangle();
                characterRectangle.X += (int)character.Speed;
                if (isTopRightCornerInBounds(characterRectangle, level) && isBottomRightCornerInBounds(characterRectangle, level))
                    if (!isCollidingWithEverythingElse(currentDirection, characterRectangle, character, level, graphicsDevice))
                        return false;
                    else
                        return true;
                else
                    return true;
            }

            if (currentDirection == (int)Character.Directions.TopLeft)
            {
                Microsoft.Xna.Framework.Rectangle characterRectangle = character.getRectangle();
                characterRectangle.Y -= (int)character.Speed;
                characterRectangle.X -= (int)character.Speed;

                if (isTopLeftCornerInBounds(characterRectangle, level) && isBottomLeftCornerInBounds(characterRectangle, level) && isTopRightCornerInBounds(characterRectangle, level))
                    if (!isCollidingWithEverythingElse(currentDirection, characterRectangle, character, level, graphicsDevice))
                        return false;
                    else
                        return true;
                else
                    return true;
            }

            if (currentDirection == (int)Character.Directions.TopRight)
            {
                Microsoft.Xna.Framework.Rectangle characterRectangle = character.getRectangle();
                characterRectangle.Y -= (int)character.Speed;
                characterRectangle.X += (int)character.Speed;

                if (isTopRightCornerInBounds(characterRectangle, level) && isTopLeftCornerInBounds(characterRectangle, level) && isBottomRightCornerInBounds(characterRectangle, level))
                    if (!isCollidingWithEverythingElse(currentDirection, characterRectangle, character, level, graphicsDevice))
                        return false;
                    else
                        return true;
                else
                    return true;
            }

            if (currentDirection == (int)Character.Directions.BottomLeft)
            {
                Microsoft.Xna.Framework.Rectangle characterRectangle = character.getRectangle();
                characterRectangle.Y += (int)character.Speed;
                characterRectangle.X -= (int)character.Speed;

                if (isBottomLeftCornerInBounds(characterRectangle, level) && isTopLeftCornerInBounds(characterRectangle, level) && isBottomRightCornerInBounds(characterRectangle, level))
                    if (!isCollidingWithEverythingElse(currentDirection, characterRectangle, character, level, graphicsDevice))
                        return false;
                    else
                        return true;
                else
                    return true;
            }
            if (currentDirection == (int)Character.Directions.BottomRight)
            {
                Microsoft.Xna.Framework.Rectangle characterRectangle = character.getRectangle();
                characterRectangle.Y += (int)character.Speed;
                characterRectangle.X += (int)character.Speed;

                if (isBottomRightCornerInBounds(characterRectangle, level) && isBottomLeftCornerInBounds(characterRectangle, level) && isTopRightCornerInBounds(characterRectangle, level))
                    if (!isCollidingWithEverythingElse(currentDirection, characterRectangle, character, level, graphicsDevice))
                        return false;
                    else
                        return true;
                else
                    return true;
            }
            return false;
        }

        public static bool isCollidingWithEverythingElse(int currentDirection, Microsoft.Xna.Framework.Rectangle characterRectangle, Character character, Level level, GraphicsDevice graphicsDevice)
        {
            if (character.GetType() != typeof(Player))
            {
                if (Vector2.Distance(character.Origin, level.player.Origin) < level.cellSize)
                        if (checkCollision(characterRectangle, character, level.player, graphicsDevice))
                            return true;
            }

            foreach (Enemy enemy in level.enemies)
            {
                if (enemy.Equals(character)) continue;
                if (Vector2.Distance(character.Origin,enemy.Origin) < level.cellSize)
                {
                    if (checkCollision(characterRectangle, character, enemy, graphicsDevice))
                        return true;
                }

            }
            foreach (Obstacle obstacle in level.obstacles)
            {
                if (Vector2.Distance(character.Origin, obstacle.Origin) < level.cellSize)
                {
                    if (checkCollision(characterRectangle, character, obstacle, graphicsDevice))
                        return true;
                }
            }
            return false;
        }


        public static bool checkCollision(Microsoft.Xna.Framework.Rectangle characterRectangle, Character character1, Character character2, GraphicsDevice graphicsDevice)
        {
            if (character1 == null || character2 == null) return false;
            Microsoft.Xna.Framework.Rectangle character1Rectangle = characterRectangle;
            Color[] character1TextureData = character1.getCurrentTextureData(graphicsDevice);

            Microsoft.Xna.Framework.Rectangle character2Rectangle = character2.getRectangle();
            Color[] character2TextureData = character2.getCurrentTextureData(graphicsDevice);

            if (character1Rectangle.Intersects(character2Rectangle))
            {
                if (IntersectPixels(character1Rectangle, character1TextureData,
                                    character2Rectangle, character2TextureData))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool checkCollision(Microsoft.Xna.Framework.Rectangle character1Rectangle, Character character,Obstacle obstacle,GraphicsDevice graphicsDevice)
        {
            if (character == null || obstacle == null) return false;
            Color[] characterTextureData = character.getCurrentTextureData(graphicsDevice);

            Microsoft.Xna.Framework.Rectangle characterRectangle = character1Rectangle;

            Microsoft.Xna.Framework.Rectangle obstacleRectangle = obstacle.getRectangle();

            if (characterRectangle.Intersects(obstacleRectangle))
            {
                if (IntersectPixels(characterRectangle, characterTextureData,
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
                if (IntersectPixels(characterRectangle, characterTextureData,
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

        public static bool checkCollision(Character character1, Character character2, GraphicsDevice graphicsDevice)
        {
            if (character1 == null || character2 == null) return false;
            Microsoft.Xna.Framework.Rectangle character1Rectangle = character1.getRectangle();
            Color[] character1TextureData = character1.getCurrentTextureData(graphicsDevice);

            Microsoft.Xna.Framework.Rectangle character2Rectangle = character2.getRectangle();
            Color[] character2TextureData = character2.getCurrentTextureData(graphicsDevice);

            if (character1Rectangle.Intersects(character2Rectangle))
            {
                if (IntersectPixels(character1Rectangle, character1TextureData,
                                    character2Rectangle, character2TextureData))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool checkCollision(Character character, Obstacle obstacle, GraphicsDevice graphicsDevice)
        {
            if (character == null || obstacle == null) return false;
            Color[] characterTextureData = character.getCurrentTextureData(graphicsDevice);

            Microsoft.Xna.Framework.Rectangle characterRectangle = character.getRectangle();

            Microsoft.Xna.Framework.Rectangle obstacleRectangle = obstacle.getRectangle();

            if (characterRectangle.Intersects(obstacleRectangle))
            {
                if (IntersectPixels(characterRectangle, characterTextureData,
                                    obstacleRectangle, obstacle.TextureData))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
