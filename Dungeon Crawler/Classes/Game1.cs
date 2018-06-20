﻿using Microsoft.Xna.Framework;
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
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();
            /*
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            */
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            Global.Camera.setViewports(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            Global.Camera.setZoom(1.5f);
            Global.GameStates = 0;
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
            Global.DrawManager = new DrawManager(Content, this);
            Global.StatsAllocationSystem = new StatsAllocationSystem(Content, levelManager);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Global.GameStates == 0)
            {
                Global.DrawManager.UpdateMainMenu(gameTime);
            }
            else if (Global.GameStates == 1)
            {
                Global.DrawManager.UpdateChooseHeroMenu(gameTime);
            }
            else if (Global.GameStates == 2)
            {
                Global.DrawManager.UpdateHelpMenu(gameTime);
            }
            else if (Global.GameStates == 3)
            {
                Global.DrawManager.UpdateAboutMenu(gameTime);
            }
            else if (Global.GameStates == 4)
            {
                Global.Camera.Move();
                levelManager.Update(gameTime, GraphicsDevice);
                Global.Gui.Update(gameTime);
                Global.CombatManager.Update();
            }
            else if (Global.GameStates == 5)
            {
                Global.StatsAllocationSystem.Update(gameTime);
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
            if (Global.GameStates == 0)
            {
                Global.DrawManager.DrawMainMenu(spriteBatch, GraphicsDevice, gameTime);
            }
            else if (Global.GameStates == 1)
            {
                Global.DrawManager.DrawChooseHeroMenu(spriteBatch, GraphicsDevice, gameTime);
            }
            else if (Global.GameStates == 2)
            {
                Global.DrawManager.DrawHelpMenu(spriteBatch, GraphicsDevice, gameTime);
            }
            else if (Global.GameStates == 3)
            {
                Global.DrawManager.DrawAboutMenu(spriteBatch, GraphicsDevice, gameTime);
            }
            else if (Global.GameStates == 4)
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
            else if (Global.GameStates == 5)
            {
                Global.StatsAllocationSystem.Draw(spriteBatch, GraphicsDevice, gameTime);
            }
            else // we draw 4 but we dont update
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
