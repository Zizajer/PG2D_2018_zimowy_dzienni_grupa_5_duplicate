using System.Collections.Generic;
using RogueSharp;
using System;

namespace Dungeon_Crawler
{
    public class DemonOak : Boss
    {
        public DemonOak(Dictionary<string, Animation> _animations, int cellSize, int level, float timeBetweenActions, Map map, List<Cell> cells) : base(_animations, cellSize, level, timeBetweenActions, map, cells)
        {
        }

        public override void calculateBaseStatistics()
        {
            Health = CurrentHealth = 110 + Level * 10;
            Defense = 210 + Level * 3;
            SpDefense = 210 + Level * 5;
            Attack = (int)Math.Floor(210 + Level * 2.5);
            SpAttack = 210 + Level * 3;
            Speed = 0f;
            Experience = 250 + Level * 5;
        }
        public override void SetAttack()
        {
            ProjectileAttack = new BigFireballCanonade();
        }
    }
}
