﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Dungeon_Crawler
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private SpriteFont font;
        private String collision = "";

        Texture2D rectTexture;
        Texture2D circleTexture;
        Circle circle;
        Rectangle rectangle;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            rectangle = new Rectangle(300, 100, 100, 100);
            circle = new Circle(500, 200, 30);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            rectTexture = new Texture2D(graphics.GraphicsDevice, 60, 60);
            font = Content.Load<SpriteFont>("Default");
            Color[] data = new Color[60 * 60];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Green;
            rectTexture.SetData(data);

            circleTexture = Content.Load<Texture2D>("circle");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Right))
            {
                rectangle.X += 2;
                if (!circle.Intersects(new Rectangle(rectangle.X + 1, rectangle.Y, rectangle.Width, rectangle.Height)))
                {   
                    collision = "";
                }
                else
                {
                    collision = "Collision";
                }
            }

            if (state.IsKeyDown(Keys.Left))
            {
                rectangle.X -= 2;
                if (!circle.Intersects(new Rectangle(rectangle.X - 1, rectangle.Y, rectangle.Width, rectangle.Height)))
                {
                    collision = "";
                }
                else
                {
                    collision = "Collision";
                }
            }

            if (state.IsKeyDown(Keys.Up))
            {
                rectangle.Y -= 2;
                if (!circle.Intersects(new Rectangle(rectangle.X, rectangle.Y - 1, rectangle.Width, rectangle.Height)))
                {
                    collision = "";
                }
                else
                {
                    collision = "Collision";
                }
            }

            if (state.IsKeyDown(Keys.Down))
            {
                rectangle.Y += 2;
                if (!circle.Intersects(new Rectangle(rectangle.X, rectangle.Y + 1, rectangle.Width, rectangle.Height)))
                {
                    collision = "";
                }
                else
                {
                    collision = "Collision";
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.DrawString(font, collision, new Vector2(100, 100), Color.Black);
            spriteBatch.Draw(rectTexture, rectangle, Color.Green);
            spriteBatch.Draw(circleTexture, circle.Bounds(), Color.Red);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}