﻿using System.Collections.Generic;
using RogueSharp;
using System;

namespace Dungeon_Crawler
{
    public class Skeleton : Enemy
    {
        public Skeleton(Dictionary<string, Animation> _animations, int cellSize, int level, float speed, float timeBetweenActions, Map map, string name) : base(_animations, cellSize, level, speed, timeBetweenActions, map, name)
        {

        }
        public override void setAttacks()
        {
            BaseAttack = new EnemyPound();
        }
        public override void calculateBaseStatistics()
        {
            if (Global.hardMode == false)
            {
                Health = CurrentHealth = 10 + Level * 10;
                Defense = 30 + Level * 3;
                SpDefense = 50 + Level * 5;
                Attack = (int)Math.Floor(35 + Level * 2.5f);
                SpAttack = 50 + Level * 3;
                Experience = 40 + Level * 5;
                Speed = 2f;
            }
            else
            {
                Health = CurrentHealth = 30 + Level * 10;
                Defense = 50 + Level * 3;
                SpDefense = 70 + Level * 5;
                Attack = (int)Math.Floor(35 + Level * 2.5f);
                SpAttack = 50 + Level * 3;
                Experience = 40 + Level * 5;
                Speed = 2f;
            }
        }
    }
}
