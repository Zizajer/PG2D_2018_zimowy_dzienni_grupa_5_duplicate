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
        public static Effects Effects;
        public static SoundManager SoundManager;
        public static String[] classes= { "Warrior", "Ranger", "Mage" };
        public static String playerClass; //TODO when menu is finished use this to assign class to player
        public static String playerName="Player"; //TODO when menu is finished use this to assign player name
        public static LevelManager levelmanager;
    }
}
