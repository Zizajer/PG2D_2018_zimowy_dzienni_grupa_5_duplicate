using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dungeon_Crawler
{
    public class Projectile
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Origin {
            get
            {
                Rectangle rec = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
                return new Vector2(rec.Width / 2, rec.Height / 2);
            }
            set { }
        }
        public float rotation;
        public bool isEnemyHit = false;
        public bool isMarkedToDelete = false;
        private float ProjectileTimer;
        private readonly float VanishDelay = 0.25f;

        public Color[] TextureData { get; }
        public Projectile(Vector2 vel, Vector2 pos, Texture2D tex,float rot)
        {
            Position = pos;
            Texture = tex;
            Velocity = vel;
            rotation = rot;
            isEnemyHit = false;
            TextureData =
               new Color[Texture.Width * Texture.Height];
            Texture.GetData(TextureData);
        }
        public Rectangle getRectangle()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, rotation, Origin, 1f, SpriteEffects.None, Layers.Projectiles);
        }
        public virtual void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {
            this.Position += this.Velocity;
            //Vanish projectile when out of player's fov.
            if (!(Math.Abs(this.Position.X - level.player.Position.X) < Global.Camera.ViewportWidth && Math.Abs(this.Position.Y - level.player.Position.Y) < Global.Camera.ViewportHeight))
            {
                isMarkedToDelete = true;
            }

            //Vanish projectile when hit an enemy after certain delay. (If we destroyed the projectile just after hitting first enemy, we couldn't kill a group of enemies staying nearby)
            if (isEnemyHit)
            {
                ProjectileTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (ProjectileTimer > VanishDelay)
                {
                    isMarkedToDelete = true;
                }
            }
        }
    }
}
