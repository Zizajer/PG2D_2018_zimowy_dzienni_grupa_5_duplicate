using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using RogueSharp.MapCreation;
using System;
using System.Collections.Generic;

namespace Dungeon_Crawler
{
    class LevelManager
    {
        private Player player;
        List<Level> levels;
        ContentManager Content;
        private int newMapWidth = 15;
        private int newMapHeight = 8;
        private int newMapRoomCount = 100;
        private int newMapRoomMaxSize = 7;
        private int newMapRoomMinSize = 2;
        private int enemiesCount = 1;
        private int itemsCount = 1;
        private int obstaclesCount = 0;
        Texture2D floor;
        Texture2D wall;
        Texture2D fireball;

        Dictionary<string, Animation> _animations;

        List<Texture2D> allItems;
        List<String> allItemsNames;

        Texture2D obstacle;

        Texture2D portalTexture;

        public LevelManager(ContentManager Content)
        {
            levels = new List<Level>();
            this.Content = Content;
            
            floor = Content.Load<Texture2D>("map/Floor");
            wall = Content.Load<Texture2D>("map/Wall");
            fireball = Content.Load<Texture2D>("spells/Fireball");
            obstacle = Content.Load<Texture2D>("map/obstacle1");
            portalTexture = Content.Load<Texture2D>("map/portal");

            _animations = new Dictionary<string, Animation>()
                {
                    {"WalkUp",new Animation(Content.Load<Texture2D>("enemy/EnemyWalkingup"),3 )},
                    {"WalkDown",new Animation(Content.Load<Texture2D>("enemy/EnemyWalkingDown"),3 )},
                    {"WalkLeft",new Animation(Content.Load<Texture2D>("enemy/EnemyWalkingLeft"),3 )},
                    {"WalkRight",new Animation(Content.Load<Texture2D>("enemy/EnemyWalkingRight"),3 )}
                };

            allItems = new List<Texture2D>(3);
            allItemsNames = new List<String>(3);

            allItems.Add(Content.Load<Texture2D>("items/bow1"));
            allItems.Add(Content.Load<Texture2D>("items/sword1"));
            allItems.Add(Content.Load<Texture2D>("items/wand1"));

            allItemsNames.Add("Bow");
            allItemsNames.Add("Sword");
            allItemsNames.Add("Wand");

            this.player =
                new Player(Content, floor.Width, 0)
                {
                    Position = new Vector2((floor.Width + floor.Width / 3), ( floor.Width) + floor.Width / 3)
                };

            CreateLevel();

            Cell randomCell = GetRandomEmptyCell(levels[0].map, levels[0].occupiedCells);
            player.Position = new Vector2((randomCell.X * floor.Width + floor.Width / 3), (randomCell.Y * floor.Width) + floor.Width / 3);

            Global.Camera.CenterOn(randomCell);

            Global.Gui = new GUI(player, Content.Load<SpriteFont>("fonts/Default"));
        }

        public void incrementMapParameters(int increaseValue)
        {
            newMapWidth = newMapWidth + increaseValue*2;
            newMapHeight = newMapHeight + increaseValue;
        }

        public void incrementOtherParameters(int increaseValue)
        {
            //if (player.CurrentLevel % 3 == 2) 
            enemiesCount = enemiesCount + increaseValue;
            //if (player.CurrentLevel % 2 == 0) 
            itemsCount = itemsCount + increaseValue;
            //if (player.CurrentLevel % 2 == 1) 
            obstaclesCount = obstaclesCount + increaseValue;
        }

        public void CreateLevel()
        {
            Map map = CreateMap(newMapWidth, newMapHeight, newMapRoomCount, newMapRoomMaxSize, newMapRoomMinSize);

            incrementMapParameters(2);

            List<Cell> occupiedCells = new List<Cell>();

            Cell portalcell = GetCellFarFromPlayer(map, occupiedCells, player);
            occupiedCells.Add(portalcell);
            Portal portal =
                new Portal(new Vector2(portalcell.X * floor.Width, portalcell.Y * floor.Width), portalTexture);

            List<Enemy> enemies = CreateEnemiesList(Content, map, floor.Width, enemiesCount, occupiedCells);
            List<Item> items = CreateItemsList(Content, map, floor.Width, itemsCount, occupiedCells, allItems, allItemsNames);
            List<Obstacle> obstacles = CreateObstaclesList(Content, map, floor.Width, obstaclesCount, occupiedCells, obstacle);
            incrementOtherParameters(1);

            Global.Camera.setParams(map.Width, map.Height, floor.Width);

            Level level = new Level(map, floor.Width, enemies, allItems, allItemsNames, items, obstacles, floor, wall, portal, player, occupiedCells, fireball);

            this.levels.Add(level);
        }

