﻿using Microsoft.Xna.Framework;
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
        int playerCurrentLevel = 0;
        private int newMapWidth = 16;
        private int newMapHeight = 10;
        private int newMapRoomCount = 100;
        private int newMapRoomWidth = 3;
        private int newMapRoomHeight = 3;

        public LevelManager(ContentManager Content)
        {
            levels = new List<Level>();

            Map map = CreateMap(newMapWidth, newMapHeight, newMapRoomCount, newMapRoomWidth, newMapRoomHeight);
            List<Cell> occupiedCells = new List<Cell>();

            incrementMapParameters(2);
            randomizeRooms(10,3,3);

            Texture2D floor = LoadFloorTexture(Content);
            Texture2D wall = LoadWallTexture(Content);

            List<Enemy> enemies = CreateEnemiesList(Content, map, floor.Width, 5, occupiedCells);
            List<Item> items = CreateItemsList(Content, map, floor.Width, 3, occupiedCells);
            List<Obstacle> obstacles = CreateObstaclesList(Content, map, floor.Width, 3, occupiedCells);

            Global.Camera.setParams(map.Width, map.Height, floor.Width);

            Cell randomCell = GetRandomEmptyCell(map, occupiedCells);
            Global.Camera.CenterOn(randomCell);

            this.player =
                new Player(Content, floor.Width)
                {
                    Position = new Vector2((randomCell.X * floor.Width + floor.Width / 3), (randomCell.Y * floor.Width) + floor.Width / 3)
                };

            Global.Gui = new GUI(player, Content.Load<SpriteFont>("fonts/Default"));
            //Global.CombatManager = new CombatManager(player, enemies); 

            Level level = new Level(map, enemies, items, obstacles, floor, wall, player, occupiedCells);

            this.levels.Add(level);
        }
        public void nextPlayerLevel()
        {
            playerCurrentLevel++;
        }
        public void incrementMapParameters(int increaseValue)
        {
            newMapWidth = newMapWidth+ increaseValue;
            newMapHeight = newMapWidth+ increaseValue;
        }
        public void randomizeRooms(int lowestNumberOfRoomsAddidtionsTries, int lowestRoomWidth, int lowestRoomHeight)
        {
            newMapRoomCount = Global.random.Next(100) + lowestNumberOfRoomsAddidtionsTries;
            newMapRoomWidth = Global.random.Next(10) + lowestRoomWidth;
            newMapRoomHeight = Global.random.Next(10) + lowestRoomHeight;
        }
        public void CreateLevel(ContentManager Content)
        {
            Map map = CreateMap(newMapWidth, newMapHeight, newMapRoomCount, newMapRoomWidth, newMapRoomHeight);
            Texture2D floor = LoadFloorTexture(Content);
            Texture2D wall = LoadWallTexture(Content);

            List<Cell> occupiedCells = new List<Cell>();

            List<Enemy> enemies = CreateEnemiesList(Content, map, floor.Width, 5, occupiedCells);
            List<Item> items = CreateItemsList(Content, map, floor.Width, 3, occupiedCells);
            List<Obstacle> obstacles = CreateObstaclesList(Content, map, floor.Width, 3, occupiedCells);

            Global.Camera.setParams(map.Width, map.Height, floor.Width);

            Level level = new Level(map, enemies, items, obstacles, floor, wall, player, occupiedCells);

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

        private List<Obstacle> CreateObstaclesList(ContentManager Content, Map map, int cellSize, int obstacleCount, List<Cell> occupiedCells)
        {
            List<Obstacle> obstacles = new List<Obstacle>(obstacleCount);

            for (int i = 0; i < obstacleCount; i++)
            {
                Cell randomCell = GetRandomEmptyCell(map, occupiedCells);
                occupiedCells.Add(randomCell);
                Obstacle tempObstacle =
                    new Obstacle(new Vector2(randomCell.X * cellSize, randomCell.Y * cellSize), Content.Load<Texture2D>("map/obstacle1"));
                obstacles.Add(tempObstacle);
            }

            return obstacles;
        }

        private List<Item> CreateItemsList(ContentManager Content, Map map, int cellSize, int itemCount, List<Cell> occupiedCells)
        {
            List<Texture2D> allItems = new List<Texture2D>(3);
            List<String> allItemsNames = new List<String>(3);

            allItems.Add(Content.Load<Texture2D>("items/bow1"));
            allItems.Add(Content.Load<Texture2D>("items/sword1"));
            allItems.Add(Content.Load<Texture2D>("items/wand1"));

            allItemsNames.Add("Bow");
            allItemsNames.Add("Sword");
            allItemsNames.Add("Wand");

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
            levels[playerCurrentLevel].Draw(gameTime, spriteBatch);
        }

        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            levels[playerCurrentLevel].Update(gameTime, graphicsDevice);
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
