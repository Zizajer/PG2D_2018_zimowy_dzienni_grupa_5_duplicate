using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

             Rectangle characterRectangle = character.getRectangle();
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

        public static bool isCharacterInBounds(Rectangle characterRectangle, Level level)
        {
            //most common scenario first to optimise calculations
            if (isWholeCharacterInBounds(characterRectangle, level))
                return true;

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
            if (x < 0 || x >= level.map.Width || y < 0 || y >= level.map.Height)
                return false;
            RogueSharp.Cell characterCell = level.map.GetCell(x, y);
            Rectangle cellRectangle = new  Rectangle(characterCell.X * level.cellSize, characterCell.Y * level.cellSize, level.cellSize, level.cellSize);
            Rectangle characterRectangle = character.getRectangle();

            if (cellRectangle.Contains(characterRectangle) && characterCell.IsWalkable)
                return true;
            else
                return false;
        }
        public static bool isWholeCharacterInBounds(Rectangle characterRectangle, Level level)
        {
            int x = characterRectangle.X / level.cellSize;
            int y = characterRectangle.Y / level.cellSize;
            if (x < 0 || x >= level.map.Width || y < 0 || y >= level.map.Height)
                return false;
            RogueSharp.Cell characterCell = level.map.GetCell(x, y);
            Rectangle cellRectangle = new Rectangle(characterCell.X * level.cellSize, characterCell.Y * level.cellSize, level.cellSize, level.cellSize);

            if (cellRectangle.Contains(characterRectangle) && characterCell.IsWalkable)
                return true;
            else
                return false;
        }

        public static bool isTopLeftCornerInBounds(Rectangle characterRectangle, Level level)
        {
            int x = characterRectangle.Left / level.cellSize;
            int y = characterRectangle.Top / level.cellSize;
            if (x < 0 || x >= level.map.Width || y < 0 || y >= level.map.Height)
                return false;
            RogueSharp.Cell characterCell = level.map.GetCell(x, y);
            if (characterCell.IsWalkable)
                return true;
            else
                return false;
        }

        public static bool isTopRightCornerInBounds(Rectangle characterRectangle, Level level)
        {

            int x = characterRectangle.Right / level.cellSize;
            int y = characterRectangle.Top / level.cellSize;
            if (x < 0 || x >= level.map.Width || y < 0 || y >= level.map.Height)
                return false;
            RogueSharp.Cell characterCell = level.map.GetCell(x, y);
            if (characterCell.IsWalkable)
                return true;
            else
                return false;
        }

        public static bool isBottomLeftCornerInBounds(Rectangle characterRectangle, Level level)
        {
            int x = characterRectangle.Left/ level.cellSize;
            int y = characterRectangle.Bottom / level.cellSize;
            if (x < 0 || x >= level.map.Width || y < 0 || y >= level.map.Height)
                return false;
            RogueSharp.Cell characterCell = level.map.GetCell(x, y);
            if (characterCell.IsWalkable)
                return true;
            else
                return false;
        }

        public static bool isBottomRightCornerInBounds(Rectangle characterRectangle, Level level)
        {
            int x = characterRectangle.Right / level.cellSize;
            int y = characterRectangle.Bottom / level.cellSize;
            if (x < 0 || x >= level.map.Width || y < 0 || y >= level.map.Height)
                return false;
            RogueSharp.Cell characterCell = level.map.GetCell(x, y);
            if (characterCell.IsWalkable)
                return true;
            else
                return false;
        }

        public static void getPlayerInBounds(Character character, Level level, GraphicsDevice graphicsDevice)
        {
            Vector2 originalPosition = character.Position;

            for(int i = 1; i < 60; i++)
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

        public static void unStuck(Character character, Level level, GraphicsDevice graphicsDevice)
        {
            Vector2 originalPosition = character.Position;

            for (int i = 1; i < 30; i++)
            {
                //top
                character._position.Y -= i;
                if (isCharacterInBounds(character, level)&&!isCollidingWithEverythingElse(character.getRectangle(),character,level,graphicsDevice))
                    return;
                else
                    character.Position = originalPosition;
                //bottom
                character._position.Y += i;
                if (isCharacterInBounds(character, level)&&!isCollidingWithEverythingElse(character.getRectangle(),character,level,graphicsDevice))
                    return;
                else
                    character.Position = originalPosition;
                //left
                character._position.X -= i;
                if (isCharacterInBounds(character, level)&&!isCollidingWithEverythingElse(character.getRectangle(),character,level,graphicsDevice))
                    return;
                else
                    character.Position = originalPosition;
                //right
                character._position.X += i;
                if (isCharacterInBounds(character, level)&&!isCollidingWithEverythingElse(character.getRectangle(),character,level,graphicsDevice))
                    return;
                else
                    character.Position = originalPosition;

                //topleft
                character._position.Y -= i;
                character._position.X -= i;
                if (isCharacterInBounds(character, level)&&!isCollidingWithEverythingElse(character.getRectangle(),character,level,graphicsDevice))
                    return;
                else
                    character.Position = originalPosition;
                //topright
                character._position.Y -= i;
                character._position.X += i;
                if (isCharacterInBounds(character, level)&&!isCollidingWithEverythingElse(character.getRectangle(),character,level,graphicsDevice))
                    return;
                else
                    character.Position = originalPosition;
                //bottomleft
                character._position.Y += i;
                character._position.X -= i;
                if (isCharacterInBounds(character, level)&&!isCollidingWithEverythingElse(character.getRectangle(),character,level,graphicsDevice))
                    return;
                else
                    character.Position = originalPosition;
                //bottomright
                character._position.Y += i;
                character._position.X += i;
                if (isCharacterInBounds(character, level)&&!isCollidingWithEverythingElse(character.getRectangle(),character,level,graphicsDevice))
                    return;
                else
                    character.Position = originalPosition;

            }
        }

        public static Character.Directions checkIfOneOfDoubleDirectionsIsOk(Character.Directions currentDirection, Character character, Level level, GraphicsDevice graphicsDevice)
        {
            if (currentDirection == Character.Directions.TopLeft)
            {
                Character.Directions top = Character.Directions.Top;
                Character.Directions left = Character.Directions.Left;

                if (!checkCollisionInGivenDirection(top, character, level, graphicsDevice))
                    return top;
                else if (!checkCollisionInGivenDirection(left, character, level, graphicsDevice))
                    return left;
                else
                    return Character.Directions.None;
            }

            if (currentDirection == Character.Directions.TopRight)
            {
                Character.Directions top = Character.Directions.Top;
                Character.Directions right = Character.Directions.Right;

                if (!checkCollisionInGivenDirection(top, character, level, graphicsDevice))
                    return top;
                else if (!checkCollisionInGivenDirection(right, character, level, graphicsDevice))
                    return right;
                else
                    return Character.Directions.None;
            }

            if (currentDirection == Character.Directions.BottomLeft)
            {
                Character.Directions bottom = Character.Directions.Bottom;
                Character.Directions left = Character.Directions.Left;

                if (!checkCollisionInGivenDirection(bottom, character, level, graphicsDevice))
                    return bottom;
                else if (!checkCollisionInGivenDirection(left, character, level, graphicsDevice))
                    return left;
                else
                    return Character.Directions.None;
            }

            if (currentDirection == Character.Directions.BottomRight)
            {
                Character.Directions bottom = Character.Directions.Bottom;
                Character.Directions right = Character.Directions.Right;

                if (!checkCollisionInGivenDirection(bottom, character, level, graphicsDevice))
                    return bottom;
                else if (!checkCollisionInGivenDirection(right, character, level, graphicsDevice))
                    return right;
                else
                    return Character.Directions.None;
            }
            return Character.Directions.None;
        }

        public static bool checkCollisionInGivenDirection(Character.Directions currentDirection,Character character, Level level, GraphicsDevice graphicsDevice)
        {
            if (currentDirection == Character.Directions.Top)
            {
                Rectangle characterRectangle = character.getRectangle();
                characterRectangle.Y -= (int)character.Speed;
                if (isTopLeftCornerInBounds(characterRectangle, level) && isTopRightCornerInBounds(characterRectangle, level))
                    if (!isCollidingWithEverythingElse(characterRectangle, character, level, graphicsDevice))
                        return false;
                    else
                        return true;

                else
                    return true;
            }
                
            if (currentDirection == Character.Directions.Bottom)
            {
                Rectangle characterRectangle = character.getRectangle();
                characterRectangle.Y += (int)character.Speed;
                if (isBottomLeftCornerInBounds(characterRectangle, level) && isBottomRightCornerInBounds(characterRectangle, level))
                    if (!isCollidingWithEverythingElse(characterRectangle, character, level, graphicsDevice))
                        return false;
                    else
                        return true;
                else
                    return true;
            }

            if (currentDirection == Character.Directions.Left)
            {
                Rectangle characterRectangle = character.getRectangle();
                characterRectangle.X -= (int)character.Speed;
                if (isTopLeftCornerInBounds(characterRectangle, level) && isBottomLeftCornerInBounds(characterRectangle, level))
                    if (!isCollidingWithEverythingElse(characterRectangle, character, level, graphicsDevice))
                        return false;
                    else
                        return true;
                else
                    return true;
            }
 
            if (currentDirection == Character.Directions.Right)
            {
                Rectangle characterRectangle = character.getRectangle();
                characterRectangle.X += (int)character.Speed;
                if (isTopRightCornerInBounds(characterRectangle, level) && isBottomRightCornerInBounds(characterRectangle, level))
                    if (!isCollidingWithEverythingElse(characterRectangle, character, level, graphicsDevice))
                        return false;
                    else
                        return true;
                else
                    return true;
            }

            if (currentDirection == Character.Directions.TopLeft)
            {
                Rectangle characterRectangle = character.getRectangle();
                characterRectangle.Y -= (int)character.Speed;
                characterRectangle.X -= (int)character.Speed;

                if (isTopLeftCornerInBounds(characterRectangle, level) && isBottomLeftCornerInBounds(characterRectangle, level) && isTopRightCornerInBounds(characterRectangle, level))
                    if (!isCollidingWithEverythingElse(characterRectangle, character, level, graphicsDevice))
                        return false;
                    else
                        return true;
                else
                    return true;
            }

            if (currentDirection == Character.Directions.TopRight)
            {
                Rectangle characterRectangle = character.getRectangle();
                characterRectangle.Y -= (int)character.Speed;
                characterRectangle.X += (int)character.Speed;

                if (isTopRightCornerInBounds(characterRectangle, level) && isTopLeftCornerInBounds(characterRectangle, level) && isBottomRightCornerInBounds(characterRectangle, level))
                    if (!isCollidingWithEverythingElse(characterRectangle, character, level, graphicsDevice))
                        return false;
                    else
                        return true;
                else
                    return true;
            }

            if (currentDirection == Character.Directions.BottomLeft)
            {
                Rectangle characterRectangle = character.getRectangle();
                characterRectangle.Y += (int)character.Speed;
                characterRectangle.X -= (int)character.Speed;

                if (isBottomLeftCornerInBounds(characterRectangle, level) && isTopLeftCornerInBounds(characterRectangle, level) && isBottomRightCornerInBounds(characterRectangle, level))
                    if (!isCollidingWithEverythingElse(characterRectangle, character, level, graphicsDevice))
                        return false;
                    else
                        return true;
                else
                    return true;
            }
            if (currentDirection == Character.Directions.BottomRight)
            {
                Rectangle characterRectangle = character.getRectangle();
                characterRectangle.Y += (int)character.Speed;
                characterRectangle.X += (int)character.Speed;

                if (isBottomRightCornerInBounds(characterRectangle, level) && isBottomLeftCornerInBounds(characterRectangle, level) && isTopRightCornerInBounds(characterRectangle, level))
                    if (!isCollidingWithEverythingElse(characterRectangle, character, level, graphicsDevice))
                        return false;
                    else
                        return true;
                else
                    return true;
            }
            return false;
        }

        public static bool isCollidingWithEverythingElse(Rectangle characterRectangle, Character character, Level level, GraphicsDevice graphicsDevice)
        {
            if (isCollidingWithPlayer(characterRectangle, character, level, graphicsDevice))
                return true;

            if (isCollidingWithEnemies(characterRectangle, character, level, graphicsDevice))
                return true;

            if (isCollidingWithRocks(characterRectangle, character, level, graphicsDevice))
                return true;

            return false;
        }

        public static bool isCollidingWithPlayer(Rectangle characterRectangle, Character character, Level level, GraphicsDevice graphicsDevice)
        {
            if (character.GetType() != typeof(Player))
            {
                if (Vector2.Distance(characterRectangle.Center.ToVector2(), level.player.Center) < level.cellSize)
                    if (checkCollision(characterRectangle, character, level.player, graphicsDevice))
                        return true;
            }
            return false;
        }

        public static bool isCollidingWithEnemies(Rectangle characterRectangle, Character character, Level level, GraphicsDevice graphicsDevice)
        {
            foreach (Enemy enemy in level.enemies)
            {
                if (enemy==character) continue;
                if (Vector2.Distance(characterRectangle.Center.ToVector2(), enemy.Center) < level.cellSize)
                {
                    if (checkCollision(characterRectangle, character, enemy, graphicsDevice))
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

        public static bool checkCollision(Rectangle characterRectangle, Character character1, Character character2, GraphicsDevice graphicsDevice)
        {
            if (character1 == null || character2 == null) return false;
            Rectangle character1Rectangle = characterRectangle;
            Color[] character1TextureData = character1.getCurrentTextureData(graphicsDevice);

            Rectangle character2Rectangle = character2.getRectangle();
            Color[] character2TextureData = character2.getCurrentTextureData(graphicsDevice);

            if (character1Rectangle.Intersects(character2Rectangle))
            {
                if (IntersectPixels(character1Rectangle, character1TextureData,character2Rectangle, character2TextureData))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool checkCollision(Rectangle character1Rectangle, Character character,Sprite sprite,GraphicsDevice graphicsDevice)
        {
            if (character == null || sprite == null) return false;
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

        public static bool checkCollision(Sprite sprite1, Sprite sprite2, GraphicsDevice graphicsDevice)
        {
            if (sprite1 == null || sprite2 == null) return false;

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
