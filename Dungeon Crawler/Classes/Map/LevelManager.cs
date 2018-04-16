using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using RogueSharp.MapCreation;
using System;
using System.Collections.Generic;
using RoyT.AStar;

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
        private int rocksCount = 0;
        Texture2D floor;
        Texture2D wall;
        Texture2D fireball;
        int cellSize;

        Dictionary<string, Animation> _animations;

        List<Texture2D> allItems;
        List<String> allItemsNames;

        Texture2D rock;

        Texture2D portalTexture;

        public LevelManager(ContentManager Content)
        {
            levels = new List<Level>();
            this.Content = Content;
            
            floor = Content.Load<Texture2D>("map/Floor");
            wall = Content.Load<Texture2D>("map/Wall");
            fireball = Content.Load<Texture2D>("spells/Fireball");
            rock = Content.Load<Texture2D>("map/rock");
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
            player =
             new Player(Content, cellSize, 0)
             {
                 Position = new Vector2((randomCell.X * cellSize + cellSize / 3), (randomCell.Y * cellSize) + cellSize / 3)
             };

            levels[0].map.ComputeFov(randomCell.X, randomCell.Y, 15, true);
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
            //if (Global.random.Next(4) % 3 == 0) 
            enemiesCount = enemiesCount + 5;
            if (Global.random.Next(4) % 3 == 0) 
            itemsCount = itemsCount + increaseValue;
            if (Global.random.Next(4) % 2 == 0) 
            rocksCount = rocksCount + increaseValue;
        }

        public void CreateLevel()
        {
            Map map = CreateMap(newMapWidth, newMapHeight, newMapRoomCount, newMapRoomMaxSize, newMapRoomMinSize);
            var grid = new Grid(newMapWidth, newMapHeight, 1.0f);


            List<Cell> occupiedCells = new List<Cell>();

            Cell portalcell = GetRandomEmptyCell(map, occupiedCells);
            occupiedCells.Add(portalcell);
            Portal portal =
                new Portal(new Vector2(portalcell.X * cellSize, portalcell.Y * cellSize), portalTexture);

            List<Enemy> enemies = CreateEnemiesList(Content, map, cellSize, enemiesCount, occupiedCells, grid);
            List<Item> items = CreateItemsList(Content, map, cellSize, itemsCount, occupiedCells, allItems, allItemsNames);
            List<Rock> rocks = CreateRocksList(Content, map, cellSize, rocksCount, occupiedCells, rock, grid);
            
            Global.Camera.setParams(map.Width, map.Height, cellSize);

            foreach (Cell cell in map.GetAllCells())
            {
                if (!cell.IsWalkable)
                {
                    grid.BlockCell(new Position(cell.X, cell.Y));
                }
            }

            Level level = new Level(map, grid, cellSize, enemies, allItems, allItemsNames, items, rocks, floor, wall, portal, occupiedCells, fireball);

            levels.Add(level);

            incrementMapParameters(2);
            incrementOtherParameters(1);
        }

        private List<Rock> CreateRocksList(ContentManager Content, Map map, int cellSize, int rocksCount, List<Cell> occupiedCells, Texture2D rock, Grid grid)
        {
            List<Rock> rocks = new List<Rock>(rocksCount);

            for (int i = 0; i < rocksCount; i++)
            {
                Cell randomCell = GetRandomEmptyCell(map, occupiedCells);
                occupiedCells.Add(randomCell);
                //Set property of a cell occupied by an rock on a map to make it non-transparent. Necessary for fov calculations.
                map.SetCellProperties(randomCell.X, randomCell.Y, false, true);
                Rock tempRock =
                    new Rock(new Vector2(randomCell.X * cellSize, randomCell.Y * cellSize), rock);
                rocks.Add(tempRock);
                grid.BlockCell(new Position(randomCell.X, randomCell.Y));
            }

            return rocks;
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

        private List<Enemy> CreateEnemiesList(ContentManager Content, Map map, int cellSize, int enemyCount, List<Cell> occupiedCells,Grid grid)
        {
            List<Enemy> enemies = new List<Enemy>(enemyCount);

            for (int i = 0; i < enemyCount; i++)
            {
                Cell randomCell = GetRandomEmptyCell(map, occupiedCells);
                occupiedCells.Add(randomCell);
                float speed = (Global.random.Next(2) + 1) / 0.7f;
                float timeBetweenActions = 1f;
                Enemy tempEnemy =
                    new Enemy(_animations, cellSize, speed, timeBetweenActions, map)
                    {
                        Position = new Vector2((randomCell.X * cellSize + cellSize /3), (randomCell.Y * cellSize) + cellSize /3)
                    };
                enemies.Add(tempEnemy);
                grid.SetCellCost(new Position(randomCell.X, randomCell.Y), 5.0f);
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
                Global.Camera.CenterOn(player.Center);
                levels[player.CurrentLevel - 1] = null;
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
