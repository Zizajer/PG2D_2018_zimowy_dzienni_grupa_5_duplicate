using Microsoft.Xna.Framework;
using RogueSharp;

namespace Dungeon_Crawler
{
    public class SurroundLogic
    {
        static public Vector2 whereToMove(int X, int Y,IMap _map)
        {
            Vector2 result = new Vector2(0.1f,0.1f);
            if (GlobalVariables.CombatManager.IsEnemyAt(X - 1, Y) && GlobalVariables.CombatManager.IsPlayerAt(X - 2, Y))
            {
                if (!GlobalVariables.CombatManager.IsEnemyAt(X - 1, Y + 1) && _map.IsWalkable(X - 1, Y + 1) && !GlobalVariables.CombatManager.IsPlayerAt(X - 1, Y + 1))
                {
                    result.X = X - 1;
                    result.Y = Y + 1;
                    return result;
                }
                if (!GlobalVariables.CombatManager.IsEnemyAt(X - 1, Y - 1) && _map.IsWalkable(X - 1, Y - 1) && !GlobalVariables.CombatManager.IsPlayerAt(X - 1, Y - 1))
                {
                    result.X = X - 1;
                    result.Y = Y - 1;
                    return result;
                }
                if (!GlobalVariables.CombatManager.IsEnemyAt(X - 2, Y + 1) && _map.IsWalkable(X - 2, Y + 1) && !GlobalVariables.CombatManager.IsPlayerAt(X - 2, Y + 1))
                {
                    result.X = X - 2;
                    result.Y = Y + 1;
                    return result;
                }
                if (!GlobalVariables.CombatManager.IsEnemyAt(X - 2, Y - 1) && _map.IsWalkable(X - 2, Y - 1) && !GlobalVariables.CombatManager.IsPlayerAt(X - 2, Y - 1))
                {
                    result.X = X - 2;
                    result.Y = Y - 1;
                    return result;
                }

                if (!GlobalVariables.CombatManager.IsEnemyAt(X - 3, Y + 1) && _map.IsWalkable(X - 3, Y + 1) && !GlobalVariables.CombatManager.IsPlayerAt(X - 3, Y + 1))
                {
                    result.X = X - 3;
                    result.Y = Y + 1;
                    return result;
                }
                if (!GlobalVariables.CombatManager.IsEnemyAt(X - 3, Y - 1) && _map.IsWalkable(X - 3, Y - 1) && !GlobalVariables.CombatManager.IsPlayerAt(X - 3, Y - 1))
                {
                    result.X = X - 3;
                    result.Y = Y - 1;
                    return result;
                }

            }

            if (GlobalVariables.CombatManager.IsEnemyAt(X + 1, Y) && GlobalVariables.CombatManager.IsPlayerAt(X + 2, Y))
            {
                if (!GlobalVariables.CombatManager.IsEnemyAt(X + 1, Y + 1) && _map.IsWalkable(X + 1, Y + 1) && !GlobalVariables.CombatManager.IsPlayerAt(X + 1, Y + 1))
                {
                    result.X = X + 1;
                    result.Y = Y + 1;
                    return result;
                }
                if (!GlobalVariables.CombatManager.IsEnemyAt(X + 1, Y - 1) && _map.IsWalkable(X + 1, Y - 1) && !GlobalVariables.CombatManager.IsPlayerAt(X + 1, Y - 1))
                {
                    result.X = X + 1;
                    result.Y = Y - 1;
                    return result;
                }
                if (!GlobalVariables.CombatManager.IsEnemyAt(X + 2, Y + 1) && _map.IsWalkable(X + 2, Y + 1) && !GlobalVariables.CombatManager.IsPlayerAt(X + 2, Y + 1))
                {
                    result.X = X + 2;
                    result.Y = Y + 1;
                    return result;
                }
                if (!GlobalVariables.CombatManager.IsEnemyAt(X + 2, Y - 1) && _map.IsWalkable(X + 2, Y - 1) && !GlobalVariables.CombatManager.IsPlayerAt(X + 2, Y - 1))
                {
                    result.X = X + 2;
                    result.Y = Y - 1;
                    return result;
                }
                if (!GlobalVariables.CombatManager.IsEnemyAt(X + 3, Y + 1) && _map.IsWalkable(X + 3, Y + 1) && !GlobalVariables.CombatManager.IsPlayerAt(X + 3, Y + 1))
                {
                    result.X = X + 3;
                    result.Y = Y + 1;
                    return result;
                }
                if (!GlobalVariables.CombatManager.IsEnemyAt(X + 3, Y - 1) && _map.IsWalkable(X + 3, Y - 1) && !GlobalVariables.CombatManager.IsPlayerAt(X + 3, Y - 1))
                {
                    result.X = X + 3;
                    result.Y = Y - 1;
                    return result;
                }
            }

            if (GlobalVariables.CombatManager.IsEnemyAt(X, Y + 1) && GlobalVariables.CombatManager.IsPlayerAt(X, Y + 2))
            {
                if (!GlobalVariables.CombatManager.IsEnemyAt(X - 1, Y + 1) && _map.IsWalkable(X - 1, Y + 1) && !GlobalVariables.CombatManager.IsPlayerAt(X - 1, Y + 1))
                {
                    result.X = X - 1;
                    result.Y = Y + 1;
                    return result;
                }
                if (!GlobalVariables.CombatManager.IsEnemyAt(X + 1, Y + 1) && _map.IsWalkable(X + 1, Y + 1) && !GlobalVariables.CombatManager.IsPlayerAt(X + 1, Y + 1))
                {
                    result.X = X + 1;
                    result.Y = Y + 1;
                    return result;
                }
                if (!GlobalVariables.CombatManager.IsEnemyAt(X - 1, Y + 2) && _map.IsWalkable(X - 1, Y + 2) && !GlobalVariables.CombatManager.IsPlayerAt(X - 1, Y + 2))
                {
                    result.X = X - 1;
                    result.Y = Y + 2;
                    return result;
                }
                if (!GlobalVariables.CombatManager.IsEnemyAt(X + 1, Y + 2) && _map.IsWalkable(X + 1, Y + 2) && !GlobalVariables.CombatManager.IsPlayerAt(X + 1, Y + 2))
                {
                    result.X = X + 1;
                    result.Y = Y + 2;
                    return result;
                }
                if (!GlobalVariables.CombatManager.IsEnemyAt(X - 1, Y + 3) && _map.IsWalkable(X - 1, Y + 3) && !GlobalVariables.CombatManager.IsPlayerAt(X - 1, Y + 3))
                {
                    result.X = X - 1;
                    result.Y = Y + 3;
                    return result;
                }
                if (!GlobalVariables.CombatManager.IsEnemyAt(X + 1, Y + 3) && _map.IsWalkable(X + 1, Y + 3) && !GlobalVariables.CombatManager.IsPlayerAt(X + 1, Y + 3))
                {
                    result.X = X + 1;
                    result.Y = Y + 3;
                    return result;
                }
            }

            if (GlobalVariables.CombatManager.IsEnemyAt(X, Y - 1) && GlobalVariables.CombatManager.IsPlayerAt(X, Y - 2))
            {
                if (!GlobalVariables.CombatManager.IsEnemyAt(X + 1, Y - 1) && _map.IsWalkable(X + 1, Y - 1) && !GlobalVariables.CombatManager.IsPlayerAt(X + 1, Y - 1))
                {
                    result.X = X + 1;
                    result.Y = Y - 1;
                    return result;
                }
                if (!GlobalVariables.CombatManager.IsEnemyAt(X - 1, Y - 1) && _map.IsWalkable(X - 1, Y - 1) && !GlobalVariables.CombatManager.IsPlayerAt(X - 1, Y - 1))
                {
                    result.X = X - 1;
                    result.Y = Y - 1;
                    return result;
                }
                if (!GlobalVariables.CombatManager.IsEnemyAt(X + 1, Y - 2) && _map.IsWalkable(X + 1, Y - 2) && !GlobalVariables.CombatManager.IsPlayerAt(X + 1, Y - 2))
                {
                    result.X = X + 1;
                    result.Y = Y - 2;
                    return result;
                }
                if (!GlobalVariables.CombatManager.IsEnemyAt(X - 1, Y - 2) && _map.IsWalkable(X - 1, Y - 2) && !GlobalVariables.CombatManager.IsPlayerAt(X - 1, Y - 2))
                {
                    result.X = X - 1;
                    result.Y = Y - 2;
                    return result;
                }
                if (!GlobalVariables.CombatManager.IsEnemyAt(X + 1, Y - 3) && _map.IsWalkable(X + 1, Y - 3) && !GlobalVariables.CombatManager.IsPlayerAt(X + 1, Y - 3))
                {
                    result.X = X + 1;
                    result.Y = Y - 3;
                    return result;
                }
                if (!GlobalVariables.CombatManager.IsEnemyAt(X - 1, Y - 3) && _map.IsWalkable(X - 1, Y - 3) && !GlobalVariables.CombatManager.IsPlayerAt(X - 1, Y - 3))
                {
                    result.X = X - 1;
                    result.Y = Y - 3;
                    return result;
                }
            }

            return result;
        }
    }
}
