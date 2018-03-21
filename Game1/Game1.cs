using Dungeon_Crawler;
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
        private SpriteFont font;
        private String collision;
        private Obstacle obstacle;

        Color[] obstacleTextureData;
        Color[] playerTextureData;
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
            font = Content.Load<SpriteFont>("Default");
            obstacle = new Obstacle(new Vector2(250, 200), Content.Load<Texture2D>("obstacle1"));
            obstacleTextureData =
                new Color[obstacle.Texture.Width * obstacle.Texture.Height];
            obstacle.Texture.GetData(obstacleTextureData);

        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player.Update(gameTime, player);

            Texture2D actualPlayerTexture = player._animationManager.getCurrentAnimationTexture();
            
            playerTextureData =
               new Color[actualPlayerTexture.Width * actualPlayerTexture.Height];
            actualPlayerTexture.GetData(playerTextureData);

            Animation actualPlayerAnimation = player._animationManager._animation;

            Rectangle playerRectangle = 
                new Rectangle((int)player.Position.X, (int)player.Position.Y,
                actualPlayerAnimation.FrameWidth, actualPlayerAnimation.FrameHeight);

            Rectangle obstacleRectangle = new Rectangle((int)obstacle.Position.X, (int)obstacle.Position.Y, obstacle.Texture.Width, obstacle.Texture.Height);

            areColliding = false;

            if (playerRectangle.Intersects(obstacleRectangle))
            {
                if (Collision.IntersectPixels(playerRectangle, playerTextureData,
                                    obstacleRectangle, obstacleTextureData))
                {
                    areColliding = true;
                }
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

            spriteBatch.DrawString(font,collision, new Vector2(400, 100), Color.Black);
            spriteBatch.Draw(obstacle.Texture, obstacle.Position, Color.White);
            player.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
