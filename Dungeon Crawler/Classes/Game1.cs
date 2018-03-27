using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using RogueSharp;
using RogueSharp.MapCreation;

namespace Dungeon_Crawler
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Player player;
        Level level;

        public Random random;
        public CameraManager camera;

        //gui
        private SpriteFont font;
        private String collision;
        bool areColliding;
        //gui
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            camera = new CameraManager(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            random = new Random();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //levelManager
            IMapCreationStrategy<Map> mapCreationStrategy =
                new RandomRoomsMapCreationStrategy<Map>(16, 10, 100, 3, 3);
            Map map = Map.Create(mapCreationStrategy);

            Texture2D floor = Content.Load<Texture2D>("map/Floor");
            Texture2D wall = Content.Load<Texture2D>("map/Wall");
            int cellSize = floor.Width;

            camera.setParams(map.Width, map.Height, cellSize);
            

            List<Enemy> enemies = new List<Enemy>(5);
            List<Item> items = new List<Item>(3); 
            List<Obstacle> obstacles = new List<Obstacle>(3);

            Cell randomCell = GetRandomEmptyCell(map);
            items.Add(new Item(new Vector2(randomCell.X * cellSize, randomCell.Y * cellSize), Content.Load<Texture2D>("items/bow1"), "Bow"));
            randomCell = GetRandomEmptyCell(map);
            items.Add(new Item(new Vector2(randomCell.X * cellSize, randomCell.Y * cellSize), Content.Load<Texture2D>("items/sword1"), "Sword"));
            randomCell = GetRandomEmptyCell(map);
            items.Add(new Item(new Vector2(randomCell.X * cellSize, randomCell.Y * cellSize), Content.Load<Texture2D>("items/wand1"), "Wand"));

            randomCell = GetRandomEmptyCell(map);
            camera.CenterOn(randomCell);

            player =
                new Player(this.Content,camera,cellSize)
                {
                    Position = new Vector2((randomCell.X*cellSize+cellSize/3), (randomCell.Y*cellSize)+ cellSize/3)
                };

            for (int i = 0; i < 5; i++)
            {
                randomCell = GetRandomEmptyCell(map);
                float speed = (random.Next(2) + 1) / 0.7f;
                float timeBetweenActions = (random.Next(2))+1 / 0.7f;
                Enemy tempEnemy =
                    new Enemy(this.Content, cellSize, speed, timeBetweenActions)
                    {
                        Position = new Vector2((randomCell.X * cellSize + cellSize / 3), (randomCell.Y * cellSize) + cellSize / 3)
                    };
                enemies.Add(tempEnemy);
            }

            for (int i = 0; i < 3; i++)
            {
                randomCell = GetRandomEmptyCell(map);

                Obstacle tempObstacle =
                    new Obstacle(new Vector2(randomCell.X * cellSize, randomCell.Y * cellSize), Content.Load<Texture2D>("map/obstacle1"));
                obstacles.Add(tempObstacle);
            }

            //levelManager

            level = new Level(map, enemies, items, obstacles, floor, wall, cellSize, player);

            font = Content.Load<SpriteFont>("fonts/Default");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            camera.Move();

            level.Update(gameTime, GraphicsDevice);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (areColliding)
            {
                collision = "Collision";
            }
            else
            {
                collision = "No Collision";
            }

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.TranslationMatrix);

            level.Draw(gameTime, spriteBatch);

            spriteBatch.DrawString(font, collision, new Vector2(400, 100), Color.Black);
            spriteBatch.DrawString(font, player.getItems(), new Vector2(400, 130), Color.Black);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        private Cell GetRandomEmptyCell(Map map)
        {
            while (true)
            {
                int x = random.Next(map.Width);
                int y = random.Next(map.Height);
                if (map.IsWalkable(x, y))
                {
                    return map.GetCell(x, y);
                }
            }
        }
    }
}
