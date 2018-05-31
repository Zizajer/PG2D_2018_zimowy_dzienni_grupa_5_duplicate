using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dungeon_Crawler
{
    public class Projectile : Sprite
    {
        public IPositionTargetedAttack Attack;
        public Character Attacker;
        public Vector2 Velocity { get; set; }
        public Vector2 OriginalPosition { get; set; }
        public float Rotation;
        public bool IsCharacterHit = false;
        private float ProjectileTimer = 0;
        private readonly float VanishDelay;
        int x, y;
        public Projectile(IPositionTargetedAttack attack, Character attacker, Vector2 velocity, Vector2 position, Texture2D texture, float rotation, float vanishDelay) : base(position, texture)
        {
            Attack = attack;
            Attacker = attacker;
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

            //Detect collision with character and notify parent attack about who took a hit. 
            //Ignore collision with projectile creator. (Character shoudn't hit himself)
            foreach (Character character in level.enemies)
            {
                if (Collision.checkCollision(character.getRectangle(), character, this, graphicsDevice))
                {
                    if (character != Attacker)
                    {
                        IsCharacterHit = true;
                        Attack.Notify(character);
                    }
                }
            }
            if (Collision.checkCollision(level.player.getRectangle(), level.player, this, graphicsDevice))
            {
                if (level.player != Attacker)
                {
                    IsCharacterHit = true;
                    Attack.Notify(level.player);
                }
            }

            //Vanish projectile when out of range
            //Make range one of attack's parameter
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
