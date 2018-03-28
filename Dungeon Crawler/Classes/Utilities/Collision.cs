using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dungeon_Crawler
{
    public static class Collision
    {
        public static bool checkCollision(Character character1, Character character2, GraphicsDevice graphicsDevice)
        {
            Rectangle character1Rectangle = character1.getRectangle();
            Color[] character1TextureData = character1.getCurrentTextureData(graphicsDevice);

            Rectangle character2Rectangle = character2.getRectangle();
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
            Color[] characterTextureData = character.getCurrentTextureData(graphicsDevice);

            Rectangle characterRectangle = character.getRectangle();
             
            Rectangle obstacleRectangle = obstacle.getRectangle();

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
