using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using RogueSharp.MapCreation;
using RogueSharp.Random;
using System.Collections.Generic;
using System.Diagnostics;

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
        private List<AggressiveEnemy> _aggressiveEnemies = new List<AggressiveEnemy>();
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
            IMapCreationStrategy<Map> mapCreationStrategy = new RandomRoomsMapCreationStrategy<Map>(Global.MapWidth, Global.MapHeight, 100, 5, 5);
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
                Sprite = Content.Load<Texture2D>("Player"),
                Damage = 15,
                Health = 100,
                Name = "Player"
            };
            AddAggressiveEnemies(10);
            UpdatePlayerFieldOfView();
            Global.gui = new GUI(_player,Content.Load<SpriteFont>("spritefont"));
            Global.CombatManager = new CombatManager(_player, _aggressiveEnemies);
            Global.GameState = GameStates.PlayerTurn;
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            _inputState.Update();
            if (_inputState.IsExitGame(PlayerIndex.One))
            {
                Exit();
            }
            else if (_inputState.IsSpace(PlayerIndex.One))
            {
                if (Global.GameState == GameStates.PlayerTurn && !Global.DebugMode)
                {
                    Global.DebugMode = true;
                    Debug.WriteLine("DebugMode on");
                }
                else if (Global.GameState == GameStates.PlayerTurn && Global.DebugMode)
                {
                    Global.DebugMode = false;
                    Debug.WriteLine("DebugMode off");
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
                    foreach (var enemy in _aggressiveEnemies)
                    {
                        enemy.Update();
                    }
                    Global.GameState = GameStates.PlayerTurn;
                }
            }
            Global.gui.Update();
            Global.Camera.HandleInput(_inputState, PlayerIndex.One);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);

            foreach (Cell cell in _map.GetAllCells())
            {
                var position = new Vector2(cell.X * Global.SpriteWidth, cell.Y * Global.SpriteHeight);
                if (!cell.IsExplored &&  !Global.DebugMode)
                {
                    continue;
                }
                Color tint = Color.White;
                if (!cell.IsInFov && !Global.DebugMode)
                {
                    tint = Color.Gray;
                }
                if (cell.IsWalkable)
                {
                    spriteBatch.Draw(_floor, position, null, tint, 0.0f, Vector2.One, 1.0f, SpriteEffects.None, LayerDepth.Cells);
                }
                else
                {
                    spriteBatch.Draw(_wall, position, null, tint, 0.0f, Vector2.One, 1.0f, SpriteEffects.None, LayerDepth.Cells);
                }
            }
            foreach (var enemy in _aggressiveEnemies)
            {
                if (Global.DebugMode || _map.IsInFov(enemy.X, enemy.Y))
                {
                    enemy.Draw(spriteBatch);
                }
            }
            Global.gui.Draw(spriteBatch);
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
        private void AddAggressiveEnemies(int numberOfEnemies)
        {
            for (int i = 0; i < numberOfEnemies; i++)
            {
                Cell enemyCell = GetRandomEmptyCell();
                var pathFromAggressiveEnemy =
                  new PathToPlayer(_player, _map, Content.Load<Texture2D>("White"));
                pathFromAggressiveEnemy.CreateFrom(enemyCell.X, enemyCell.Y);
                var enemy = new AggressiveEnemy(_map, pathFromAggressiveEnemy)
                {
                    X = enemyCell.X,
                    Y = enemyCell.Y,
                    Sprite = Content.Load<Texture2D>("Hound"),
                    Damage = 8,
                    Health = 15,
                    Name = "Aggresive Hound"
                };

                _aggressiveEnemies.Add(enemy);
            }
        }
    }
}
