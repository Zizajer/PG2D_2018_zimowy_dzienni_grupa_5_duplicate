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
            
            Texture2D actualSpriteTexture = player._animationManager._animation.Texture;
            Rectangle sourceRectangle = player._animationManager.getCurrentFrameRectangle();

            Texture2D cropTexture = new Texture2D(GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
            Color[] cropTexturedata = new Color[sourceRectangle.Width * sourceRectangle.Height];
            actualSpriteTexture.GetData(0, sourceRectangle, cropTexturedata, 0, cropTexturedata.Length);
            cropTexture.SetData(cropTexturedata);

            Animation actualSpriteAnimation = player._animationManager._animation;

            Rectangle playerRectangle=
                new Rectangle((int)player.Position.X, (int)player.Position.Y,
                actualSpriteAnimation.FrameWidth, actualSpriteAnimation.FrameHeight);

            Rectangle obstacle1Rectangle = obstacle1.getRectangle();

            areColliding = false;

            if (playerRectangle.Intersects(obstacle1Rectangle))
            {
                if (Collision.IntersectPixels(playerRectangle, cropTexturedata,
                                    obstacle1Rectangle, obstacle1.TextureData))
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
            obstacle1.Draw(spriteBatch);
            player.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
