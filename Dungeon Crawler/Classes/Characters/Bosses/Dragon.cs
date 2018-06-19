using System.Collections.Generic;
using RogueSharp;
using System;

namespace Dungeon_Crawler
{
    public class Dragon : Boss
    {
        public Dragon(Dictionary<string, Animation> _animations, int cellSize, int level, float timeBetweenActions, Map map, List<Cell> cells) : base(_animations, cellSize, level, timeBetweenActions, map, cells)
        {
            timeBetweenActions = 0.2f;
        }

        public override void calculateBaseStatistics()
        {
            if (Global.hardMode == false)
            {
                Health = CurrentHealth = 110 + Level * 10;
                Defense = 210 + Level * 3;
                SpDefense = 210 + Level * 5;
                Attack = (int)Math.Floor(210 + Level * 2.5);
                SpAttack = 210 + Level * 3;
                Speed = 0f;
                Experience = 250 + Level * 5;
            }
            else
            {
                Health = CurrentHealth = 150 + Level * 10;
                Defense = 240 + Level * 3;
                SpDefense = 240 + Level * 5;
                Attack = (int)Math.Floor(210 + Level * 2.5);
                SpAttack = 210 + Level * 3;
                Speed = 0f;
                Experience = 250 + Level * 5;
            }
        }
        public override void SetAttack()
        {
            ProjectileAttack = new DragonAttack();
        }
    }
}
