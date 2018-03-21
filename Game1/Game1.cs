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
        Texture2D blockTexture;
        Vector2 blockPosition;
        Color[] blockTextureData;


        bool personHit = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
            blockPosition = new Vector2(250, 200);

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
            blockTexture = Content.Load<Texture2D>("obstacle1");

            blockTextureData =
                new Color[blockTexture.Width * blockTexture.Height];
            blockTexture.GetData(blockTextureData);

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

            Rectangle spriteRectangle=
                new Rectangle((int)player.Position.X, (int)player.Position.Y,
                actualSpriteAnimation.FrameWidth, actualSpriteAnimation.FrameHeight);

            Rectangle blockRectangle = new Rectangle((int)blockPosition.X, (int)blockPosition.Y, blockTexture.Width, blockTexture.Height);

            personHit = false;

            if (spriteRectangle.Intersects(blockRectangle))
            {
                personHit = true;
                if (Collision.IntersectPixels(spriteRectangle, cropTexturedata,
                                    blockRectangle, blockTextureData))
                {
                    personHit = true;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (personHit)
            {
                collision = "Collision";

            }
            else
            {
                collision = "No Collision";
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(font,collision, new Vector2(400, 100), Color.Black);
            spriteBatch.Draw(blockTexture, blockPosition, Color.White);
            player.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
