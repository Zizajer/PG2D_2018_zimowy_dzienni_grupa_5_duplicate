using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RogueSharp;
using RogueSharp.MapCreation;

namespace Dungeon_Crawler
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D rectTexture;
        Microsoft.Xna.Framework.Rectangle rectangle;

        private IMap _map;

        private Texture2D _floor;
        private Texture2D _wall;

        SpriteFont spriteFont;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
            IMapCreationStrategy<Map> mapCreationStrategy =
            new RandomRoomsMapCreationStrategy<Map>(50, 30, 100, 7, 3);
            /*
            width = The width of the Map to be created
            height = The height of the Map to be created
            maxRooms = The maximum number of rooms that will exist in the generated Map
            roomMaxSize = The maximum width and height of each room that will be generated in the Map
            roomMinSize = The minimum width and height of each room that will be generated in the Map
            */
            _map = Map.Create(mapCreationStrategy);
            rectangle = new Microsoft.Xna.Framework.Rectangle(300, 140, 60, 60);

        }

        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);

            this.spriteFont = Content.Load<SpriteFont>("spritefont");

            rectTexture = new Texture2D(graphics.GraphicsDevice, 60, 60);

            _floor = Content.Load<Texture2D>("Floor");
            _wall = Content.Load<Texture2D>("Wall");
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

            int sizeOfSprites = 64;
            float scale = .25f;
            foreach (Cell cell in _map.GetAllCells())
            {
                var position = new Vector2(cell.X * sizeOfSprites * scale, cell.Y * sizeOfSprites * scale);
                if (cell.IsWalkable)
                {
                    spriteBatch.Draw(_floor, position, null, null, null, 0.0f, new Vector2(scale, scale), Color.White);
                }
                else
                {
                    spriteBatch.Draw(_wall, position, null, null, null, 0.0f, new Vector2(scale, scale), Color.White);
                }
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}