using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dungeon_Crawler
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D rectTexture;

        Rectangle rectangle;
        SpriteFont spriteFont;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            rectangle = new Rectangle(300, 140, 60, 60);

        }

        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);

            this.spriteFont = Content.Load<SpriteFont>("spritefont");

            rectTexture = new Texture2D(graphics.GraphicsDevice, 60, 60);

            Color[] data = new Color[60 * 60];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Green;
            rectTexture.SetData(data);

        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
                
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Left))
            {
                rectangle.X -= 1;

            }
            if (state.IsKeyDown(Keys.Right))
            {
                rectangle.X += 1;

            }
            if (state.IsKeyDown(Keys.Up))
            {
                rectangle.Y -= 1;
            }
            if (state.IsKeyDown(Keys.Down))
            {
                rectangle.Y += 1;

            }
                
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(rectTexture, rectangle, Color.White);

            spriteBatch.DrawString(spriteFont, "X= "+rectangle.X+" Y= "+rectangle.Y, new Vector2(300, 20), Color.Black);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}