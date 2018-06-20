using Microsoft.Xna.Framework.Input;
using System;

namespace Dungeon_Crawler
{
    class Global
    {
        public static readonly Random random = new Random();
        public static readonly CameraManager Camera = new CameraManager();
        public static GUI Gui;
        public static int GameStates;
        //0- main menu 1- choosing hero 2- keyboard 3- about 4-game 5- stats 6- gameover
        public static bool hardMode; //false easy | true hard
        public static CombatManager CombatManager;
        public static DrawManager DrawManager;
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
