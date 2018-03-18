using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            spriteBatch.Draw(Sprite, new Vector2(X * Sprite.Width, Y * Sprite.Height), null, null, null, 0.0f, Vector2.One, Color.White, SpriteEffects.None, LayerDepth.Figures);
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
                        X = _path.FirstCell.X;
                        Y = _path.FirstCell.Y;
                    }
                    
                }
            }
        }
    }
}
