﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RogueSharp;

namespace Dungeon_Crawler
{
    public class CameraManager
    {
        public int mapWidth, mapHeight, cellSize;
        // Centered Position of the Camera in pixels.
        public Vector2 Position { get; private set; }
        // Current Zoom level with 1.0f being standard
        public float Zoom { get; private set; }
        // Current Rotation amount with 0.0f being standard orientation
        public float Rotation { get; private set; }

        // Height and width of the viewport window which we need to adjust
        // any time the player resizes the game window.
        public int ViewportWidth { get; set; }
        public int ViewportHeight { get; set; }

        //custon constructor
        public CameraManager()
        {
            Zoom = 1.5f;
        }
        // Construct a new Camera class with standard zoom (no scaling)
        public CameraManager(int ViewportWidth, int ViewportHeight)
        {
            Zoom = 1.5f;
            this.ViewportWidth = ViewportWidth;
            this.ViewportHeight = ViewportHeight;
        }

        public void setZoom(float f)
        {
            Zoom = f;
        }

        public void setViewports(int ViewportWidth, int ViewportHeight)
        {
            this.ViewportWidth = ViewportWidth;
            this.ViewportHeight = ViewportHeight;
        }
        // Center of the Viewport which does not account for scale
        public Vector2 ViewportCenter
        {
            get
            {
                return new Vector2(ViewportWidth * 0.5f, ViewportHeight * 0.5f);
            }
        }

        public void setParams(int mapWidth, int mapHeight, int cellSize)
        {
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            this.cellSize = cellSize;
    }
        // Create a matrix for the camera to offset everything we draw,
        // the map and our objects. since the camera coordinates are where
        // the camera is, we offset everything by the negative of that to simulate
        // a camera moving. We also cast to integers to avoid filtering artifacts.
        public Matrix TranslationMatrix
        {
            get
            {
                return Matrix.CreateTranslation(-(int)Position.X,
                   -(int)Position.Y, 0) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                   Matrix.CreateTranslation(new Vector3(ViewportCenter, 0));
            }
        }

        // Call this method with negative values to zoom out
        // or positive values to zoom in. It looks at the current zoom
        // and adjusts it by the specified amount. If we were at a 1.0f
        // zoom level and specified -0.5f amount it would leave us with
        // 1.0f - 0.5f = 0.5f so everything would be drawn at half size.
        public void AdjustZoom(float amount)
        {
            Zoom += amount;
            if (Zoom < 1.50f)
            {
                Zoom = 1.50f;
            }
            if (Zoom > 3.00f)
            {
                Zoom = 3.00f;
            }
        }

        // Move the camera in an X and Y amount based on the cameraMovement param.
        // if clampToMap is true the camera will try not to pan outside of the
        // bounds of the map.
        public void MoveCamera(Vector2 cameraMovement, bool clampToMap = false)
        {
            Vector2 newPosition = Position + cameraMovement;

            if (clampToMap)
            {
                Position = MapClampedPosition(newPosition);
            }
            else
            {
                Position = newPosition;
            }
        }

        public Microsoft.Xna.Framework.Rectangle ViewportWorldBoundry()
        {
            Vector2 viewPortCorner = ScreenToWorld(new Vector2(0, 0));
            Vector2 viewPortBottomCorner =
               ScreenToWorld(new Vector2(ViewportWidth, ViewportHeight));

            return new Microsoft.Xna.Framework.Rectangle((int)viewPortCorner.X,
               (int)viewPortCorner.Y,
               (int)(viewPortBottomCorner.X - viewPortCorner.X),
               (int)(viewPortBottomCorner.Y - viewPortCorner.Y));
        }

        // Center the camera on specific pixel coordinates
        public void CenterOn(Vector2 position)
        {
            Position = MapClampedPosition(position);
        }
        // Center the camera on a specific cell in the map
        public void CenterOn(Cell cell)
        {
            Position = CenteredPosition(cell, true);
        }

        private Vector2 CenteredPosition(Cell cell, bool clampToMap = false)
        {
            var cameraPosition = new Vector2(cell.X * cellSize,
               cell.Y * cellSize);
            var cameraCenteredOnTilePosition =
               new Vector2(cameraPosition.X + cellSize / 2,
                   cameraPosition.Y + cellSize / 2);
            if (clampToMap)
            {
                return MapClampedPosition(cameraCenteredOnTilePosition);
            }

            return cameraCenteredOnTilePosition;
        }
        // Clamp the camera so it never leaves the visible area of the map.
        private Vector2 MapClampedPosition(Vector2 position)
        {
            var cameraMax = new Vector2(mapWidth * cellSize -
                (ViewportWidth / Zoom / 2),
                mapHeight * cellSize -
                (ViewportHeight / Zoom / 2));

            return Vector2.Clamp(position,
               new Vector2(ViewportWidth / Zoom / 2, ViewportHeight / Zoom / 2),
               cameraMax);
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, TranslationMatrix);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition,
                Matrix.Invert(TranslationMatrix));
        }

        // Move the camera's position based on input
        public virtual void Move()
        {
            Vector2 cameraMovement = Vector2.Zero;

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                cameraMovement.X = -1;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                cameraMovement.X = 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                cameraMovement.Y = -1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                cameraMovement.Y = 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemComma))
            {
                AdjustZoom(0.05f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemPeriod))
            {
                AdjustZoom(-0.05f);
            }

            // When using a controller, to match the thumbstick behavior,
            // we need to normalize non-zero vectors in case the user
            // is pressing a diagonal direction.
            if (cameraMovement != Vector2.Zero)
            {
                cameraMovement.Normalize();
            }

            // scale our movement to move 25 pixels per second
            cameraMovement *= 25f;

            MoveCamera(cameraMovement, true);
        }
    }
}
