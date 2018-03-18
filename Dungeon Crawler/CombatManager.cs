using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class CombatManager
    {
        private readonly Player _player;
        private readonly List<AggressiveEnemy> _aggressiveEnemies;

        public CombatManager(Player player, List<AggressiveEnemy> aggressiveEnemies)
        {
            _player = player;
            _aggressiveEnemies = aggressiveEnemies;
        }

        public void Attack(Character attacker, Character defender)
        {
            // 80% chance to hit
            if (Global.Random.Next(10)<8)
            {
                //(1-attacker.damage)
                int damage = Global.Random.Next(attacker.Damage)+1;
                defender.Health -= damage;
                Debug.WriteLine("{0} hit {1} for {2} and he has {3} health remaining.",
                  attacker.Name, defender.Name, damage, defender.Health);
                if (defender.Health <= 0)
                {
                    if (defender is AggressiveEnemy)
                    {
                        var enemy = defender as AggressiveEnemy;
                        _aggressiveEnemies.Remove(enemy);
                        Debug.WriteLine("{0} killed {1}", attacker.Name, defender.Name);
                    }
                    if (defender is Player)
                    {
                        Debug.WriteLine("You were killed by {0}", attacker.Name);
                    }  
                }
            }
            else
            {
                Debug.WriteLine("{0} missed {1}", attacker.Name, defender.Name);
            }
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
            return (_player.X == x && _player.Y == y);
        }

        public AggressiveEnemy EnemyAt(int x, int y)
        {
            foreach (var enemy in _aggressiveEnemies)
            {
                if (enemy.X == x && enemy.Y == y)
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

        public List<AggressiveEnemy> IsEnemyInCellAround(int x, int y)
        {
            List<AggressiveEnemy> listOfEnemiesAround = new List<AggressiveEnemy>(8);
            foreach(AggressiveEnemy enemy in _aggressiveEnemies)
            {
                if (enemy.X == x - 1 && enemy.Y == y + 1)
                    listOfEnemiesAround.Add(enemy);
                if (enemy.X == x && enemy.Y == y + 1)
                    listOfEnemiesAround.Add(enemy);
                if (enemy.X == x + 1 && enemy.Y == y + 1)
                    listOfEnemiesAround.Add(enemy);
                if (enemy.X == x + 1 && enemy.Y == y)
                    listOfEnemiesAround.Add(enemy);
                if (enemy.X == x + 1 && enemy.Y == y - 1)
                    listOfEnemiesAround.Add(enemy);
                if (enemy.X == x && enemy.Y == y - 1)
                    listOfEnemiesAround.Add(enemy);
                if (enemy.X == x - 1 && enemy.Y == y - 1)
                    listOfEnemiesAround.Add(enemy);
                if (enemy.X == x - 1 && enemy.Y == y)
                    listOfEnemiesAround.Add(enemy);
            }

            return listOfEnemiesAround;
        }
    }
}
