using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dungeon_Crawler
{
    public class PlayerProjectile:Sprite
    {
        public Vector2 Velocity { get; set; }
        public Vector2 OriginalPosition { get; set; }
        public float rotation;
        public bool isEnemyHit = false;
        private float ProjectileTimer=0;
        private readonly float VanishDelay = 0.25f;
        int x, y;
        public PlayerProjectile(Vector2 vel, Vector2 pos, Texture2D tex,float rot) : base(pos, tex)
        {
            OriginalPosition = pos;
            Velocity = vel;
            rotation = rot;
            isEnemyHit = false;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, rotation, Origin, Size, SpriteEffects.None, Layers.Projectiles);
        }
        public void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {
            Position += Velocity;
            //Vanish projectile when out of range
            if (Vector2.Distance(Center, OriginalPosition) > 3*level.cellSize)
            {
                isMarkedToDelete = true;
            }
            //destroy when hit a wall
            x = (int)Math.Floor(Position.X / level.cellSize);
            y = (int)Math.Floor(Position.Y / level.cellSize);
            if (!level.map.GetCell(x, y).IsWalkable)
                isMarkedToDelete = true;
            //destroy when hit a rock
            if (Collision.isCollidingWithRocks(this, level, graphicsDevice))
                isMarkedToDelete = true;

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
