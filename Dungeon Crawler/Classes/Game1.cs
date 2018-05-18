﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dungeon_Crawler
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        LevelManager levelManager;
        bool wasGameOverSoundPlayed = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            Global.Camera.setViewports(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            Global.GameState = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            levelManager = new LevelManager(Content);
            Global.Gui.addLevelMananger(levelManager);
            Global.CombatManager = new CombatManager(levelManager);
            Global.SoundManager = new SoundManager(Content);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Global.GameState==true)
            {
                Global.Camera.Move();

                levelManager.Update(gameTime, GraphicsDevice);

                Global.Gui.Update();
                Global.CombatManager.Update();
            }
            else
            {
                if (wasGameOverSoundPlayed == false)
                {
                    Global.SoundManager.playGameOver();
                    wasGameOverSoundPlayed = true;
                }
            }
            
            
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);

            levelManager.Draw(gameTime, spriteBatch);

            Global.Gui.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
