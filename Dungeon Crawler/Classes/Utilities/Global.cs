using Microsoft.Xna.Framework.Input;
using System;

namespace Dungeon_Crawler
{
    class Global
    {
        public static readonly Random random = new Random();
        public static readonly CameraManager Camera = new CameraManager();
        public static GUI Gui;
        public enum Gamestates { isMainMenu, isHeroChooseMenu, isClassSpecificMenu, isHelpMenu, isAboutMenu, isGameActive, isStatsMenu, isGameOver };
        public static Gamestates CurrentGameState;
        public static bool hardMode; //false easy | true hard
        public static CombatManager CombatManager;
        public static MenuManager MenuManager;
        public static StatsAllocationSystem StatsAllocationSystem;
        public static Effects Effects;
        public static SoundManager SoundManager;
        public static String[] classes= { "Warrior", "Ranger", "Mage" };
        public static String playerClass; //TODO when menu is finished use this to assign class to player
        public static String playerName="Player"; //TODO when menu is finished use this to assign player name
        public static LevelManager levelmanager;
        public static KeyboardState pastKey13; //O
    }
}
