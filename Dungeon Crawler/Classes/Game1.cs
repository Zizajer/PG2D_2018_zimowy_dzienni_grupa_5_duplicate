using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dungeon_Crawler
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        LevelManager levelManager;
        bool wasGameOverSoundPlayed = false;

        public static Texture2D lightMask;
        public static Effect effect1;
        RenderTarget2D lightsTarget;
        RenderTarget2D mainTarget;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            Global.Camera.setViewports(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            Global.Camera.setZoom(1.5f);
            Global.CurrentGameState = 0;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Global.Gui = new GUI(graphics, Content);
            levelManager = new LevelManager(Content);
            Global.levelmanager = levelManager;
            Global.Gui.lm = levelManager;
            Global.Effects = new Effects(Content);

            effect1 = Content.Load<Effect>("shaders/lighteffect");
            lightMask = Content.Load<Texture2D>("shaders/lightmask");
            var pp = GraphicsDevice.PresentationParameters;
            lightsTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            mainTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);

            Global.CombatManager = new CombatManager(levelManager);
            Global.SoundManager = new SoundManager(Content);
            Global.DrawManager = new DrawManager(Content, this, levelManager, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            Global.StatsAllocationSystem = new StatsAllocationSystem(Content, levelManager, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            Global.SoundManager.playMenuSong();
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Global.CurrentGameState == Global.Gamestates.isMainMenu)
            {
                Global.DrawManager.UpdateMainMenu(gameTime);
            }
            else if (Global.CurrentGameState == Global.Gamestates.isHeroChooseMenu)
            {
                Global.DrawManager.UpdateChooseHeroMenu(gameTime);
            }
            else if (Global.CurrentGameState == Global.Gamestates.isClassSpecificMenu)
            {
                Global.DrawManager.UpdateClassSpecificMenu(gameTime);
            }
            else if (Global.CurrentGameState == Global.Gamestates.isHelpMenu)
            {
                Global.DrawManager.UpdateHelpMenu(gameTime);
            }
            else if (Global.CurrentGameState == Global.Gamestates.isAboutMenu)
            {
                Global.DrawManager.UpdateAboutMenu(gameTime);
            }
            else if (Global.CurrentGameState == Global.Gamestates.isGameActive)
            {
                Global.StatsAllocationSystem.PointsUpdate(gameTime);
                Global.Camera.Move();
                levelManager.Update(gameTime, GraphicsDevice);
                Global.Gui.Update(gameTime);
                Global.CombatManager.Update();
            }
            else if (Global.CurrentGameState == Global.Gamestates.isStatsMenu)
            {
                Global.StatsAllocationSystem.Update(gameTime);
            }
            else
            {
                if (wasGameOverSoundPlayed == false)
                {
                    //Global.SoundManager.playerDead.Play();
                    Global.SoundManager.gameover.Play();
                    wasGameOverSoundPlayed = true;
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (Global.CurrentGameState == Global.Gamestates.isMainMenu)
            {
                Global.DrawManager.DrawMainMenu(spriteBatch, GraphicsDevice, gameTime);
            }
            else if (Global.CurrentGameState == Global.Gamestates.isHeroChooseMenu)
            {
                Global.DrawManager.DrawChooseHeroMenu(spriteBatch, GraphicsDevice, gameTime);
            }
            else if (Global.CurrentGameState == Global.Gamestates.isClassSpecificMenu)
            {
                Global.DrawManager.DrawClassSpecificMenu(spriteBatch, GraphicsDevice, gameTime);
            }
            else if (Global.CurrentGameState == Global.Gamestates.isHelpMenu)
            {
                Global.DrawManager.DrawHelpMenu(spriteBatch, GraphicsDevice, gameTime);
            }
            else if (Global.CurrentGameState == Global.Gamestates.isAboutMenu)
            {
                Global.DrawManager.DrawAboutMenu(spriteBatch, GraphicsDevice, gameTime);
            }
            else if (Global.CurrentGameState == Global.Gamestates.isGameActive)
            {
                GraphicsDevice.SetRenderTarget(lightsTarget);
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Global.Camera.TranslationMatrix);
                //draw light mask where there should be torches etc...

                Vector2 playerPos = levelManager.player.Center;
                float scale = 1 / ((levelManager.player.CurrentMapLevel + 1) * 0.05f + 0.5f);
                if (scale < 0.5f) scale = 0.5f;
                playerPos.X -= lightMask.Width * scale / 2;
                playerPos.Y -= lightMask.Height * scale / 2;
                //spriteBatch.Draw(lightMask, playerPos, Color.White);
                spriteBatch.Draw(lightMask, playerPos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);

                spriteBatch.End();

                GraphicsDevice.SetRenderTarget(mainTarget);
                GraphicsDevice.Clear(Color.Transparent);

                levelManager.Draw(gameTime, spriteBatch);

                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.Black);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                effect1.Parameters["lightMask"].SetValue(lightsTarget);
                effect1.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Draw(mainTarget, Vector2.Zero, Color.White);
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);
                Global.Gui.Draw(spriteBatch, gameTime);
                spriteBatch.End();
            }
            else if (Global.CurrentGameState == Global.Gamestates.isStatsMenu)
            {
                Global.StatsAllocationSystem.Draw(spriteBatch, GraphicsDevice, gameTime);
            }
            else // we draw isGameActive but we dont update
            {
                GraphicsDevice.SetRenderTarget(lightsTarget);
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Global.Camera.TranslationMatrix);
                //draw light mask where there should be torches etc...

                Vector2 playerPos = levelManager.player.Center;
                float scale = 1 / ((levelManager.player.CurrentMapLevel + 1) * 0.05f + 0.5f);
                if (scale < 0.5f) scale = 0.5f;
                playerPos.X -= lightMask.Width * scale / 2;
                playerPos.Y -= lightMask.Height * scale / 2;
                //spriteBatch.Draw(lightMask, playerPos, Color.White);
                spriteBatch.Draw(lightMask, playerPos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);

                spriteBatch.End();

                GraphicsDevice.SetRenderTarget(mainTarget);
                GraphicsDevice.Clear(Color.Transparent);

                levelManager.Draw(gameTime, spriteBatch);

                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.Black);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                effect1.Parameters["lightMask"].SetValue(lightsTarget);
                effect1.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Draw(mainTarget, Vector2.Zero, Color.White);
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);
                Global.Gui.Draw(spriteBatch, gameTime);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }
    }
}
