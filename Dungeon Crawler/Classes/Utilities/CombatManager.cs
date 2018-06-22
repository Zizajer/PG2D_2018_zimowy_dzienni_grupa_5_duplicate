using System;
using System.Collections.Generic;
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
        }


        public void setPLayer() {
            _player = levelManager.player;
        }

        public void Attack(Character attacker, IAttack attack, Character defender)
        {
            if (attacker.Name.Equals(defender.Name)) return;
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
                attacker.Experience = attacker.Experience + defender.Experience;
                if(defender is Player)
                {
                    Global.CurrentGameState = Global.Gamestates.isGameOver;
                }
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
            if(attacker is Warrior)
            {
                Warrior war = (Warrior)attacker;
                if (war.isBerserkerRageOn)
                {
                    int heal = (int)Math.Ceiling(war.Health * 0.01);
                    war.addHealth(heal);
                    Global.Gui.WriteToConsole("Berserker Rage healed "+war.Name + " for " + heal);
                }
            }
            if (defender is Player)
                Global.SoundManager.playerHurt.Play();
            
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

        public Character EnemyAt(int x, int y)
        {
            if (currentLevel.isBossLevel)
            {
                if (currentLevel.enemies.Count == 0) return null;
                Boss boss = (Boss)currentLevel.enemies.ElementAt(0);

                foreach (RogueSharp.Cell cell in boss.occupyingCells)
                {
                    if (cell.X == x && cell.Y == y)
                    {
                        return boss;
                    }
                }
                if (x == boss.CellX && y == boss.CellY)
                {
                    return boss;
                }
            }
            else
            {
                foreach (var enemy in currentLevel.enemies)
                {
                    if (enemy.CellX == x && enemy.CellY == y)
                    {
                        return enemy;
                    }
                }
            }
            
            return null;
        }

        public bool IsEnemyAt(int x, int y)
        {
            return EnemyAt(x, y) != null;
        }

        public Character PlayerAt(int x, int y)
        {
            Character tempPlayer = currentLevel.player;
            if (x == tempPlayer.CellX && y == tempPlayer.CellY)
                return tempPlayer;
            else
                return null;
        }
        public bool IsPlayerAt(int x, int y)
        {
            return PlayerAt(x, y) != null;
        }

        public List<Character> GetEnemiesInAreaInFrontOfAttacker(Character attaker, int cellX, int cellY, int distance)
        {
            List<Character> listOfEnemiesAround = new List<Character>();

            List<RogueSharp.Cell> cells = GetCellsInFrontOfCharacter(attaker, cellX, cellY, distance);

            if (currentLevel.isBossLevel)
            {
                if (currentLevel.enemies.Count == 0) return listOfEnemiesAround;
                Boss boss = (Boss)currentLevel.enemies.ElementAt(0);

                foreach (RogueSharp.Cell cell in cells)
                {
                    if (IsEnemyAt(cell.X, cell.Y))
                    {
                        listOfEnemiesAround.Add(EnemyAt(cell.X, cell.Y));
                    }
                    if (IsPlayerAt(cell.X, cell.Y))
                    {
                        listOfEnemiesAround.Add(PlayerAt(cell.X, cell.Y));
                    }
                }
            }
            else
            {
                foreach (Character enemy in currentLevel.enemies)
                {
                    foreach (RogueSharp.Cell cell in cells)
                    {
                        if (IsEnemyAt(cell.X, cell.Y))
                        {
                            listOfEnemiesAround.Add(EnemyAt(cell.X, cell.Y));
                        }
                    }
                }
                foreach (RogueSharp.Cell cell in cells)
                {
                    if (IsPlayerAt(cell.X, cell.Y))
                    {
                        listOfEnemiesAround.Add(PlayerAt(cell.X, cell.Y));
                    }
                }
            }
            return listOfEnemiesAround;
        }

        public bool SetAnimationInAreaInFrontOfAttacker(Character attaker, string attackName, string animationName, int cellX, int cellY, int distance)
        {
            List<RogueSharp.Cell> cells = GetCellsInFrontOfCharacter(attaker, cellX, cellY, distance);

            foreach (RogueSharp.Cell cell in cells)
            {
                currentLevel.attackAnimations.Add(new AttackAnimation(levelManager.Content, attackName, animationName, cell.X, cell.Y, currentLevel.cellSize));
            }
            if (cells.Count > 0) return true;
            return false;
        }

        private List<RogueSharp.Cell> GetCellsInFrontOfCharacter(Character attaker, int cellX, int cellY, int distance)
        {
            List<RogueSharp.Cell> cells = new List<RogueSharp.Cell>(distance);
            RogueSharp.Cell nextCell = null;
            int nextX, nextY;

            if (attaker.currentFaceDirection == Character.FaceDirections.Up)
            {
                for (int dist = 1; dist <= distance; dist++)
                {
                    nextX = cellX;
                    nextY = cellY - dist;

                    if (nextX < 0 || nextX >= currentLevel.map.Width || nextY < 0 || nextY >= currentLevel.map.Height)
                        break;
                    else
                    {
                        nextCell = currentLevel.map.GetCell(nextX, nextY);
                        if (!nextCell.IsWalkable)
                            break;
                        cells.Add(nextCell);
                    }
                }
            }
            else if (attaker.currentFaceDirection == Character.FaceDirections.Down)
            {
                for (int dist = 1; dist < distance; dist++)
                {
                    nextX = cellX;
                    nextY = cellY + dist;

                    if (nextX < 0 || nextX >= currentLevel.map.Width || nextY < 0 || nextY >= currentLevel.map.Height)
                        break;
                    else
                    {
                        nextCell = currentLevel.map.GetCell(nextX, nextY);
                        if (!nextCell.IsWalkable)
                            break;
                        cells.Add(nextCell);
                    }
                }
            }
            else if (attaker.currentFaceDirection == Character.FaceDirections.Left)
            {
                for (int dist = 1; dist < distance; dist++)
                {
                    nextX = cellX - dist;
                    nextY = cellY;

                    if (nextX < 0 || nextX >= currentLevel.map.Width || nextY < 0 || nextY >= currentLevel.map.Height)
                        break;
                    else
                    {
                        nextCell = currentLevel.map.GetCell(nextX, nextY);
                        if (!nextCell.IsWalkable)
                            break;
                        cells.Add(nextCell);
                    }
                }
            }
            else
            {
                for (int dist = 1; dist < distance; dist++)
                {
                    nextX = cellX + dist;
                    nextY = cellY;

                    if (nextX < 0 || nextX >= currentLevel.map.Width || nextY < 0 || nextY >= currentLevel.map.Height)
                        break;
                    else
                    {
                        nextCell = currentLevel.map.GetCell(nextX, nextY);
                        if (!nextCell.IsWalkable)
                            break;
                        cells.Add(nextCell);
                    }
                }
            }

            return cells;
        }

        public List<Character> GetEnemiesInArea(int cellX, int cellY, int distance)
        {
            List<Character> listOfEnemiesAround = new List<Character>();

            if (currentLevel.isBossLevel)
            {
                if (currentLevel.enemies.Count == 0) return listOfEnemiesAround;
                Boss boss = (Boss)currentLevel.enemies.ElementAt(0);
                int dist = Global.CombatManager.DistanceBetween2Points(cellX, cellY, boss.CellX, boss.CellY);
                if (dist <= distance + 1 && dist > 0)
                {
                    listOfEnemiesAround.Add(boss);
                }
                Character enemy2 = currentLevel.player;
                if (Global.CombatManager.DistanceBetween2Points(cellX, cellY, enemy2.CellX, enemy2.CellY) <= distance)
                {
                    listOfEnemiesAround.Add(enemy2);
                }
            }
            else
            {
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
                Character enemy2 = currentLevel.player;
                for (int i = 0; i <= distance; i++)
                {
                    if (enemy2.CellX == cellX - i && enemy2.CellY == cellY + i)
                        listOfEnemiesAround.Add(enemy2);
                    if (enemy2.CellX == cellX && enemy2.CellY == cellY + i)
                        listOfEnemiesAround.Add(enemy2);
                    if (enemy2.CellX == cellX + i && enemy2.CellY == cellY + i)
                        listOfEnemiesAround.Add(enemy2);
                    if (enemy2.CellX == cellX + i && enemy2.CellY == cellY)
                        listOfEnemiesAround.Add(enemy2);
                    if (enemy2.CellX == cellX + i && enemy2.CellY == cellY - i)
                        listOfEnemiesAround.Add(enemy2);
                    if (enemy2.CellX == cellX && enemy2.CellY == cellY - i)
                        listOfEnemiesAround.Add(enemy2);
                    if (enemy2.CellX == cellX - i && enemy2.CellY == cellY - i)
                        listOfEnemiesAround.Add(enemy2);
                    if (enemy2.CellX == cellX - i && enemy2.CellY == cellY)
                        listOfEnemiesAround.Add(enemy2);
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

        public int DistanceBetween2Points(int x1, int y1, int x2, int y2)
        {
            //wow Im actually good at maths :d
            return (int) Math.Max(Math.Ceiling(Math.Sqrt(Math.Pow(x2 - x1, 2))), Math.Ceiling(Math.Sqrt(Math.Pow(y2 - y1, 2))));
            //return (int)Math.Round(Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2)));
        }
    }
}