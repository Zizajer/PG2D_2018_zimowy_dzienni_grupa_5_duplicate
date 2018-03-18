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

        Player Player;
        int speed;
        Rectangle Block;
        Texture2D RectangleTexture;
        Color rotatedRectangleColor;
        SpriteFont spriteFont;

        bool isCollision = false;

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

            speed = 2;
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
            this.spriteFont = Content.Load<SpriteFont>("spritefont");

            RectangleTexture = Content.Load<Texture2D>("Block");

            Player = new Player(new Rectangle(100, 200, RectangleTexture.Width*3, RectangleTexture.Height*2), 0.0f);
            Block = new Rectangle(400, 200, RectangleTexture.Width * 3, RectangleTexture.Height * 4);

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

            Move();
            isCollision = Player.Intersects(Block);

            base.Update(gameTime);
        }
       
        private void Move()
        {

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                Player.CollisionRectangle.Y -= speed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                Player.CollisionRectangle.Y += speed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Player.CollisionRectangle.X -= speed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Player.CollisionRectangle.X += speed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Player.Rotation += 0.01f;
            }
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (isCollision)
            {
                rotatedRectangleColor = Color.Yellow;
            }
            else rotatedRectangleColor = Color.Blue;

            spriteBatch.Begin();

            spriteBatch.DrawString(spriteFont, "Space - Rotate ", new Vector2(20, 20), Color.Black);
            spriteBatch.DrawString(spriteFont, "Arrow Keys - Movement ", new Vector2(20, 40), Color.Black);


            spriteBatch.Draw(RectangleTexture, Block, Color.Blue);

            Rectangle positionAdjustedRectangle = new Rectangle(Player.CollisionRectangle.X + (Player.CollisionRectangle.Width / 2), Player.CollisionRectangle.Y + (Player.CollisionRectangle.Height / 2), Player.CollisionRectangle.Width, Player.CollisionRectangle.Height);
            spriteBatch.Draw(RectangleTexture, positionAdjustedRectangle, new Rectangle(0, 0, 2, 6), rotatedRectangleColor, Player.Rotation, new Vector2(2 / 2, 6 / 2), SpriteEffects.None, 0);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
