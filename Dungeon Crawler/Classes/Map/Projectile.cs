using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dungeon_Crawler
{
    public class Projectile : Sprite
    {
        public IPositionTargetedAttack Attack;
        public Vector2 Velocity { get; set; }
        public Vector2 OriginalPosition { get; set; }
        public float Rotation;
        public bool IsCharacterHit = false;
        private float ProjectileTimer = 0;
        private readonly float VanishDelay;
        int x, y;
        public Projectile(IPositionTargetedAttack attack, Vector2 velocity, Vector2 position, Texture2D texture, float rotation, float vanishDelay) : base(position, texture)
        {
            Attack = attack;
            OriginalPosition = position;
            Velocity = velocity;
            Rotation = rotation;
            IsCharacterHit = false;
            VanishDelay = vanishDelay;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, Origin, Size, SpriteEffects.None, Layers.Projectiles);
        }
        public void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {
            //Moving by specified velocity
            Position += Velocity;

            //Detect collision with character and notify attack about who took a hit
            foreach (Character character in level.enemies)
            {
                if (Collision.checkCollision(character.getRectangle(), character, this, graphicsDevice))
                {
                    IsCharacterHit = true;
                    Attack.Notify(character);
                }
            }
            if (Collision.checkCollision(level.player.getRectangle(), level.player, this, graphicsDevice))
            {
                IsCharacterHit = true;
                Attack.Notify(level.player);
            }

            //Vanish projectile when out of range
            if (Vector2.Distance(Center, OriginalPosition) > 3 * level.cellSize)
            {
                isMarkedToDelete = true;
            }

            //Destroy when hit a wall
            x = (int)Math.Floor(Position.X / level.cellSize);
            y = (int)Math.Floor(Position.Y / level.cellSize);
            if (!level.map.GetCell(x, y).IsWalkable)
                isMarkedToDelete = true;

            //Destroy when hit a rock
            if (Collision.isCollidingWithRocks(this, level, graphicsDevice))
                isMarkedToDelete = true;

            //Vanish projectile when hit an enemy after certain delay. (If we destroyed the projectile just after hitting first enemy, we couldn't kill a group of enemies staying nearby)
            if (IsCharacterHit)
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
