using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dungeon_Crawler
{
    // Source: http://www.xnadevelopment.com/tutorials/rotatedrectanglecollisions/rotatedrectanglecollisions.shtml
    // Algorithm bases on Separating Axes Theorem (SAT).
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        RotatedRectangle RectangleA;
        RotatedRectangle RectangleB;

        Texture2D RectangleTexture;

        bool IsRectangleASelected = true;

        KeyboardState PreviousKeyboardState;

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

            RectangleTexture = Content.Load<Texture2D>("Square");

            RectangleA = new RotatedRectangle(new Rectangle(100, 200, 70, 70), 0.0f);
            RectangleB = new RotatedRectangle(new Rectangle(400, 150, 130, 200), 45.0f);

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


            SwitchActiveRectangle();
            MoveRectangle();

            base.Update(gameTime);
        }


        private void SwitchActiveRectangle()
        {
            KeyboardState aCurrentKeyboardState = Keyboard.GetState();
            if (aCurrentKeyboardState.IsKeyDown(Keys.Tab) && PreviousKeyboardState.IsKeyDown(Keys.Tab) == false)
            {
                IsRectangleASelected = !IsRectangleASelected;
            }
            PreviousKeyboardState = aCurrentKeyboardState;
        }

        private void MoveRectangle()
        {
            KeyboardState CurrentKeyboard = Keyboard.GetState();
            RotatedRectangle RectangleToMove;
            RotatedRectangle AnotherRectangle;

            if (IsRectangleASelected)
            {
                RectangleToMove = RectangleA;
                AnotherRectangle = RectangleB;
            }
            else
            {
                RectangleToMove = RectangleB;
                AnotherRectangle = RectangleA;
            }

            if (CurrentKeyboard.IsKeyDown(Keys.Up) && !AnotherRectangle.Intersects(new RotatedRectangle(new Rectangle(RectangleToMove.X, RectangleToMove.Y - 2, RectangleToMove.Width, RectangleToMove.Height), RectangleToMove.Rotation)))
            {
                RectangleToMove.ChangePosition(0, -2);
            }

            if (CurrentKeyboard.IsKeyDown(Keys.Down) && !AnotherRectangle.Intersects(new RotatedRectangle(new Rectangle(RectangleToMove.X, RectangleToMove.Y + 2, RectangleToMove.Width, RectangleToMove.Height), RectangleToMove.Rotation)))
            {
                RectangleToMove.ChangePosition(0, 2);
            }

            if (CurrentKeyboard.IsKeyDown(Keys.Left) && !AnotherRectangle.Intersects(new RotatedRectangle(new Rectangle(RectangleToMove.X - 2, RectangleToMove.Y, RectangleToMove.Width, RectangleToMove.Height), RectangleToMove.Rotation)))
            {
                RectangleToMove.ChangePosition(-2, 0);
            }

            if (CurrentKeyboard.IsKeyDown(Keys.Right) && !AnotherRectangle.Intersects(new RotatedRectangle(new Rectangle(RectangleToMove.X + 2, RectangleToMove.Y, RectangleToMove.Width, RectangleToMove.Height), RectangleToMove.Rotation)))
            {
                RectangleToMove.ChangePosition(2, 0);
            }

            if (CurrentKeyboard.IsKeyDown(Keys.Space) && !AnotherRectangle.Intersects(new RotatedRectangle(new Rectangle(RectangleToMove.X, RectangleToMove.Y, RectangleToMove.Width, RectangleToMove.Height), RectangleToMove.Rotation)))
            {
                RectangleToMove.Rotation += 0.01f;
            }

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Color aColor = Color.Blue;
            Color bColor = Color.Yellow;

            spriteBatch.Begin();

            Rectangle aPositionAdjusted = new Rectangle(RectangleA.X + (RectangleA.Width / 2), RectangleA.Y + (RectangleA.Height / 2), RectangleA.Width, RectangleA.Height);
            spriteBatch.Draw(RectangleTexture, aPositionAdjusted, new Rectangle(0, 0, 2, 6), aColor, RectangleA.Rotation, new Vector2(2 / 2, 6 / 2), SpriteEffects.None, 0);

            aPositionAdjusted = new Rectangle(RectangleB.X + (RectangleB.Width / 2), RectangleB.Y + (RectangleB.Height / 2), RectangleB.Width, RectangleB.Height);
            spriteBatch.Draw(RectangleTexture, aPositionAdjusted, new Rectangle(0, 0, 2, 6), bColor, RectangleB.Rotation, new Vector2(2 / 2, 6 / 2), SpriteEffects.None, 0);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
