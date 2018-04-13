using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dungeon_Crawler
{
    public class Projectile:Sprite
    {
        public Vector2 Velocity { get; set; }
        public Vector2 Origin2 {
            get
            {
                return new Vector2(getRectangle().Width / 2, getRectangle().Height / 2);
            }
        }
        public float rotation;
        public bool isEnemyHit = false;
        public bool isMarkedToDelete = false;
        private float ProjectileTimer;
        private readonly float VanishDelay = 0.25f;
        int x, y;
        public Projectile(Vector2 vel, Vector2 pos, Texture2D tex,float rot) : base(pos, tex)
        {
            Velocity = vel;
            rotation = rot;
            isEnemyHit = false;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, rotation, Origin2, Size, SpriteEffects.None, Layers.Projectiles);
        }
        public virtual void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {
            Position += Velocity;
            //Vanish projectile when out of player's fov.
            if (!(Math.Abs(this.Position.X - level.player.Position.X) < Global.Camera.ViewportWidth && Math.Abs(this.Position.Y - level.player.Position.Y) < Global.Camera.ViewportHeight))
            {
                isMarkedToDelete = true;
            }
            //destroy when hit a wall
            x = (int)Math.Floor(Position.X / level.cellSize);
            y = (int)Math.Floor(Position.Y / level.cellSize);
            if (!level.map.GetCell(x, y).IsWalkable)
                isMarkedToDelete = true;

            if (Collision.isCollidingWithRocks(this, level, graphicsDevice))
                isMarkedToDelete = true;

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
