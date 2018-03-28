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

            Map map = CreateMap(16, 10, 100, 3, 3);
            Texture2D floor = LoadFloorTexture(Content);
            Texture2D wall = LoadWallTexture(Content);

            List<Enemy> enemies = CreateEnemiesList(Content, map, floor.Width);
            List<Item> items = CreateItemsList(Content, map, floor.Width);
            List<Obstacle> obstacles = CreateObstaclesList(Content, map, floor.Width);

            Global.Camera.setParams(map.Width, map.Height, floor.Width);

            Cell randomCell = GetRandomEmptyCell(map);
            Global.Camera.CenterOn(randomCell);

            this.player =
                new Player(Content, floor.Width)
                {
                    Position = new Vector2((randomCell.X * floor.Width + floor.Width / 3), (randomCell.Y * floor.Width) + floor.Width / 3)
                };

            Global.Gui = new GUI(player, Content.Load<SpriteFont>("fonts/Default"));
            //Global.CombatManager = new CombatManager(player, enemies); 

            Level level = new Level(map, enemies, items, obstacles, floor, wall, player);

            this.levels.Add(level);
        }

        public void CreateLevel(ContentManager Content)
        {
            Map map = CreateMap(16, 10, 100, 3, 3);
            Texture2D floor = LoadFloorTexture(Content);
            Texture2D wall = LoadWallTexture(Content);

            List<Enemy> enemies = CreateEnemiesList(Content,map,floor.Width);
            List<Item> items = CreateItemsList(Content,map, floor.Width);
            List<Obstacle> obstacles = CreateObstaclesList(Content,map, floor.Width);

            Global.Camera.setParams(map.Width, map.Height, floor.Width);

            Level level = new Level(map, enemies, items, obstacles, floor, wall, player);

            this.levels.Add(level);
        }

        private Texture2D LoadWallTexture(ContentManager Content)
        {
            Texture2D wall = Content.Load<Texture2D>("map/Wall");
            return wall;
        }

        private Texture2D LoadFloorTexture(ContentManager Content)
        {
            Texture2D floor = Content.Load<Texture2D>("map/Floor");
            return floor;
        }

        private List<Obstacle> CreateObstaclesList(ContentManager Content, Map map, int cellSize)
        {
            List<Obstacle> obstacles = new List<Obstacle>(3);

            for (int i = 0; i < 3; i++)
            {
                Cell randomCell = GetRandomEmptyCell(map);

                Obstacle tempObstacle =
                    new Obstacle(new Vector2(randomCell.X * cellSize, randomCell.Y * cellSize), Content.Load<Texture2D>("map/obstacle1"));
                obstacles.Add(tempObstacle);
            }

            return obstacles;
        }

        private List<Item> CreateItemsList(ContentManager Content, Map map, int cellSize)
        {
            List<Item> items = new List<Item>(3);

            Cell randomCell = GetRandomEmptyCell(map);
            items.Add(new Item(new Vector2(randomCell.X * cellSize + cellSize / 3, randomCell.Y * cellSize + cellSize / 3), Content.Load<Texture2D>("items/bow1"), "Bow"));
            randomCell = GetRandomEmptyCell(map);
            items.Add(new Item(new Vector2(randomCell.X * cellSize + cellSize / 3, randomCell.Y * cellSize + cellSize / 3), Content.Load<Texture2D>("items/sword1"), "Sword"));
            randomCell = GetRandomEmptyCell(map);
            items.Add(new Item(new Vector2(randomCell.X * cellSize + cellSize / 3, randomCell.Y * cellSize + cellSize / 3), Content.Load<Texture2D>("items/wand1"), "Wand"));

            return items;
        }

        private List<Enemy> CreateEnemiesList(ContentManager Content, Map map, int cellSize)
        {
            List<Enemy> enemies = new List<Enemy>(5);

            for (int i = 0; i < 5; i++)
            {
                Cell randomCell = GetRandomEmptyCell(map);
                float speed = (Global.random.Next(2) + 1) / 0.7f;
                float timeBetweenActions = (Global.random.Next(2)) + 1 / 0.7f;
                Enemy tempEnemy =
                    new Enemy(Content, cellSize, speed, timeBetweenActions)
                    {
                        Position = new Vector2((randomCell.X * cellSize + cellSize / 3), (randomCell.Y * cellSize) + cellSize / 3)
                    };
                enemies.Add(tempEnemy);
            }

            return enemies;
        }

        private Map CreateMap(int mapWidth,int mapHeight,int roomCount, int roomWidth, int roomHeight)
        {
            IMapCreationStrategy<Map> mapCreationStrategy =
                new RandomRoomsMapCreationStrategy<Map>(mapWidth, mapHeight, roomCount, roomWidth, roomHeight);
            Map map = Map.Create(mapCreationStrategy);
            return map;
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
