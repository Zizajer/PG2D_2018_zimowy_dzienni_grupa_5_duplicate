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
        int cellSize;

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

            cellSize = floor.Width;

            CreateLevel();
            Cell randomCell = GetRandomEmptyCell(levels[0].map, levels[0].occupiedCells);
            this.player =
             new Player(Content, cellSize, 0)
             {
                 Position = new Vector2((randomCell.X * cellSize + cellSize / 3), (randomCell.Y * cellSize) + cellSize / 3)
             };

            levels[0].addPlayer(player);
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
            enemiesCount = enemiesCount + 5;
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

            Cell portalcell = GetRandomEmptyCell(map, occupiedCells);
            occupiedCells.Add(portalcell);
            Portal portal =
                new Portal(new Vector2(portalcell.X * cellSize, portalcell.Y * cellSize), portalTexture);

            List<Enemy> enemies = CreateEnemiesList(Content, map, cellSize, enemiesCount, occupiedCells);
            List<Item> items = CreateItemsList(Content, map, cellSize, itemsCount, occupiedCells, allItems, allItemsNames);
            List<Obstacle> obstacles = CreateObstaclesList(Content, map, cellSize, obstaclesCount, occupiedCells, obstacle);
            incrementOtherParameters(1);

            Global.Camera.setParams(map.Width, map.Height, cellSize);

            Level level = new Level(map, cellSize, enemies, allItems, allItemsNames, items, obstacles, floor, wall, portal, occupiedCells, fireball);

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
                    new Enemy(_animations, cellSize, speed, timeBetweenActions, map)
                    {
                        Position = new Vector2((randomCell.X * cellSize + cellSize /3), (randomCell.Y * cellSize) + cellSize /3)
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
                levels[player.CurrentLevel].addPlayer(player);
                Vector2 newPlayerPosition = levels[player.CurrentLevel].GetRandomEmptyCell();
                player.Position = newPlayerPosition;
                Global.Camera.CenterOn(player.Origin);
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
    }
}
