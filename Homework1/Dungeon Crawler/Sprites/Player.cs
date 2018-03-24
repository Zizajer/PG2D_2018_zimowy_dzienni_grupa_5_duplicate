using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dungeon_Crawler.Sprites
{ public class Player : Sprite
    {
        public Player(Texture2D texture) : base(texture)
        {
        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            Move();
            

            foreach (var sprite in sprites)
            {
                if (sprite == this)
                    continue;

                if (this.velocity.X > 0 && this.isTouchingLeft(sprite) ||
                    this.velocity.X < 0 && this.isTouchingRight(sprite))
                    this.velocity.X = 0;

                if (this.velocity.Y > 0 && this.isTouchingTop(sprite) ||
                    this.velocity.Y < 0 && this.isTouchingBottom(sprite))
                    this.velocity.Y = 0;
                    

            }

            if (this.circle.Intersects(new Rectangle(this.Rectangle.X + (int)speed, this.Rectangle.Y, this.Rectangle.Width, this.Rectangle.Height)))
            {
                this.velocity.X = 0;
                this.position.X -= 1;
            }

            if (this.circle.Intersects(new Rectangle(this.Rectangle.X - (int)speed, this.Rectangle.Y, this.Rectangle.Width, this.Rectangle.Height)))
            {
                this.velocity.X = 0;
                this.position.X += 1;
            }

            if (this.circle.Intersects(new Rectangle(this.Rectangle.X , this.Rectangle.Y + (int)speed, this.Rectangle.Width, this.Rectangle.Height)))
            {
                this.velocity.Y = 0;
                this.position.Y -= 1;

            }

            if (this.circle.Intersects(new Rectangle(this.Rectangle.X, this.Rectangle.Y - (int)speed, this.Rectangle.Width, this.Rectangle.Height)))
            {
                this.velocity.Y = 0;
                this.position.Y += 1;
            }


            position += velocity;

            velocity = Vector2.Zero;

        }

        public void Move()
        {
            if (Keyboard.GetState().IsKeyDown(input.left))
                velocity.X = -speed;
            else if (Keyboard.GetState().IsKeyDown(input.right))
                velocity.X = speed;
            else if(Keyboard.GetState().IsKeyDown(input.up))
                velocity.Y = -speed;
            else if(Keyboard.GetState().IsKeyDown(input.down))
                velocity.Y = speed;

        }
    }
}
