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
        private Sprite sprite;
        private SpriteFont font;
        private String collision;
        Texture2D blockTexture;
        Vector2 blockPosition;
        Color[] blockTextureData;
        Color[] spriteTextureData;

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
            sprite =
                new Sprite(animations){
                    Position = new Vector2(200, 200)
                };
            font = Content.Load<SpriteFont>("Default");
            blockTexture = Content.Load<Texture2D>("RedBlock");

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

            sprite.Update(gameTime, sprite);

            Texture2D actualSpriteTexture = sprite._animationManager._animation.Texture;
            spriteTextureData =
               new Color[actualSpriteTexture.Width * actualSpriteTexture.Height];
            actualSpriteTexture.GetData(spriteTextureData);

            Rectangle spriteRectangle =
                new Rectangle((int)sprite.Position.X, (int)sprite.Position.Y,
                actualSpriteTexture.Width/3, actualSpriteTexture.Height);

            Rectangle blockRectangle = new Rectangle((int)blockPosition.X, (int)blockPosition.Y, blockTexture.Width, blockTexture.Height);

            personHit = false;

            if (spriteRectangle.Intersects(blockRectangle))
            {
                if (Collision.IntersectPixels(spriteRectangle, spriteTextureData,
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
            sprite.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
