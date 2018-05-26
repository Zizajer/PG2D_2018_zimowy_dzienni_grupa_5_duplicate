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
    public class LevelManager
    {
        public Player player;
        public List<Level> levels;
        public ContentManager Content;
        public int newMapWidth = 15;
        public int newMapHeight = 8;
        public int newMapRoomCount = 100;
        public int newMapRoomWidth = 7;
        public int newMapRoomHeight = 2;
        public int enemiesCount = 1;
        public int itemsCount = 0;
        public int rocksCount = 0;
        public float enemySpeedFactor = 1.0f;
        public Texture2D floor;
        public Texture2D wall;
        public Texture2D fireball;
        public Texture2D bossFireball;
        public int cellSize;

        public Dictionary<string, Animation> _animations;
        public Dictionary<string, Animation> _animationsBoss;

        public List<Texture2D> allItems;
        public List<String> allItemsNames;

        public Texture2D rock;

        public Texture2D portalTexture;

        public LevelManager(ContentManager Content)
        {
            levels = new List<Level>();
            this.Content = Content;
            
            floor = Content.Load<Texture2D>("map/Floor");
            wall = Content.Load<Texture2D>("map/Wall");
            fireball = Content.Load<Texture2D>("spells/Fireball");
            bossFireball= Content.Load<Texture2D>("spells/BossFireball");
            rock = Content.Load<Texture2D>("map/rock");
            portalTexture = Content.Load<Texture2D>("map/portal");

            _animations = new Dictionary<string, Animation>()
                {
                    {"WalkUp",new Animation(Content.Load<Texture2D>("enemy/EnemyWalkingup"),3 )},
                    {"WalkDown",new Animation(Content.Load<Texture2D>("enemy/EnemyWalkingDown"),3 )},
                    {"WalkLeft",new Animation(Content.Load<Texture2D>("enemy/EnemyWalkingLeft"),3 )},
                    {"WalkRight",new Animation(Content.Load<Texture2D>("enemy/EnemyWalkingRight"),3 )}
                };
            _animationsBoss = new Dictionary<string, Animation>()
                {
                    {"BossAlive",new Animation(Content.Load<Texture2D>("enemy/BossAlive"),3 )}
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

            CreateNormalLevel();
            Cell randomCell = GetRandomEmptyCell(levels[0].map, levels[0].occupiedCells, levels[0].grid);
            player =
             new Player(Content, cellSize, 0, "Player") //TODO: Set player name at the game beginning
             {
                 Position = new Vector2((randomCell.X * cellSize + cellSize / 3), (randomCell.Y * cellSize) + cellSize / 3)
             };

            levels[0].map.ComputeFov(randomCell.X, randomCell.Y, 15, true);
            levels[0].addPlayer(player);
            Global.Camera.CenterOn(randomCell);
        }

        public void incrementMapParameters(int increaseValue)
        {
            newMapWidth = newMapWidth + increaseValue*2;
            newMapHeight = newMapHeight + increaseValue;
        }

        public void incrementOtherParameters(int increaseValue)
        {
            enemiesCount = enemiesCount + 5;
            if (Global.random.Next(4) % 2 == 0) 
            rocksCount = rocksCount + increaseValue;
            enemySpeedFactor = enemySpeedFactor + 0.5f;
            if (player != null && player.CurrentMapLevel > 5)
            {
                itemsCount = Global.random.Next(1);
            }
        }

        public void CreateNormalLevel()
        {
            Map map = CreateMap(newMapWidth, newMapHeight, newMapRoomCount, newMapRoomWidth, newMapRoomHeight);
            var grid = new Grid(newMapWidth, newMapHeight, 1.0f);


            List<Cell> occupiedCells = new List<Cell>();

            Portal portal = new Portal(portalTexture);

            List<Character> enemies = CreateEnemiesList(Content, map, cellSize, enemiesCount, occupiedCells, grid);
            List<Item> items = CreateItemsList(Content, map, cellSize, itemsCount, occupiedCells, allItems, allItemsNames, grid);
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

        public void CreateBossLevel()
        {
            Map map = CreateMap(15, 11, 50, 10, 10);
            var grid = new Grid(15, 11, 1.0f);

            List<Cell> occupiedCells = new List<Cell>();

            Portal portal = new Portal(portalTexture);

            List<Character> enemies =new List<Character>(1);

            Cell randomCell = map.GetCell(5, 5);
            grid.BlockCell(new Position(5, 5));
            grid.BlockCell(new Position(6, 5));
            grid.BlockCell(new Position(7, 5));

            grid.BlockCell(new Position(5, 6));
            grid.BlockCell(new Position(6, 6));
            grid.BlockCell(new Position(7, 6));

            grid.BlockCell(new Position(5, 7));
            grid.BlockCell(new Position(6, 7));
            grid.BlockCell(new Position(7, 7));
            occupiedCells.Add(randomCell);

            float timeBetweenActions = 1f;
            Character tempBoss =
                new Boss(_animationsBoss, cellSize, player.CurrentMapLevel, timeBetweenActions, map)
                {
                    Position = new Vector2((randomCell.X * cellSize), (randomCell.Y * cellSize))
                };
                enemies.Add(tempBoss);

            Global.Camera.setParams(map.Width, map.Height, cellSize);

            foreach (Cell cell in map.GetAllCells())
            {
                if (!cell.IsWalkable)
                {
                    grid.BlockCell(new Position(cell.X, cell.Y));
                }
            }

            Level level = new Level(map, grid, cellSize, enemies, allItems, allItemsNames, floor, wall, portal, occupiedCells, fireball, bossFireball);

            levels.Add(level);
        }

        private List<Rock> CreateRocksList(ContentManager Content, Map map, int cellSize, int rocksCount, List<Cell> occupiedCells, Texture2D rock, Grid grid)
        {
            List<Rock> rocks = new List<Rock>(rocksCount);

            for (int i = 0; i < rocksCount; i++)
            {
                Cell randomCell = GetRandomEmptyCell(map, occupiedCells, grid);
                occupiedCells.Add(randomCell);
                map.SetCellProperties(randomCell.X, randomCell.Y, false, true);
                Rock tempRock =
                    new Rock(new Vector2(randomCell.X * cellSize, randomCell.Y * cellSize), rock);
                rocks.Add(tempRock);
                grid.BlockCell(new Position(randomCell.X, randomCell.Y));
            }

            return rocks;
        }

        private List<Item> CreateItemsList(ContentManager Content, Map map, int cellSize, int itemCount, List<Cell> occupiedCells, List<Texture2D> allItems, List<String> allItemsNames, Grid grid)
        {
            List <Item> items = new List<Item>(itemCount);

            for (int i = 0; i < itemCount; i++)
            {
                Cell randomCell = GetRandomEmptyCell(map, occupiedCells, grid);
                occupiedCells.Add(randomCell);
                int rand = Global.random.Next(2) + 1;
                Item tempItem=new Item(new Vector2(randomCell.X * cellSize + cellSize / 3, randomCell.Y * cellSize + cellSize / 3), allItems[rand], allItemsNames[rand]);
                items.Add(tempItem);
            }

            return items;
        }

        private List<Character> CreateEnemiesList(ContentManager Content, Map map, int cellSize, int enemyCount, List<Cell> occupiedCells,Grid grid)
        {
            List<Character> enemies = new List<Character>(enemyCount);

            for (int i = 0; i < enemyCount; i++)
            {
                Cell randomCell = GetRandomEmptyCell(map, occupiedCells, grid);
                occupiedCells.Add(randomCell);
                float speed = (Global.random.Next(2) + 1) / 0.9f + (float)Math.Log(enemySpeedFactor);
                float timeBetweenActions = 1f;

                int level;
                if (player == null) // Case when the game has just begun and we haven't initialized our player yet (player is initialized AFTER creating first level in constructor of this class)
                {
                    level = 1;
                }
                else
                {
                    level = Global.random.Next(player.CurrentMapLevel, player.CurrentMapLevel + 3);
                }

                Character tempEnemy =
                    new Enemy(_animations, cellSize, level, speed, timeBetweenActions, map)
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
            levels[player.CurrentMapLevel].Draw(gameTime, spriteBatch);
        }

        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            levels[player.CurrentMapLevel].Update(gameTime, graphicsDevice);
            if (levels[player.CurrentMapLevel].finished == true)
            {
                if (player.CurrentMapLevel % 2 == 1)
                {
                    CreateNormalLevel();
                }
                else
                {
                    CreateBossLevel();
                }
                
                player.CurrentMapLevel++;
                player.currentState = Player.State.Standing;
                levels[player.CurrentMapLevel].addPlayer(player);
                Vector2 newPlayerPosition = levels[player.CurrentMapLevel].GetRandomEmptyCell();
                player.Position = newPlayerPosition;
                Global.Camera.CenterOn(player.Center);
                levels[player.CurrentMapLevel - 1] = null;
            }
        }

        private Cell GetRandomEmptyCell(Map map, List<Cell> occupiedCells, Grid grid)
        {
            int x, y;
            Cell tempCell;
            do
            {
                x = Global.random.Next(map.Width);
                y = Global.random.Next(map.Height);
                tempCell = map.GetCell(x, y);
            } while (!tempCell.IsWalkable || grid.GetCellCost(new Position(x, y)) > 1.0f || occupiedCells.Contains(tempCell));
            return tempCell;
        }
    }
}
