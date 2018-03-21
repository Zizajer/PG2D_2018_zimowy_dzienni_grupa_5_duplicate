using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Game1
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Player player;
        private Obstacle obstacle1;

        private SpriteFont font;
        private String collision;
        bool areColliding;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            var animations = new Dictionary<string, Animation>()
            {
                {"WalkUp",new Animation(Content.Load<Texture2D>("Walkingup"),3 )},
                {"WalkDown",new Animation(Content.Load<Texture2D>("WalkingDown"),3 )},
                {"WalkLeft",new Animation(Content.Load<Texture2D>("WalkingLeft"),3 )},
                {"WalkRight",new Animation(Content.Load<Texture2D>("WalkingRight"),3 )}
            };
            player =
                new Player(animations){
                    Position = new Vector2(200, 200)
                };
            obstacle1 = 
                new Obstacle(new Vector2(250, 200), Content.Load<Texture2D>("obstacle1"));

            font = Content.Load<SpriteFont>("Default");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player.Update(gameTime, player);

            areColliding = false;
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

            spriteBatch.Begin();
            obstacle1.Draw(spriteBatch);
            player.Draw(spriteBatch);
            spriteBatch.DrawString(font, collision, new Vector2(400, 100), Color.Black);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
