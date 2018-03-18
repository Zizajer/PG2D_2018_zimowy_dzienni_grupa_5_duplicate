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
        private AggressiveEnemy _aggressiveEnemy;
        private int los = 30;

        private InputState _inputState;

        public Main()
           : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

        }

        protected override void Initialize()
        {
            IMapCreationStrategy<Map> mapCreationStrategy = new RandomRoomsMapCreationStrategy<Map>(Global.MapWidth, Global.MapHeight, 100, 10, 5);
            _map = Map.Create(mapCreationStrategy);
            _inputState = new InputState();
            Global.Camera.ViewportWidth = graphics.GraphicsDevice.Viewport.Width;
            Global.Camera.ViewportHeight = graphics.GraphicsDevice.Viewport.Height;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _floor = Content.Load<Texture2D>("Floor");
            _wall = Content.Load<Texture2D>("Wall");
            Cell startingCell = GetRandomEmptyCell();
            Global.Camera.CenterOn(startingCell);
            _player = new Player
            {
                X = startingCell.X,
                Y = startingCell.Y,
                Sprite = Content.Load<Texture2D>("Player")
            };
            startingCell = GetRandomEmptyCell();
            var pathFromAggressiveEnemy = new PathToPlayer(_player, _map, Content.Load<Texture2D>("White"));
            pathFromAggressiveEnemy.CreateFrom(startingCell.X, startingCell.Y);
            _aggressiveEnemy = new AggressiveEnemy(pathFromAggressiveEnemy)
            {
                X = startingCell.X,
                Y = startingCell.Y,
                Sprite = Content.Load<Texture2D>("Hound")
            };
            UpdatePlayerFieldOfView();
            Global.GameState = GameStates.PlayerTurn;
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            _inputState.Update();
            if (_inputState.IsExitGame(PlayerIndex.One))
            {
                Exit();
            }
            else if (_inputState.IsSpace(PlayerIndex.One))
            {
                if (Global.GameState == GameStates.PlayerTurn)
                {
                    Global.GameState = GameStates.Debugging;
                }
                else if (Global.GameState == GameStates.Debugging)
                {
                    Global.GameState = GameStates.PlayerTurn;
                }
            }
            else
            {
                if (Global.GameState == GameStates.PlayerTurn
                   && _player.HandleInput(_inputState, _map))
                {
                    UpdatePlayerFieldOfView();
                    Global.Camera.CenterOn(_map.GetCell(_player.X, _player.Y));
                    Global.GameState = GameStates.EnemyTurn;
                }
                if (Global.GameState == GameStates.EnemyTurn)
                {
                    _aggressiveEnemy.Update();
                    Global.GameState = GameStates.PlayerTurn;
                }
            }
            Global.Camera.HandleInput(_inputState, PlayerIndex.One);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
    null, null, null, null, Global.Camera.TranslationMatrix);

            foreach (Cell cell in _map.GetAllCells())
            {
                var position = new Vector2(cell.X * Global.SpriteWidth, cell.Y * Global.SpriteHeight);
                if (!cell.IsExplored && Global.GameState != GameStates.Debugging)
                {
                    continue;
                }
                Color tint = Color.White;
                if (!cell.IsInFov && Global.GameState != GameStates.Debugging)
                {
                    tint = Color.Gray;
                }
                if (cell.IsWalkable)
                {
                    spriteBatch.Draw(_floor, position, null, null, null, 0.0f, Vector2.One, tint, SpriteEffects.None, LayerDepth.Cells);
                }
                else
                {
                    spriteBatch.Draw(_wall, position, null, null, null, 0.0f, Vector2.One, tint, SpriteEffects.None, LayerDepth.Cells);
                }
            }
            if (Global.GameState == GameStates.Debugging
             || _map.IsInFov(_aggressiveEnemy.X, _aggressiveEnemy.Y))
            {
                _aggressiveEnemy.Draw(spriteBatch);
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
                int x = Global.Random.Next(Global.MapWidth-1);
                int y = Global.Random.Next(Global.MapHeight-1);
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
