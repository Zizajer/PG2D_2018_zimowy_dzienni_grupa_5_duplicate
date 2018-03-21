﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Game1
{
    public static class Collision
    {

        public static bool checkCollision(Player player,Obstacle obstacle,GraphicsDevice graphicsDevice)
        {
            Texture2D multipleSpriteTexture = player._animationManager._animation.Texture;
            Rectangle toCropRectangle = player._animationManager.getCurrentFrameRectangle();

            Texture2D singleTexture = new Texture2D(graphicsDevice, toCropRectangle.Width, toCropRectangle.Height);
            Color[] singleTextureData = new Color[toCropRectangle.Width * toCropRectangle.Height];
            multipleSpriteTexture.GetData(0, toCropRectangle, singleTextureData, 0, singleTextureData.Length);
            singleTexture.SetData(singleTextureData);

            Rectangle playerRectangle = player.getRectangle();
             
            Rectangle obstacleRectangle = obstacle.getRectangle();

            if (playerRectangle.Intersects(obstacleRectangle))
            {
                if (Collision.IntersectPixels(playerRectangle, singleTextureData,
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
