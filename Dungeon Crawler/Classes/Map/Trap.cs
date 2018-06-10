using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dungeon_Crawler
{
    public class Trap : Projectile
    {
        public Trap(IPositionTargetedAttack attack, Character attacker, Vector2 position, Texture2D texture) : base(position, texture)
        {
            Attack = attack;
            Attacker = attacker;
            Position = position;
            IsCharacterHit = false;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 1f, Origin, Size, SpriteEffects.None, Layers.Projectiles);
        }
        public override void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {
            x = (int)Math.Floor(Position.X / level.cellSize);
            y = (int)Math.Floor(Position.Y / level.cellSize);
            //Detect collision with character and notify parent attack about who took a hit. 
            //Ignore collision with projectile creator. (Character shoudn't hit himself)
            foreach (Character character in level.enemies)
            {
                if (Collision.checkCollision(character.getRectangle(), character, this, graphicsDevice))
                {
                    if (character != Attacker)
                    {
                        isMarkedToDelete = true;
                        Attack.Notify(character);
                    }
                }
            }
            if (Collision.checkCollision(level.player.getRectangle(), level.player, this, graphicsDevice))
            {
                if (level.player != Attacker)
                {
                    isMarkedToDelete = true;
                    Attack.Notify(level.player);
                }
            }
        }
    }
}
