using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace BlurShader
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private SpriteFont spriteFont;
        private Texture2D texture2D;
        private List<Texture2D> textures;
        private Effect blurEffect;
        private float value = 0.001f;


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
            textures = new List<Texture2D>();
            graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
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
            textures.Add(Content.Load<Texture2D>("pic1"));
            textures.Add(Content.Load<Texture2D>("pic2"));
            blurEffect = Content.Load<Effect>("blur");
            spriteFont = Content.Load<SpriteFont>("font");
            texture2D = textures[0];
            // TODO: use this.Content to load your game content here
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
            
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                value += 0.000005f;

            else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                value -= 0.000005f;

            if (Keyboard.GetState().IsKeyDown(Keys.NumPad1))
                texture2D = textures[0];

            if (Keyboard.GetState().IsKeyDown(Keys.NumPad2))
                texture2D = textures[1];


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

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            blurEffect.Parameters["param"].SetValue(value);
            blurEffect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(texture2D, new Vector2((GraphicsDevice.DisplayMode.Width/2)-texture2D.Width/2, (GraphicsDevice.DisplayMode.Height / 2) - texture2D.Height/2), Color.White);

            spriteBatch.End();

            spriteBatch.Begin();

            spriteBatch.DrawString(spriteFont, "Klawisz do gory zwieksza rozmycie", new Vector2(10, 70), Color.Black);
            spriteBatch.DrawString(spriteFont, "Klawisz w dol zmniejsza rozmycie", new Vector2(10, 40), Color.Black);
            spriteBatch.DrawString(spriteFont, "Wcisnij klawisz 1 lub 2 na NumPadzie by zmienic obraz", new Vector2(10, 100), Color.Black);
            spriteBatch.DrawString(spriteFont, "Rozmycie wynosi " + value, new Vector2(GraphicsDevice.DisplayMode.Width / 2, 20), Color.Black);
            spriteBatch.DrawString(spriteFont, "Klawisz ESC zamyka program", new Vector2(10, 10), Color.Black);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