        private List<Obstacle> CreateObstaclesList(ContentManager Content, Map map, int cellSize, int obstacleCount, List<Cell> occupiedCells, Texture2D obstacle)
        {
            List<Obstacle> obstacles = new List<Obstacle>(obstacleCount);

            for (int i = 0; i < obstacleCount; i++)
            {
                Cell randomCell = GetRandomEmptyCell(map, occupiedCells);
                occupiedCells.Add(randomCell);
                //Set property of a cell occupied by an obstacle on a map to make it non-transparent. Necessary for fov calculations.
                map.SetCellProperties(randomCell.X, randomCell.Y, false, true);
                Obstacle tempObstacle =
                    new Obstacle(new Vector2(randomCell.X * cellSize, randomCell.Y * cellSize), obstacle);
                obstacles.Add(tempObstacle);
            }

            return obstacles;
        }

        private List<Item> CreateItemsList(ContentManager Content, Map map, int cellSize, int itemCount, List<Cell> occupiedCells, List<Texture2D> allItems, List<String> allItemsNames)
        {
            List <Item> items = new List<Item>(itemCount);

            for (int i = 0; i < itemCount; i++)
            {
                Cell randomCell = GetRandomEmptyCell(map, occupiedCells);
                occupiedCells.Add(randomCell);
                int rand = Global.random.Next(2) + 1;
                Item tempItem=new Item(new Vector2(randomCell.X * cellSize + cellSize / 3, randomCell.Y * cellSize + cellSize / 3), allItems[rand], allItemsNames[rand]);
                items.Add(tempItem);
            }

            return items;
        }

        private List<Enemy> CreateEnemiesList(ContentManager Content, Map map, int cellSize, int enemyCount, List<Cell> occupiedCells)
        {
            List<Enemy> enemies = new List<Enemy>(enemyCount);

            for (int i = 0; i < enemyCount; i++)
            {
                Cell randomCell = GetRandomEmptyCell(map, occupiedCells);
                occupiedCells.Add(randomCell);
                float speed = (Global.random.Next(2) + 1) / 0.7f;
                float timeBetweenActions = (Global.random.Next(2)) + 1 / 0.7f;
                Enemy tempEnemy =
                    new Enemy(_animations, cellSize, speed, timeBetweenActions)
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
            levels[player.CurrentLevel].Draw(gameTime, spriteBatch);
        }

        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            levels[player.CurrentLevel].Update(gameTime, graphicsDevice);
            if (levels[player.CurrentLevel].finished == true)
            {
                CreateLevel();
                player.CurrentLevel++;
                Vector2 newPlayerPosition = levels[player.CurrentLevel].GetRandomEmptyCell();
                player.Position = newPlayerPosition;
            }
        }

        private Cell GetRandomEmptyCell(Map map, List<Cell> occupiedCells)
        {
            int x, y;
            Cell tempCell;
            do
            {
                x = Global.random.Next(map.Width);
                y = Global.random.Next(map.Height);
                tempCell = map.GetCell(x, y);
            } while (!tempCell.IsWalkable || occupiedCells.Contains(tempCell));
            return tempCell;
        }
        private Cell GetCellFarFromPlayer(Map map, List<Cell> occupiedCells, Player player)
        {
            int x, y;
            Cell tempCell;
            do
            {
                x = Global.random.Next(map.Width);
                y = Global.random.Next(map.Height);
                tempCell = map.GetCell(x, y);
            } while (!tempCell.IsWalkable || occupiedCells.Contains(tempCell));
            return tempCell;
        }
    }
}
