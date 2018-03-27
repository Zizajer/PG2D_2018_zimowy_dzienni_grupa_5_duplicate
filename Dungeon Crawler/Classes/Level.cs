using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueSharp;

namespace Dungeon_Crawler.Classes
{
    class Level
    {
        private Texture2D floor;
        private Texture2D wall;
        private int cellSize;
        private Map map;
        private Player player;
        List<Item> items;
        List<Enemy> enemies;
    }
}
