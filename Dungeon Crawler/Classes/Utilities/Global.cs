using System;

namespace Dungeon_Crawler
{
    class Global
    {
        public static readonly Random random = new Random();
        public static readonly CameraManager Camera = new CameraManager();
        public static GUI Gui;
        public static bool GameState; //1- game on 0- game over
        public static CombatManager CombatManager;
    }
}
