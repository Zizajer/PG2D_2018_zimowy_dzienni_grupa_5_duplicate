using Microsoft.Xna.Framework;
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
            Global.GameState = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Global.Gui = new GUI(graphics, Content);

            //Global.playerClass = Global.classes[2];
            Global.playerClass =Global.classes[Global.random.Next(Global.classes.Length)];
            levelManager = new LevelManager(Content);
            Global.levelmanager = levelManager;

            Global.Effects = new Effects(Content);

            effect1 = Content.Load<Effect>("shaders/lighteffect");
            lightMask = Content.Load<Texture2D>("shaders/lightmask");
            var pp = GraphicsDevice.PresentationParameters;
            lightsTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            mainTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);

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
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
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
            Global.Gui.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
