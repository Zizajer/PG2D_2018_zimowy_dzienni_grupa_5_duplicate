using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon_Crawler
{
    public class AttackAnimation
    {
        public AnimationManager _animationManager;
        protected Dictionary <String, Animation> _animations;
        public bool isActive = true;
        public float timer = 0;
        public float creationTime;
        public float howLongShouldAnimationPlay = 0.5f;

        public AttackAnimation(ContentManager content,int x, int y, int cellSize, GameTime gameTime)
        {
            _animations = new Dictionary<string, Animation>()
                {
                    {"Exori",new Animation(content.Load<Texture2D>("spells/Exori"),3 )}
                };

            _animationManager = new AnimationManager(_animations.First().Value);
            int newx = x * cellSize + cellSize / 2 - _animationManager.getCurrentFrameRectangle().Width / 2;
            int newy = y * cellSize + cellSize / 2 - _animationManager.getCurrentFrameRectangle().Height / 2;

            creationTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _animationManager.Position = new Vector2(newx, newy);
            
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            _animationManager.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            
            if (isActive)
            {
                _animationManager.Play(_animations["Exori"]);
                _animationManager.Update(gameTime);
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                if (timer > creationTime + howLongShouldAnimationPlay)
                {
                    isActive = false;
                }
            }
        }
    }
}
