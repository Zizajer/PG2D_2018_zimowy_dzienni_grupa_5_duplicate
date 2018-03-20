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
        private GoalMap _goalMap;
        private Player _player;
        private List<AggressiveEnemy> _aggressiveEnemies = new List<AggressiveEnemy>();
        private int los = 8;

        private InputManager _inputState;

        public Main()
           : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

        }

        protected override void Initialize()
        {
            IMapCreationStrategy<Map> mapCreationStrategy = new RandomRoomsMapCreationStrategy<Map>(GlobalVariables.MapWidth, GlobalVariables.MapHeight, 100, 2, 2);
            _map = Map.Create(mapCreationStrategy);
            _goalMap = new GoalMap(_map);
            _inputState = new InputManager();
            GlobalVariables.Camera.ViewportWidth = graphics.GraphicsDevice.Viewport.Width;
            GlobalVariables.Camera.ViewportHeight = graphics.GraphicsDevice.Viewport.Height;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _floor = Content.Load<Texture2D>("Wood");
            _wall = Content.Load<Texture2D>("Brick");
            

            Cell startingCell = GetRandomEmptyCell();
            GlobalVariables.Camera.CenterOn(startingCell);
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
            GlobalVariables.Gui = new GUI(_player,Content.Load<SpriteFont>("spritefont"));
            GlobalVariables.CombatManager = new CombatManager(_player, _aggressiveEnemies);
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
                if (!GlobalVariables.DebugMode)
                {
                    GlobalVariables.DebugMode = true;
                    Debug.WriteLine("DebugMode on");
                }
                else if (GlobalVariables.DebugMode)
                {
                    GlobalVariables.DebugMode = false;
                    Debug.WriteLine("DebugMode off");
                }
            }
            else
            {
                if ( _player.HandleInput(_inputState, _map))
                {
                    UpdatePlayerFieldOfView();
                    GlobalVariables.Camera.CenterOn(_map.GetCell(_player.X, _player.Y));
                }
                foreach (var enemy in _aggressiveEnemies)
                {
                    enemy.Update(gameTime);
                }
            }
            GlobalVariables.Gui.Update();
            GlobalVariables.Camera.HandleInput(_inputState, PlayerIndex.One);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, GlobalVariables.Camera.TranslationMatrix);

            foreach (Cell cell in _map.GetAllCells())
            {
                var position = new Vector2(cell.X * GlobalVariables.SpriteWidth, cell.Y * GlobalVariables.SpriteHeight);
                if (!cell.IsExplored &&  !GlobalVariables.DebugMode)
                {
                    continue;
                }
                Color tint = Color.White;
                if (!cell.IsInFov && !GlobalVariables.DebugMode)
                {
                    tint = Color.Gray;
                }
                if (cell.IsWalkable)
                {
                    spriteBatch.Draw(_floor, position, null, tint, 0.0f, Vector2.One, 1.0f, SpriteEffects.None, Layers.Cells);
                }
                else
                {
                    spriteBatch.Draw(_wall, position, null, tint, 0.0f, Vector2.One, 1.0f, SpriteEffects.None, Layers.Cells);
                }
            }
            foreach (var enemy in _aggressiveEnemies)
            {
                if (GlobalVariables.DebugMode || _map.IsInFov(enemy.X, enemy.Y))
                {
                    enemy.Draw(spriteBatch);
                }
            }
            GlobalVariables.Gui.Draw(spriteBatch);
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
                int x = GlobalVariables.Random.Next(GlobalVariables.MapWidth-1);
                int y = GlobalVariables.Random.Next(GlobalVariables.MapHeight-1);
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
                pathFromAggressiveEnemy.CreateFrom(enemyCell.X, enemyCell.Y, _goalMap);
                float speed = (float)(GlobalVariables.Random.Next(5) + 8 )/ 10;
                var enemy = new AggressiveEnemy(_map,_goalMap, pathFromAggressiveEnemy, speed)
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
