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
        public float howLongShouldAnimationPlay = 0.5f;
        public string AttackName;

        public AttackAnimation(ContentManager content, String attackName, String animationName, int x, int y, int cellSize)
        {
            AttackName = attackName;

            _animations = new Dictionary<string, Animation>()
                {
                    {attackName, new Animation(content.Load<Texture2D>("spells/" + animationName),3 )}
                };

            _animationManager = new AnimationManager(_animations.First().Value);
            int newx = x * cellSize + cellSize / 2 - _animationManager.getCurrentFrameRectangle().Width / 2;
            int newy = y * cellSize + cellSize / 2 - _animationManager.getCurrentFrameRectangle().Height / 2;

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
                _animationManager.Play(_animations[AttackName]);
                _animationManager.Update(gameTime);
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                if (timer > howLongShouldAnimationPlay)
                {
                    isActive = false;
                }
            }
        }
    }
}
