using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using RogueSharp.MapCreation;
using RogueSharp.Random;

namespace Dungeon_Crawler
{
    public class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Texture2D _floor;
        private Texture2D _wall;
        private IMap _map;
        private Player _player;
        private float scale = 0.25f;
        private int los = 30;
        private int mapWidth = 50;
        private int mapHeight = 30;

        private InputState _inputState;

        public Main()
           : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IMapCreationStrategy<Map> mapCreationStrategy = new RandomRoomsMapCreationStrategy<Map>(mapWidth, mapHeight, 100, 10, 5);
            _map = Map.Create(mapCreationStrategy);
            _inputState = new InputState();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _floor = Content.Load<Texture2D>("Floor");
            _wall = Content.Load<Texture2D>("Wall");
            Cell startingCell = GetRandomEmptyCell();
            _player = new Player
            {
                X = startingCell.X,
                Y = startingCell.Y,
                Scale = scale,
                Sprite = Content.Load<Texture2D>("Player")
            };
            UpdatePlayerFieldOfView();
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (_inputState.IsExitGame(PlayerIndex.One))
            {
                Exit();
            }
            else
            {
                if (_player.HandleInput(_inputState, _map))
                {
                    UpdatePlayerFieldOfView();
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            int sizeOfSprites = 64;
            foreach (Cell cell in _map.GetAllCells())
            {
                var position = new Vector2(cell.X * sizeOfSprites * scale, cell.Y * sizeOfSprites * scale);
                if (!cell.IsExplored)
                {
                    continue;
                }
                Color tint = Color.White;
                if (!cell.IsInFov)
                {
                    tint = Color.Gray;
                }
                if (cell.IsWalkable)
                {
                    spriteBatch.Draw(_floor, position, null, null, null,
                       0.0f, new Vector2(scale, scale), tint, SpriteEffects.None, 0.8f);
                }
                else
                {
                    spriteBatch.Draw(_wall, position, null, null, null,
                       0.0f, new Vector2(scale, scale), tint, SpriteEffects.None, 0.8f);
                }
            }

            _player.Draw(spriteBatch);
            _inputState.Update();
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private Cell GetRandomEmptyCell()
        {
            IRandom random = new DotNetRandom();

            while (true)
            {
                int x = random.Next(mapWidth-1);
                int y = random.Next(mapHeight-1);
                if (_map.IsWalkable(x, y))
                {
                    return _map.GetCell(x, y);
                }
            }
        }
        private void UpdatePlayerFieldOfView()
        {
            _map.ComputeFov(_player.X, _player.Y, los, true);
            foreach (Cell cell in _map.GetAllCells())
            {
                if (_map.IsInFov(cell.X, cell.Y))
                {
                    _map.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                }
            }
        }
    }
}
