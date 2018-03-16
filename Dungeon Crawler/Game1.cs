using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dungeon_Crawler
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        Texture2D rectTexture;
        Texture2D circleTexture;
        Circle circle;
        Rectangle rectangle;
        bool IsCoinTaken;

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
            rectangle = new Rectangle(300, 140, 60, 60);
            circle = new Circle(180, 180, 20);
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

            Color[] data = new Color[60 * 60];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Green;
            rectTexture.SetData(data);

            circleTexture = Content.Load<Texture2D>("coin");
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
                if (IsCoinTaken == true || !circle.Intersects(new Rectangle(rectangle.X + 1, rectangle.Y, rectangle.Width, rectangle.Height)))
                {
                    rectangle.X += 1;
                }
                else
                {
                    IsCoinTaken = true;
                    circle = null;
                }
            }

            if (state.IsKeyDown(Keys.Left))
            {
                if (IsCoinTaken == true || !circle.Intersects(new Rectangle(rectangle.X - 1, rectangle.Y, rectangle.Width, rectangle.Height)) )
                {
                    rectangle.X -= 1;
                }
                else
                {
                    IsCoinTaken = true;
                    circle = null;
                }
            }

            if (state.IsKeyDown(Keys.Up))
            {
                if (IsCoinTaken == true ||  !circle.Intersects(new Rectangle(rectangle.X, rectangle.Y - 1, rectangle.Width, rectangle.Height)))
                {
                    rectangle.Y -= 1;
                }
                else
                {
                    IsCoinTaken = true;
                    circle = null;
                }
            }

            if (state.IsKeyDown(Keys.Down))
            {
                if (IsCoinTaken == true || !circle.Intersects(new Rectangle(rectangle.X, rectangle.Y + 1, rectangle.Width, rectangle.Height)))
                {
                    rectangle.Y += 1;
                }
                else
                {
                    IsCoinTaken = true;
                    circle = null;
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

            spriteBatch.Draw(rectTexture, rectangle, Color.White);

            if (!IsCoinTaken)
            {
                spriteBatch.Draw(circleTexture, circle.Bounds(), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}