using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;

namespace Dungeon_Crawler
{
    public class AggressiveEnemy:Character
    {
        private readonly IMap _map;
        private bool _isAwareOfPlayer;
        private readonly PathToPlayer _path;
        public AggressiveEnemy(IMap map,PathToPlayer path)
        {
            _map = map;
            _path = path;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, new Vector2(X * Sprite.Width, Y * Sprite.Height), null, Color.White, 0.0f, Vector2.One, 1.0f, SpriteEffects.None, LayerDepth.Figures);
            _path.Draw(spriteBatch);
        }
        public void Update()
        {
            if (!_isAwareOfPlayer)
            {
                if (_map.IsInFov(X, Y))
                {
                    _isAwareOfPlayer = true;
                }
            }
            else
            { 

                _path.CreateFrom(X, Y);
                if (Global.CombatManager.IsPlayerAt(_path.FirstCell.X, _path.FirstCell.Y))
                {
                    Global.CombatManager.Attack(this,Global.CombatManager.CharacterAt(_path.FirstCell.X, _path.FirstCell.Y));
                }
                else
                {
                    int tempX = _path.FirstCell.X;
                    int tempY = _path.FirstCell.Y;
                    if(!Global.CombatManager.IsEnemyAt(tempX, tempY))
                    {
                        X = tempX;
                        Y = tempY;
                    }
                    
                }
            }
        }
    }
}
