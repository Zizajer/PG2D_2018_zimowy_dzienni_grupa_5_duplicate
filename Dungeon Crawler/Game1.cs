using Dungeon_Crawler.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Dungeon_Crawler
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private List<Sprite> sprites;
        private Circle circle;

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
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            var playerTexture = Content.Load<Texture2D>("Block");
            var circleTexture = Content.Load<Texture2D>("SmallCircle");


            circle = new Circle(circleTexture)
            {
                center = new Vector2(200, 100),
                color = Color.Yellow,
                radius = 16,
            };

            sprites = new List<Sprite>()
            {
                new Player(playerTexture)
                {
                    input = new Models.Input()
                    {
                        left = Keys.A,
                        right = Keys.D,
                        up = Keys.W,
                        down = Keys.S,

                    },
                    position = new Vector2(300,100),
                    color = Color.Black,
                    speed = 3f,
                    circle = circle,
                },
                new Player(playerTexture)
                {
                    input = new Models.Input()
                    {
                        left = Keys.Left,
                        right = Keys.Right,
                        up = Keys.Up,
                        down = Keys.Down,

                    },
                    position = new Vector2(100,100),
                    color = Color.Red,
                    speed = 1f,
                    circle = circle,
                },
            };

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

            foreach (var sprite in sprites)
                sprite.Update(gameTime, sprites);

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            foreach (var sprite in sprites)
                sprite.Draw(spriteBatch);

            circle.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

