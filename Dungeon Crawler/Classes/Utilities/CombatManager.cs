using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dungeon_Crawler
{
    public class CombatManager
    {
        public Player _player;
        public LevelManager levelManager;
        private int playerCurrentLevel;
        private Level currentLevel;

        public CombatManager(LevelManager levelManager)
        {
            this.levelManager = levelManager;
            _player = levelManager.player;
        }

        public void Attack(Character attacker, Character defender)
        {
            string tempString;
            // 80% chance to hit
            if (Global.random.Next(10) < 8)
            {
                //(1-attacker.damage)
                int damage = Global.random.Next(attacker.Attack) + 1;
                defender.Health -= damage;
                if (defender.Health > 0)
                {
                    tempString = attacker.Name + " hit " + defender.Name + " for " + damage;
                }
                else
                {
                    tempString = attacker.Name + " killed " + defender.Name;
                } 
                Global.Gui.WriteToConsole(tempString);

            }
            else
            {
                tempString = attacker.Name + " missed " + defender.Name;
                Global.Gui.WriteToConsole(tempString);
            }
        }

        public void Update()
        {
            playerCurrentLevel = _player.CurrentMapLevel;
            currentLevel = levelManager.levels[playerCurrentLevel];
        }

        public Character CharacterAt(int x, int y)
        {
            if (IsPlayerAt(x, y))
            {
                return _player;
            }
            return EnemyAt(x, y);
        }

        public bool IsPlayerAt(int x, int y)
        {
            return (_player.x == x && _player.y == y);
        }

        public Character EnemyAt(int x, int y)
        {
            foreach (var enemy in currentLevel.enemies)
            {
                if (enemy.x == x && enemy.y == y)
                {
                    return enemy;
                }
            }
            return null;
        }

        public bool IsEnemyAt(int x, int y)
        {
            return EnemyAt(x, y) != null;
        }

        public List<Character> IsEnemyInCellAround(int x, int y)
        {
            List<Character> listOfEnemiesAround = new List<Character>(8);
            foreach (Character enemy in currentLevel.enemies)
            {
                if (enemy.x == x - 1 && enemy.y == y + 1)
                    listOfEnemiesAround.Add(enemy);
                if (enemy.x == x && enemy.y == y + 1)
                    listOfEnemiesAround.Add(enemy);
                if (enemy.x == x + 1 && enemy.y == y + 1)
                    listOfEnemiesAround.Add(enemy);
                if (enemy.x == x + 1 && enemy.y == y)
                    listOfEnemiesAround.Add(enemy);
                if (enemy.x == x + 1 && enemy.y == y - 1)
                    listOfEnemiesAround.Add(enemy);
                if (enemy.x == x && enemy.y == y - 1)
                    listOfEnemiesAround.Add(enemy);
                if (enemy.x == x - 1 && enemy.y == y - 1)
                    listOfEnemiesAround.Add(enemy);
                if (enemy.x == x - 1 && enemy.y == y)
                    listOfEnemiesAround.Add(enemy);
            }

            return listOfEnemiesAround;
        }
    }
}