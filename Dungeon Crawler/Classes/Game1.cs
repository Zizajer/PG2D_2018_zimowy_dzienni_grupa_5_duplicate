using Dungeon_Crawler.Classes.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

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

        private Texture2D mag;
        private Texture2D warrior;
        private Texture2D hunter;
        private Texture2D button;
        private SpriteFont buttonFont;
        private String choosenHero;

        private List<Button> _gameComponents;

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
            Global.GameState = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Global.Gui = new GUI(graphics, Content);

            //Global.playerClass = Global.classes[2];
            Global.playerClass = Global.classes[Global.random.Next(Global.classes.Length)];
            levelManager = new LevelManager(Content);
            Global.levelmanager = levelManager;
            Global.Gui.lm = levelManager;
            Global.Effects = new Effects(Content);
            Global.IsGameStarted = false;

            effect1 = Content.Load<Effect>("shaders/lighteffect");
            lightMask = Content.Load<Texture2D>("shaders/lightmask");
            var pp = GraphicsDevice.PresentationParameters;
            lightsTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            mainTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);

            Global.CombatManager = new CombatManager(levelManager);
            Global.SoundManager = new SoundManager(Content);

            mag = Content.Load<Texture2D>("Arts/magini2");
            warrior = Content.Load<Texture2D>("Arts/wojownik2");
            hunter = Content.Load<Texture2D>("Arts/rangerka2");
            button = Content.Load<Texture2D>("Controls/button");
            buttonFont = Content.Load<SpriteFont>("fonts/Default");

            var magButton = new Button(button, buttonFont)
            {
                Position = new Vector2(50, 350),
                Text = "Mag",
            };

            magButton.Click += magButton_Click;

            var warriorButton = new Button(button, buttonFont)
            {
                Position = new Vector2(575, 300),
                Text = "Warrior",
            };

            warriorButton.Click += warriorButton_Click;

            var hunterButton = new Button(button, buttonFont)
            {
                Position = new Vector2(1100, 350),
                Text = "Hunter",
            };

            hunterButton.Click += hunterButton_Click;

            var quitButton = new Button(button, buttonFont)
            {
                Position = new Vector2(575, 625),
                Text = "Quit",
            };

            quitButton.Click += quitButton_Click;


            _gameComponents = new List<Button>()
            {
                magButton,
                warriorButton,
                hunterButton,
                quitButton,
            };
        }

        void hunterButton_Click(object sender, EventArgs e)
        {
            Global.playerClass = "Ranger";
            Global.IsGameStarted = true;
        }

        void warriorButton_Click(object sender, EventArgs e)
        {
            Global.playerClass = "Warrior";
            Global.IsGameStarted = true;
        }

        void magButton_Click(object sender, EventArgs e)
        {
            Global.playerClass = "Mage";
            Global.IsGameStarted = true;
        }

        void quitButton_Click(object sender, System.EventArgs e)
        {
            Exit();
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Global.IsGameStarted) { 
                if (Global.GameState == true)
                {
                    Global.Camera.Move();

                    levelManager.Update(gameTime, GraphicsDevice);

                    Global.Gui.Update(gameTime);
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
            }
            else
            {
                foreach (Button button in _gameComponents)
                    button.Update(gameTime);
            }
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (Global.IsGameStarted)
            {
                GraphicsDevice.SetRenderTarget(lightsTarget);
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Global.Camera.TranslationMatrix);
                //draw light mask where there should be torches etc...

                Vector2 playerPos = levelManager.player.Center;
                playerPos.X -= lightMask.Width / 2;
                playerPos.Y -= lightMask.Height / 2;
                spriteBatch.Draw(lightMask, playerPos, Color.White);

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
            else {
                GraphicsDevice.Clear(Color.CornflowerBlue);

                spriteBatch.Begin();

                spriteBatch.DrawString(buttonFont, "Choose Character", new Vector2(550, 10), Color.Black);

                spriteBatch.Draw(mag, new Rectangle(50, 50, mag.Width, mag.Height), Color.White);

                spriteBatch.Draw(warrior, new Rectangle(575, 50, warrior.Width, warrior.Height), Color.White);

                spriteBatch.Draw(hunter, new Rectangle(1100, 50, hunter.Width, hunter.Height), Color.White);

                foreach (Button button in _gameComponents)
                {
                    button.Draw(gameTime, spriteBatch);
                }

                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
