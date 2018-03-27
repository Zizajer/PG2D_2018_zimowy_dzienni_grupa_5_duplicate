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
        List<Enemy> enemies;
        private Obstacle obstacle1;

        //map
        private Texture2D floor;
        private Texture2D wall;
        private int cellSize;
        private Map map;
        List<Item> items;
        //map

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
            IMapCreationStrategy<Map> mapCreationStrategy =
                new RandomRoomsMapCreationStrategy<Map>(16, 10, 100, 3, 3);
            map = Map.Create(mapCreationStrategy);
            camera = new CameraManager(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            random = new Random();
            items = new List<Item>();
            enemies = new List<Enemy>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            floor = Content.Load<Texture2D>("map/Floor");
            wall = Content.Load<Texture2D>("map/Wall");
            cellSize = floor.Width;
            camera.setParams(map.Width, map.Height, cellSize);

            Cell randomCell = GetRandomEmptyCell(map);
            items.Add(new Item(new Vector2(randomCell.X * cellSize, randomCell.Y * cellSize), Content.Load<Texture2D>("items/bow1"), "Bow"));
            randomCell = GetRandomEmptyCell(map);
            items.Add(new Item(new Vector2(randomCell.X * cellSize, randomCell.Y * cellSize), Content.Load<Texture2D>("items/sword1"), "Sword"));
            randomCell = GetRandomEmptyCell(map);
            items.Add(new Item(new Vector2(randomCell.X * cellSize, randomCell.Y * cellSize), Content.Load<Texture2D>("items/wand1"), "Wand"));

            var animations = new Dictionary<string, Animation>()
            {
                {"WalkUp",new Animation(Content.Load<Texture2D>("player/Walkingup"),3 )},
                {"WalkDown",new Animation(Content.Load<Texture2D>("player/WalkingDown"),3 )},
                {"WalkLeft",new Animation(Content.Load<Texture2D>("player/WalkingLeft"),3 )},
                {"WalkRight",new Animation(Content.Load<Texture2D>("player/WalkingRight"),3 )}
            };

            randomCell = GetRandomEmptyCell(map);
            camera.CenterOn(randomCell);
            player =
                new Player(animations,camera,cellSize)
                {
                    Position = new Vector2((randomCell.X*cellSize+cellSize/3), (randomCell.Y*cellSize)+ cellSize/3)
                };
            var animations2 = new Dictionary<string, Animation>()
                {
                    {"WalkUp",new Animation(Content.Load<Texture2D>("enemy/EnemyWalkingup"),3 )},
                    {"WalkDown",new Animation(Content.Load<Texture2D>("enemy/EnemyWalkingDown"),3 )},
                    {"WalkLeft",new Animation(Content.Load<Texture2D>("enemy/EnemyWalkingLeft"),3 )},
                    {"WalkRight",new Animation(Content.Load<Texture2D>("enemy/EnemyWalkingRight"),3 )}
                };
            for (int i = 0; i < 5; i++)
            {
                randomCell = GetRandomEmptyCell(map);
                float speed = (random.Next(2) + 1) / 0.7f;
                float timeBetweenActions = (random.Next(2))+1 / 0.7f;
                Enemy tempEnemy =
                    new Enemy(animations2, cellSize, speed, timeBetweenActions)
                    {
                        Position = new Vector2((randomCell.X * cellSize + cellSize / 3), (randomCell.Y * cellSize) + cellSize / 3)
                    };
                enemies.Add(tempEnemy);
            }

            randomCell = GetRandomEmptyCell(map);
            obstacle1 = 
                new Obstacle(new Vector2(randomCell.X * cellSize, randomCell.Y * cellSize), Content.Load<Texture2D>("map/obstacle1"));

            font = Content.Load<SpriteFont>("fonts/Default");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player.Update(gameTime, map);
            foreach(Enemy enemy in enemies)
            {
                enemy.Update(gameTime, map);
            }
            camera.Move();
            areColliding = false;
            Item[] itemArray = items.ToArray();
            for(int i=0; i<items.Count;i++)
            {
                if (Collision.checkCollision(player, itemArray[i], GraphicsDevice))
                {
                    player.inventory.Add(itemArray[i]);
                    items.Remove(itemArray[i]);
                }
            }

            if (Collision.checkCollision(player, obstacle1, GraphicsDevice))
            {
                areColliding = true;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (areColliding)
            {
                collision = "Collision";
            }
            else
            {
                collision = "No Collision";
            }

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.TranslationMatrix);

            foreach (Cell cell in map.GetAllCells())
            {
                var position = new Vector2(cell.X * cellSize, cell.Y * cellSize);
                if (cell.IsWalkable)
                {
                    spriteBatch.Draw(floor, position, null, Color.White, 0.0f, Vector2.One, 1.0f, SpriteEffects.None, 0.9f);
                }
                else
                {
                    spriteBatch.Draw(wall, position, null, Color.White, 0.0f, Vector2.One, 1.0f, SpriteEffects.None, 0.9f);
                }
            }
            foreach (var item in items)
            {
                item.Draw(spriteBatch);
            }

            obstacle1.Draw(spriteBatch);
            player.Draw(spriteBatch);
            foreach (var enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }

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
