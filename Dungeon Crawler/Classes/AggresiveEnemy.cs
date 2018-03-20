using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;

namespace Dungeon_Crawler
{
    public class AggressiveEnemy:Character
    {
        private readonly IMap _map;
        private readonly GoalMap _goalMap;
        private bool _isAwareOfPlayer;
        private readonly PathToPlayer _path;
        float actionTimer;
        float timeBetweenActions;
        public AggressiveEnemy(IMap map,GoalMap goalMap, PathToPlayer path, float secondsBetweenActions)
        {
            timeBetweenActions = secondsBetweenActions;
            _map = map;
            _goalMap = goalMap;
            _path = path;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, new Vector2(X * Sprite.Width, Y * Sprite.Height), null, Color.White, 0.0f, Vector2.One, 1.0f, SpriteEffects.None, Layers.Figures);
            _path.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            actionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (actionTimer > timeBetweenActions)
            {
                actionTimer = 0;
                if (!_isAwareOfPlayer)
                {
                    if (_map.IsInFov(X, Y))
                    {
                        _isAwareOfPlayer = true;
                    }
                }
                else
                {
                    _path.CreateFrom(X, Y, _goalMap);
                    _goalMap.ClearObstacles();
                    if (GlobalVariables.CombatManager.IsPlayerAt(_path.FirstCell.X, _path.FirstCell.Y))
                    {
                        GlobalVariables.CombatManager.Attack(this, GlobalVariables.CombatManager.CharacterAt(_path.FirstCell.X, _path.FirstCell.Y));
                    }
                    else
                    {
                        int tempX = _path.FirstCell.X;
                        int tempY = _path.FirstCell.Y;
                        if (!GlobalVariables.CombatManager.IsEnemyAt(tempX, tempY))
                        {
                            X = tempX;
                            Y = tempY;
                        }
                        else
                        {
                            Vector2 tempVector2=SurroundLogic.whereToMove(X, Y,_map);
                            if (tempVector2.X == 0.1f || tempVector2.Y == 0.1f) return;
                            X = (int)tempVector2.X;
                            Y = (int)tempVector2.Y;
                        }
                    }
                }
            }
        }
    }
}
