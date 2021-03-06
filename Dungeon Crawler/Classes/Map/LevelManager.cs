﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using RogueSharp.MapCreation;
using System;
using System.Collections.Generic;
using RoyT.AStar;
using System.IO;
using System.Linq;
using System.Diagnostics;

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
        public int itemsCount;
        public int rocksCount = 0;
        public float enemySpeedFactor = 1.0f;

        public List<Texture2D> floors = new List<Texture2D>();
        public List<Texture2D> walls = new List<Texture2D>();

        public int cellSize;

        public Dictionary<string, Animation> _animationsBlob;
        public Dictionary<string, Animation> _animationsSkeleton;
        public Dictionary<string, Animation> _animationsZombie;
        public Dictionary<string, Animation> _animationsDemonOak;
        public Dictionary<string, Animation> _animationsDragon;
        public Dictionary<string, Animation> _animationsBlackKnight;
        public Dictionary<string, Animation> _animationsBrainy;
        public Dictionary<string, Animation> _animationsSpider;
        public Dictionary<string, Animation> _animationsLich;

        int whichBossToSpawn = -1;
        public List<String> allItemsNames;

        public Texture2D rock;

        public Texture2D portalTexture;

        List<String> BlobNamesList;
        List<String> DemonOakNamesList;
        List<String> DragonNamesList;
        List<String> BlackKnightNamesList;
        List<String> SkeletonNamesList;
        List<String> ZombieNamesList;
        List<String> BrainyNamesList;
        List<String> SpiderNamesList;
        List<String> LichNamesList;

        public List<String> MageNamesList;
        public List<String> WarriorNamesList;
        public List<String> RangerNamesList;

        public LevelManager(ContentManager Content)
        {
            levels = new List<Level>();
            this.Content = Content;

            floors.Add(Content.Load<Texture2D>("map/Floor"));
            floors.Add(Content.Load<Texture2D>("map/Floor2"));
            floors.Add(Content.Load<Texture2D>("map/Floor3"));
            floors.Add(Content.Load<Texture2D>("map/Floor4"));

            walls.Add(Content.Load<Texture2D>("map/Wall"));
            walls.Add(Content.Load<Texture2D>("map/Wall2"));
            walls.Add(Content.Load<Texture2D>("map/Wall3"));
            walls.Add(Content.Load<Texture2D>("map/Wall4"));

            rock = Content.Load<Texture2D>("map/rock");
            portalTexture = Content.Load<Texture2D>("map/portal");

            _animationsBlob = new Dictionary<string, Animation>()
                {
                    {"WalkUp",new Animation(Content.Load<Texture2D>("enemy/Blob/Walkingup"),3 )},
                    {"WalkDown",new Animation(Content.Load<Texture2D>("enemy/Blob/WalkingDown"),3 )},
                    {"WalkLeft",new Animation(Content.Load<Texture2D>("enemy/Blob/WalkingLeft"),3 )},
                    {"WalkRight",new Animation(Content.Load<Texture2D>("enemy/Blob/WalkingRight"),3 )}
                };
            _animationsSkeleton = new Dictionary<string, Animation>()
                {
                    {"WalkUp",new Animation(Content.Load<Texture2D>("enemy/Skeleton/Walkingup"),3 )},
                    {"WalkDown",new Animation(Content.Load<Texture2D>("enemy/Skeleton/WalkingDown"),3 )},
                    {"WalkLeft",new Animation(Content.Load<Texture2D>("enemy/Skeleton/WalkingLeft"),3 )},
                    {"WalkRight",new Animation(Content.Load<Texture2D>("enemy/Skeleton/WalkingRight"),3 )}
                };
            _animationsZombie = new Dictionary<string, Animation>()
                {
                    {"WalkUp",new Animation(Content.Load<Texture2D>("enemy/Zombie/Walkingup"),3 )},
                    {"WalkDown",new Animation(Content.Load<Texture2D>("enemy/Zombie/WalkingDown"),3 )},
                    {"WalkLeft",new Animation(Content.Load<Texture2D>("enemy/Zombie/WalkingLeft"),3 )},
                    {"WalkRight",new Animation(Content.Load<Texture2D>("enemy/Zombie/WalkingRight"),3 )}
                };
            _animationsDemonOak = new Dictionary<string, Animation>()
                {
                    {"BossAlive",new Animation(Content.Load<Texture2D>("enemy/DemonOak/BossAlive"),3 )}
                };
            _animationsLich = new Dictionary<string, Animation>()
                {
                    {"BossAlive",new Animation(Content.Load<Texture2D>("enemy/Lich/BossAlive"),3 )}
                };
            _animationsDragon = new Dictionary<string, Animation>()
                {
                    {"BossAlive",new Animation(Content.Load<Texture2D>("enemy/Dragon/BossAlive"),3 )}
                };
            _animationsBlackKnight = new Dictionary<string, Animation>()
                {
                    {"WalkUp",new Animation(Content.Load<Texture2D>("enemy/BlackKnight/Walkingup"),3 )},
                    {"WalkDown",new Animation(Content.Load<Texture2D>("enemy/BlackKnight/WalkingDown"),3 )},
                    {"WalkLeft",new Animation(Content.Load<Texture2D>("enemy/BlackKnight/WalkingLeft"),3 )},
                    {"WalkRight",new Animation(Content.Load<Texture2D>("enemy/BlackKnight/WalkingRight"),3 )}
                };
            _animationsBrainy = new Dictionary<string, Animation>()
                {
                    {"WalkUp",new Animation(Content.Load<Texture2D>("enemy/Brainy/Walkingup"),3 )},
                    {"WalkDown",new Animation(Content.Load<Texture2D>("enemy/Brainy/WalkingDown"),3 )},
                    {"WalkLeft",new Animation(Content.Load<Texture2D>("enemy/Brainy/WalkingLeft"),3 )},
                    {"WalkRight",new Animation(Content.Load<Texture2D>("enemy/Brainy/WalkingRight"),3 )}
                };
            _animationsSpider = new Dictionary<string, Animation>()
                {
                    {"WalkUp",new Animation(Content.Load<Texture2D>("enemy/Spider/Walkingup"),6 )},
                    {"WalkDown",new Animation(Content.Load<Texture2D>("enemy/Spider/WalkingDown"),6 )},
                    {"WalkLeft",new Animation(Content.Load<Texture2D>("enemy/Spider/WalkingLeft"),6 )},
                    {"WalkRight",new Animation(Content.Load<Texture2D>("enemy/Spider/WalkingRight"),6 )}
                };
            allItemsNames = new List<String>
            {
                "Wand",
                "HealthIncreasingPotion",
                "InvisibilityPotion",
                "SmallHealthPotion",
                "SmallResourcePotion",
                "Shield",
                "Shield2",
                "Pendant",
                "Wand2",
                "Axe1",
                "Gloves",
                "Trainers",
                "DokerCharm",
                "FullHealthPotion",
                "BitterPowder",
                "IceCube"
            };
            itemsCount = 2;
            cellSize = floors[0].Width;

            try
            {
                BlobNamesList = new List<String>(File.ReadAllLines(@"..\..\..\..\files\names\Blob.txt"));
                DemonOakNamesList = new List<String>(File.ReadAllLines(@"..\..\..\..\files\names\DemonOak.txt"));
                DragonNamesList = new List<String>(File.ReadAllLines(@"..\..\..\..\files\names\Dragon.txt"));
                SkeletonNamesList = new List<String>(File.ReadAllLines(@"..\..\..\..\files\names\Skeleton.txt"));
                ZombieNamesList = new List<String>(File.ReadAllLines(@"..\..\..\..\files\names\Zombie.txt"));
                BlackKnightNamesList = new List<String>(File.ReadAllLines(@"..\..\..\..\files\names\BlackKnight.txt"));
                BrainyNamesList = new List<String>(File.ReadAllLines(@"..\..\..\..\files\names\Brainy.txt"));
                SpiderNamesList = new List<String>(File.ReadAllLines(@"..\..\..\..\files\names\Spider.txt"));
                LichNamesList = new List<String>(File.ReadAllLines(@"..\..\..\..\files\names\Lich.txt"));

                MageNamesList = new List<String>(File.ReadAllLines(@"..\..\..\..\files\names\Mage.txt"));
                WarriorNamesList = new List<String>(File.ReadAllLines(@"..\..\..\..\files\names\Warrior.txt"));
                RangerNamesList = new List<String>(File.ReadAllLines(@"..\..\..\..\files\names\Ranger.txt"));
            }
            catch (Exception e)
            {
                throw new Exception("Check enemy names files path's //Czareg\n" + e.Message);
            }

            CreateNormalLevel();

        }

        public void setPlayer(String ClassName)
        {
            Cell randomCell = GetRandomEmptyCell(levels[0].map, levels[0].occupiedCells, levels[0].grid, true);

            if (ClassName.Equals("Warrior"))
            {
                player =
                new Warrior(Content, cellSize, 0, Global.playerName + " the " + Global.playerClass)
                {
                    Position = new Vector2((randomCell.X * cellSize + cellSize / 3), (randomCell.Y * cellSize) + cellSize / 3)
                };

            }
            else if (ClassName.Equals("Ranger"))
            {
                player =
                new Ranger(Content, cellSize, 0, Global.playerName + " the " + Global.playerClass)
                {
                    Position = new Vector2((randomCell.X * cellSize + cellSize / 3), (randomCell.Y * cellSize) + cellSize / 3)
                };
            }
            else if (ClassName.Equals("Mage"))
            {
                player =
                new Mage(Content, cellSize, 0, Global.playerName + " the " + Global.playerClass)
                {
                    Position = new Vector2((randomCell.X * cellSize + cellSize / 3), (randomCell.Y * cellSize) + cellSize / 3)
                };
            }

            levels[0].addPlayer(player);
            levels[0].grid.SetCellCost(new Position(randomCell.X, randomCell.Y), 5.0f);
            Global.Camera.CenterOn(randomCell);

        }

        public void incrementMapParameters(int increaseValue)
        {
            newMapWidth = newMapWidth + increaseValue * 2;
            newMapHeight = newMapHeight + increaseValue;
        }

        public void incrementOtherParameters(int increaseValue)
        {
            enemiesCount = enemiesCount + 3;
            if (Global.random.Next(4) % 2 == 0)
                rocksCount = rocksCount + increaseValue;
            enemySpeedFactor = enemySpeedFactor + 0.2f;
            if (player != null && player.CurrentMapLevel > 5)
            {
                itemsCount = 3;
            }
        }

        public void CreateNormalLevel()
        {
            Map map = CreateMap(newMapWidth, newMapHeight, newMapRoomCount, newMapRoomWidth, newMapRoomHeight);
            var grid = new Grid(newMapWidth, newMapHeight, 1.0f);

            List<Cell> occupiedCells = new List<Cell>();

            Portal portal = new Portal(portalTexture);

            List<Character> enemies = CreateEnemiesList(Content, map, cellSize, enemiesCount, occupiedCells, grid);
            List<Item> items = CreateItemsList(Content, map, cellSize, itemsCount, occupiedCells, allItemsNames, grid);
            List<Rock> rocks = CreateRocksList(Content, map, cellSize, rocksCount, occupiedCells, rock, grid);

            Global.Camera.setParams(map.Width, map.Height, cellSize);

            foreach (Cell cell in map.GetAllCells())
            {
                if (!cell.IsWalkable)
                {
                    grid.BlockCell(new Position(cell.X, cell.Y));
                }
            }

            Level level = new Level(map, grid, cellSize, enemies, items, rocks, floors[Global.random.Next(floors.Count)], walls[Global.random.Next(walls.Count)], portal, occupiedCells);

            levels.Add(level);

            incrementMapParameters(1);
            incrementOtherParameters(1);
        }

        public void CreateBossLevel()
        {
            Map map = CreateBossMap(13, 13);
            var grid = new Grid(13, 13, 1.0f);

            List<Cell> occupiedCells = new List<Cell>();

            Portal portal = new Portal(portalTexture);

            List<Character> enemies = new List<Character>(1);
            List<Item> items = CreateItemsList(Content, map, cellSize, 0, occupiedCells, allItemsNames, grid);

            List<Item> BossInventory = new List<Item>();
            if (Global.random.Next(10) > 1) //Boss has 90% of chance to be equipped with 2-3 items
            {
                int numberOfItems = Global.random.Next(2, 4);
                BossInventory = CreateItemsList(Content, numberOfItems, allItemsNames);
            }

            whichBossToSpawn++;
            if (whichBossToSpawn > 4) whichBossToSpawn = 0;
            if (whichBossToSpawn == 0)
            {
                List<Cell> bossOccupyingCells = map.GetCellsInArea(6, 6, 1).ToList();

                Cell randomCell = map.GetCell(5, 5);
                foreach (Cell cell in bossOccupyingCells)
                {
                    grid.SetCellCost(new Position(cell.X, cell.Y), 5.0f);
                }

                occupiedCells.Union(bossOccupyingCells);

                float timeBetweenActions = 1f; //we should move that to each boss class but w/e
                Character tempBoss =
                    new Dragon(_animationsDragon, cellSize, player.CurrentMapLevel, timeBetweenActions, map, bossOccupyingCells)
                    {
                        Name = DragonNamesList[Global.random.Next(DragonNamesList.Count)],
                        Position = new Vector2((randomCell.X * cellSize), (randomCell.Y * cellSize))
                    };
                enemies.Add(tempBoss);
                tempBoss.TakeItems(BossInventory);
            }
            else if (whichBossToSpawn == 1)
            {
                Cell randomCell = GetRandomEmptyCell(map, occupiedCells, grid, true);
                List<Cell> bossOccupyingCells = new List<Cell>();

                float timeBetweenActions = 1.0f; //we should move that to each boss class but w/e
                Character tempBoss =
                    new BlackKnight(_animationsBlackKnight, cellSize, player.CurrentMapLevel, timeBetweenActions, map, bossOccupyingCells)
                    {
                        Name = BlackKnightNamesList[Global.random.Next(BlackKnightNamesList.Count)],
                        Position = new Vector2((randomCell.X * cellSize + cellSize / 4), (randomCell.Y * cellSize) + cellSize / 4)
                    };
                enemies.Add(tempBoss);
                tempBoss.TakeItems(BossInventory);

                grid.SetCellCost(new Position(randomCell.X, randomCell.Y), 5.0f);
            }
            else if (whichBossToSpawn == 2)
            {
                List<Cell> bossOccupyingCells = map.GetCellsInArea(6, 6, 1).ToList();

                Cell randomCell = map.GetCell(5, 5);
                foreach (Cell cell in bossOccupyingCells)
                {
                    grid.SetCellCost(new Position(cell.X, cell.Y), 5.0f);
                }

                occupiedCells.Union(bossOccupyingCells);

                float timeBetweenActions = 0.9f; //we should move that to each boss class but w/e
                Character tempBoss =
                    new Lich(_animationsLich, cellSize, player.CurrentMapLevel, timeBetweenActions, map, bossOccupyingCells)
                    {
                        Name = LichNamesList[Global.random.Next(LichNamesList.Count)],
                        Position = new Vector2((randomCell.X * cellSize), (randomCell.Y * cellSize))
                    };
                enemies.Add(tempBoss);
                tempBoss.TakeItems(BossInventory);
            }
            else if (whichBossToSpawn == 3)
            {
                Cell randomCell = GetRandomEmptyCell(map, occupiedCells, grid, true);
                List<Cell> bossOccupyingCells = new List<Cell>();

                float timeBetweenActions = 1.0f; //we should move that to each boss class but w/e
                Character tempBoss =
                    new Brainy(_animationsBrainy, cellSize, player.CurrentMapLevel, timeBetweenActions, map, bossOccupyingCells)
                    {
                        Name = BrainyNamesList[Global.random.Next(BrainyNamesList.Count)],
                        Position = new Vector2((randomCell.X * cellSize + cellSize / 4), (randomCell.Y * cellSize) + cellSize / 4)
                    };
                enemies.Add(tempBoss);
                tempBoss.TakeItems(BossInventory);

                grid.SetCellCost(new Position(randomCell.X, randomCell.Y), 5.0f);
            }
            else
            {
                List<Cell> bossOccupyingCells = map.GetCellsInArea(6, 6, 1).ToList();

                Cell randomCell = map.GetCell(5, 5);
                foreach (Cell cell in bossOccupyingCells)
                {
                    grid.SetCellCost(new Position(cell.X, cell.Y), 5.0f);
                }

                occupiedCells.Union(bossOccupyingCells);

                float timeBetweenActions = 1.7f; //we should move that to each boss class but w/e
                Character tempBoss =
                    new DemonOak(_animationsDemonOak, cellSize, player.CurrentMapLevel, timeBetweenActions, map, bossOccupyingCells)
                    {
                        Name = DemonOakNamesList[Global.random.Next(DemonOakNamesList.Count)],
                        Position = new Vector2((randomCell.X * cellSize), (randomCell.Y * cellSize))
                    };
                enemies.Add(tempBoss);
                tempBoss.TakeItems(BossInventory);
            }

            Global.Camera.setParams(map.Width, map.Height, cellSize);

            foreach (Cell cell in map.GetAllCells())
            {
                if (!cell.IsWalkable)
                {
                    grid.BlockCell(new Position(cell.X, cell.Y));
                }
            }

            Level level = new Level(map, grid, cellSize, enemies, items, floors[Global.random.Next(floors.Count)], walls[Global.random.Next(walls.Count)], portal, occupiedCells);

            levels.Add(level);
        }

        private List<Rock> CreateRocksList(ContentManager Content, Map map, int cellSize, int rocksCount, List<Cell> occupiedCells, Texture2D rock, Grid grid)
        {
            List<Rock> rocks = new List<Rock>(rocksCount);

            for (int i = 0; i < rocksCount; i++)
            {
                Cell randomCell = GetRandomEmptyCell(map, occupiedCells, grid, false);
                occupiedCells.Add(randomCell);
                map.SetCellProperties(randomCell.X, randomCell.Y, false, true);
                Rock tempRock =
                    new Rock(new Vector2(randomCell.X * cellSize, randomCell.Y * cellSize), rock);
                rocks.Add(tempRock);
                grid.BlockCell(new Position(randomCell.X, randomCell.Y));
            }

            return rocks;
        }

        private List<Item> CreateItemsList(ContentManager Content, Map map, int cellSize, int itemCount, List<Cell> occupiedCells, List<String> allItemsNames, Grid grid)
        {
            List<Item> items = new List<Item>(itemCount);

            for (int i = 0; i < itemCount; i++)
            {
                Cell randomCell = GetRandomEmptyCell(map, occupiedCells, grid, true);
                occupiedCells.Add(randomCell);
                String randomItem = allItemsNames[Global.random.Next(allItemsNames.Count)];
                Item tempItem = (Item)Activator.CreateInstance(Type.GetType("Dungeon_Crawler." + randomItem), Content, new Vector2(randomCell.X * cellSize + cellSize / 3, randomCell.Y * cellSize + cellSize / 3));
                items.Add(tempItem);
            }

            return items;
        }

        private List<Item> CreateItemsList(ContentManager Content, int itemCount, List<String> allItemsNames)
        {
            List<Item> items = new List<Item>(itemCount);

            for (int i = 0; i < itemCount; i++)
            {
                String randomItem = allItemsNames[Global.random.Next(allItemsNames.Count)];
                Item tempItem = (Item)Activator.CreateInstance(Type.GetType("Dungeon_Crawler." + randomItem), Content);
                items.Add(tempItem);
            }

            return items;
        }

        private List<Character> CreateEnemiesList(ContentManager Content, Map map, int cellSize, int enemyCount, List<Cell> occupiedCells, Grid grid)
        {
            List<Character> enemies = new List<Character>(enemyCount);

            for (int i = 0; i < enemyCount; i++)
            {
                Cell randomCell = GetRandomEmptyCell(map, occupiedCells, grid, true);
                occupiedCells.Add(randomCell);
                float speed = 1 / 0.9f + (float)Math.Log(enemySpeedFactor);
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

                List<Item> EnemyInventory = new List<Item>();
                if (Global.random.Next(20) > 18)  // Enemy has 10% of chance to be equipped with one item (for testing I changed it to 100%)
                {
                    EnemyInventory = CreateItemsList(Content, 1, allItemsNames);
                }

                string enemyName;
                int whichEnemyToSpawn = Global.random.Next(4);
                if (whichEnemyToSpawn == 0)
                {
                    enemyName = "Blob";

                    Character tempEnemy =
                    new Blob(_animationsBlob, cellSize, level, speed, timeBetweenActions, map, enemyName)
                    {
                        Name = BlobNamesList[Global.random.Next(BlobNamesList.Count)],
                        Position = new Vector2((randomCell.X * cellSize + cellSize / 4), (randomCell.Y * cellSize) + cellSize / 4)
                    };
                    tempEnemy.TakeItems(EnemyInventory);
                    enemies.Add(tempEnemy);
                    grid.SetCellCost(new Position(randomCell.X, randomCell.Y), 5.0f);
                }
                else if (whichEnemyToSpawn == 1)
                {
                    enemyName = "Skeleton";

                    Character tempEnemy =
                    new Skeleton(_animationsSkeleton, cellSize, level, speed, timeBetweenActions, map, enemyName)
                    {
                        Name = SkeletonNamesList[Global.random.Next(SkeletonNamesList.Count)],
                        Position = new Vector2((randomCell.X * cellSize + cellSize / 3), (randomCell.Y * cellSize) + cellSize / 3)
                    };
                    tempEnemy.TakeItems(EnemyInventory);
                    enemies.Add(tempEnemy);
                    grid.SetCellCost(new Position(randomCell.X, randomCell.Y), 5.0f);
                }
                else if (whichEnemyToSpawn == 2)
                {
                    enemyName = "Spider";

                    Character tempEnemy =
                    new Spider(_animationsSpider, cellSize, level, speed, timeBetweenActions, map, enemyName)
                    {
                        Name = SpiderNamesList[Global.random.Next(SpiderNamesList.Count)],
                        Position = new Vector2((randomCell.X * cellSize + cellSize / 3), (randomCell.Y * cellSize) + cellSize / 3)
                    };
                    tempEnemy.TakeItems(EnemyInventory);
                    enemies.Add(tempEnemy);
                    grid.SetCellCost(new Position(randomCell.X, randomCell.Y), 5.0f);
                }
                else
                {
                    enemyName = "Zombie";

                    Character tempEnemy =
                    new Zombie(_animationsZombie, cellSize, level, speed, timeBetweenActions, map, enemyName)
                    {
                        Name = ZombieNamesList[Global.random.Next(ZombieNamesList.Count)],
                        Position = new Vector2((randomCell.X * cellSize + cellSize / 3), (randomCell.Y * cellSize) + cellSize / 3)
                    };
                    tempEnemy.TakeItems(EnemyInventory);
                    enemies.Add(tempEnemy);
                    grid.SetCellCost(new Position(randomCell.X, randomCell.Y), 5.0f);
                }
            }

            return enemies;
        }

        private Map CreateMap(int mapWidth, int mapHeight, int roomCount, int roomWidth, int roomHeight)
        {
            IMapCreationStrategy<Map> mapCreationStrategy =
                new RandomRoomsMapCreationStrategy<Map>(mapWidth, mapHeight, roomCount, roomWidth, roomHeight);
            Map map = Map.Create(mapCreationStrategy);
            return map;
        }

        private Map CreateBossMap(int mapWidth, int mapHeight)
        {
            IMapCreationStrategy<Map> mapCreationStrategy =
                new BorderOnlyMapCreationStrategy<Map>(mapWidth, mapHeight);
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
                if (player.CurrentMapLevel % 3 == 1)
                {
                    CreateBossLevel();
                }
                else
                {
                    CreateNormalLevel();
                }

                player.CurrentMapLevel++;
                player.currentActionState = Player.ActionState.Standing;
                levels[player.CurrentMapLevel].addPlayer(player);
                Vector2 newPlayerPosition = levels[player.CurrentMapLevel].GetRandomEmptyCell();
                player.Position = newPlayerPosition;
                Global.Camera.CenterOn(player.Center);
                levels[player.CurrentMapLevel - 1] = null;
            }
        }

        private Cell GetRandomEmptyCell(Map map, List<Cell> occupiedCells, Grid grid, bool canBeSurroundedByWallsFromTwoSides)
        {
            int x, y;
            Cell tempCell;
            Cell LeftTempCell;
            Cell UpTempCell;
            Cell RightTempCell;
            Cell BottomTempCell;
            if (canBeSurroundedByWallsFromTwoSides)
            {
                do
                {
                    x = Global.random.Next(map.Width);
                    y = Global.random.Next(map.Height);
                    tempCell = map.GetCell(x, y);
                } while (!tempCell.IsWalkable || grid.GetCellCost(new Position(x, y)) > 1.0f || occupiedCells.Contains(tempCell));
            }
            else
            {
                do
                {
                    x = Global.random.Next(1, map.Width);
                    y = Global.random.Next(1, map.Height);
                    tempCell = map.GetCell(x, y);

                    if (x + 1 < map.Width)
                    {
                        RightTempCell = map.GetCell(x + 1, y);
                    }
                    else
                    {
                        RightTempCell = tempCell;
                    }

                    if (y + 1 < map.Height)
                    {
                        BottomTempCell = map.GetCell(x, y + 1);
                    }
                    else
                    {
                        BottomTempCell = tempCell;
                    }
                    LeftTempCell = map.GetCell(x - 1, y);
                    UpTempCell = map.GetCell(x, y - 1);

                } while (!tempCell.IsWalkable
                 || (!LeftTempCell.IsWalkable && (!RightTempCell.IsWalkable || !UpTempCell.IsWalkable || !BottomTempCell.IsWalkable))
                 || (!RightTempCell.IsWalkable && (!LeftTempCell.IsWalkable || !UpTempCell.IsWalkable || !BottomTempCell.IsWalkable))
                 || (!UpTempCell.IsWalkable && (!BottomTempCell.IsWalkable || !LeftTempCell.IsWalkable || !RightTempCell.IsWalkable))
                 || (!BottomTempCell.IsWalkable && (!UpTempCell.IsWalkable || !LeftTempCell.IsWalkable || !RightTempCell.IsWalkable))
                 || grid.GetCellCost(new Position(x, y)) > 1.0f || occupiedCells.Contains(tempCell));
            }
            return tempCell;
        }
    }
}
