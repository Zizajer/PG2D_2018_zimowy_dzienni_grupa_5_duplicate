using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using System.Collections.Generic;

namespace Dungeon_Crawler
{
    public class Player:Character
    {
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, new Vector2(X * Sprite.Width, Y * Sprite.Height), null, Color.White, 0.0f, Vector2.One, 1.0f, SpriteEffects.None, LayerDepth.Figures);
        }
        public bool HandleInput(InputState inputState, IMap map)
        {
            if (inputState.IsLeft(PlayerIndex.One))
            {
                int tempX = X - 1;
                if (map.IsWalkable(tempX, Y) && !Global.CombatManager.IsEnemyAt(tempX, Y))
                {
                    X = tempX;
                    return true;
                }
            }
            else if (inputState.IsRight(PlayerIndex.One))
            {
                int tempX = X + 1;
                if (map.IsWalkable(tempX, Y) && !Global.CombatManager.IsEnemyAt(tempX, Y))
                {
                    X = tempX;
                    return true;
                }
            }
            else if (inputState.IsUp(PlayerIndex.One))
            {
                int tempY = Y - 1;
                if (map.IsWalkable(X, tempY) && !Global.CombatManager.IsEnemyAt(X, tempY))
                {
                    Y = tempY;
                    return true;
                }
            }
            else if (inputState.IsDown(PlayerIndex.One))
            {
                int tempY = Y + 1;
                if (map.IsWalkable(X, tempY) && !Global.CombatManager.IsEnemyAt(X, tempY))
                {
                    Y = tempY;
                    return true;
                }
            }
            else if (inputState.IsShift(PlayerIndex.One))
            {
                List<AggressiveEnemy> listOfEnemiesAround = Global.CombatManager.IsEnemyInCellAround(X, Y);
                if (listOfEnemiesAround.Count > 0)
                {
                    foreach (AggressiveEnemy enemy in listOfEnemiesAround)
                    {
                        Global.CombatManager.Attack(this, enemy);
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
