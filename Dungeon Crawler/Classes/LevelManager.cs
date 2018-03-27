using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using RogueSharp.MapCreation;
using System.Collections.Generic;

namespace Dungeon_Crawler
{
    class LevelManager
    {
        private Player player;
        List<Level> levels;

        public LevelManager(ContentManager Content)
        {
            levels = new List<Level>();
            IMapCreationStrategy<Map> mapCreationStrategy =
                new RandomRoomsMapCreationStrategy<Map>(16, 10, 100, 3, 3);
            Map map = Map.Create(mapCreationStrategy);

            Texture2D floor = Content.Load<Texture2D>("map/Floor");
            Texture2D wall = Content.Load<Texture2D>("map/Wall");
            int cellSize = floor.Width;

            Global.Camera.setParams(map.Width, map.Height, cellSize);


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
            Global.Camera.CenterOn(randomCell);

            this.player =
                new Player(Content, cellSize)
                {
                    Position = new Vector2((randomCell.X * cellSize + cellSize / 3), (randomCell.Y * cellSize) + cellSize / 3)
                };

            for (int i = 0; i < 5; i++)
            {
                randomCell = GetRandomEmptyCell(map);
                float speed = (Global.random.Next(2) + 1) / 0.7f;
                float timeBetweenActions = (Global.random.Next(2)) + 1 / 0.7f;
                Enemy tempEnemy =
                    new Enemy(Content, cellSize, speed, timeBetweenActions)
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

            Level level = new Level(map, enemies, items, obstacles, floor, wall, cellSize, player);

            this.levels.Add(level);

            Global.Gui = new GUI(player, Content.Load<SpriteFont>("fonts/Default"));
            //Global.CombatManager = new CombatManager(player, enemies);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach(Level level in levels)
            {
                level.Draw(gameTime, spriteBatch);
            }
        }

        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            foreach (Level level in levels)
            {
                level.Update(gameTime, graphicsDevice);
            }
        }

        private Cell GetRandomEmptyCell(Map map)
        {
            while (true)
            {
                int x = Global.random.Next(map.Width);
                int y = Global.random.Next(map.Height);
                if (map.IsWalkable(x, y))
                {
                    return map.GetCell(x, y);
                }
            }
        }
    }
}
