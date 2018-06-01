using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Dungeon_Crawler.Character;

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

        public void Attack(Character attacker, IAttack attack, Character defender)
        {
            string tempString;
            bool IsCritical = false;

            if (Global.random.Next(100) >= 100 - attack.CriticalHitProbability)
            {
                IsCritical = true;
            }

            if (defender.currentHealthState == HealthState.Normal)
            {
                if (Global.random.Next(100) >= 100 - attack.BurnProbability)
                {
                    defender.currentHealthState = HealthState.Burn;
                    defender.isBurnShaderOn = true;
                }

                if (Global.random.Next(100) >= 100 - attack.FreezeProbability)
                {
                    defender.currentHealthState = HealthState.Freeze;
                    defender.isFreezeShaderOn = true;
                }
            }

            //(1-attacker.damage)
            int Damage = CalculateDamage(attacker, attack, IsCritical, defender);
            defender.CurrentHealth -= Damage;
            defender.isHitShaderOn = true;

            if (defender.CurrentHealth > 0)
            {
                tempString = attacker.Name + " hit " + defender.Name + " with " + attack.Name + " for " + Damage;
            }
            else
            {
                tempString = attacker.Name + " killed " + defender.Name + " with " + attack.Name;
            }
            if (IsCritical)
            {
                tempString += ". Critical hit!";
            }
            if (defender.currentHealthState == HealthState.Freeze)
            {
                tempString += ". Frozen!";
            }
            if (defender.currentHealthState == HealthState.Burn)
            {
                tempString += ". Burned!";
            }
            Global.Gui.WriteToConsole(tempString);

        }

        public int CalculateDamage(Character attacker, IAttack attack, Boolean isCritical, Character defender)
        {
            float Modifier = 1;

            if (isCritical)
            {
                Modifier = Modifier * 1.5f;
            }

            if (defender.currentHealthState == HealthState.Burn)
            {
                Modifier = Modifier * 0.5f;
            }

            if (attack.IsSpecial)
            {
                return (int)(((((((2f * attacker.Level) / 5f) + 2f) * attack.Power * attacker.SpAttack / defender.SpDefense) / 50f) + 2f) * Modifier);
            }
            else
            {
                return (int)(((((((2f * attacker.Level) / 5f) + 2f) * attack.Power * attacker.Attack / defender.Defense) / 50f) + 2f) * Modifier);
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
            return (_player.CellX == x && _player.CellY == y);
        }

        //TODO: Reimplement character detection by checking collision of charater's rectangle with cell's rectangle (enemies bigger than single cell doesn't actually occupy
        //all of their cells in code, so we cannot simply check single cell
        public Character EnemyAt(int x, int y)
        {
            foreach (var enemy in currentLevel.enemies)
            {
                if (enemy.CellX == x && enemy.CellY == y)
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

        //TODO: Reimplement character detection by checking collision of charater's rectangle with cell's rectangle (enemies bigger than single cell doesn't actually occupy
        //all of their cells in code, so we cannot simply check single cell. Current boss detection is buggy.
        public List<Character> GetEnemiesInArea(int cellX, int cellY, int distance)
        {
            List<Character> listOfEnemiesAround = new List<Character>();
            foreach (Character enemy in currentLevel.enemies)
            {
                for (int i = 0; i <= distance; i++)
                {
                    if (enemy.CellX == cellX - i && enemy.CellY == cellY + i)
                        listOfEnemiesAround.Add(enemy);
                    if (enemy.CellX == cellX && enemy.CellY == cellY + i)
                        listOfEnemiesAround.Add(enemy);
                    if (enemy.CellX == cellX + i && enemy.CellY == cellY + i)
                        listOfEnemiesAround.Add(enemy);
                    if (enemy.CellX == cellX + i && enemy.CellY == cellY)
                        listOfEnemiesAround.Add(enemy);
                    if (enemy.CellX == cellX + i && enemy.CellY == cellY - i)
                        listOfEnemiesAround.Add(enemy);
                    if (enemy.CellX == cellX && enemy.CellY == cellY - i)
                        listOfEnemiesAround.Add(enemy);
                    if (enemy.CellX == cellX - i && enemy.CellY == cellY - i)
                        listOfEnemiesAround.Add(enemy);
                    if (enemy.CellX == cellX - i && enemy.CellY == cellY)
                        listOfEnemiesAround.Add(enemy);
                }
            }

            foreach (Character enemy in currentLevel.enemies)
            {
                if(enemy is Boss)
                {
                    List<RogueSharp.Cell> cellsAroundTheCellList = currentLevel.map.GetCellsInArea(enemy.CellX, enemy.CellY, 1).ToList();
                    foreach (RogueSharp.Cell cell in cellsAroundTheCellList)
                    {
                        if (cell.X == cellX - 1 && cell.Y == cellY + 1)
                        {
                            listOfEnemiesAround.Add(enemy);
                            break;
                        }

                        if (cell.X == cellX && cell.Y == cellY + 1)
                        {
                            listOfEnemiesAround.Add(enemy);
                            break;
                        }

                        if (cell.X == cellX + 1 && cell.Y == cellY + 1)
                        {
                            listOfEnemiesAround.Add(enemy);
                            break;
                        }
                        if (cell.X == cellX + 1 && cell.Y == cellY)
                        {
                            listOfEnemiesAround.Add(enemy);
                            break;
                        }
                        if (cell.X == cellX + 1 && cell.Y == cellY - 1)
                        {
                            listOfEnemiesAround.Add(enemy);
                            break;
                        }
                        if (cell.X == cellX && cell.Y == cellY - 1)
                        {
                            listOfEnemiesAround.Add(enemy);
                            break;
                        }
                        if (cell.X == cellX - 1 && cell.Y == cellY - 1)
                        {
                            listOfEnemiesAround.Add(enemy);
                            break;
                        }
                        if (cell.X == cellX - 1 && cell.Y == cellY)
                        {
                            listOfEnemiesAround.Add(enemy);
                            break;
                        }
                    } 
                }
            }
                return listOfEnemiesAround;
        }

        public void SetAnimationInArea(string attackName, string animationName, int cellX, int cellY, int distance)
        {
            List<RogueSharp.Cell> cellsAroundTheCellList = currentLevel.map.GetCellsInArea(cellX, cellY, distance).ToList();
            foreach (RogueSharp.Cell cell in cellsAroundTheCellList)
            {
                if (cell.IsWalkable)
                    currentLevel.attackAnimations.Add(new AttackAnimation(levelManager.Content, attackName, animationName, cell.X, cell.Y, currentLevel.cellSize));
            }
        }

        public void SetAnimation(string attackName, string animationName, int cellX, int cellY)
        {
            RogueSharp.Cell cell = currentLevel.map.GetCell(cellX, cellY);
            if (cell.IsWalkable)
                currentLevel.attackAnimations.Add(new AttackAnimation(levelManager.Content, attackName, animationName, cellX, cellY, currentLevel.cellSize));
        }

        //CombatManager really should not be responsible for such things
        public void PutProjectile(Projectile projectile)
        {
            currentLevel.Projectiles.Add(projectile);
        }

    }
}