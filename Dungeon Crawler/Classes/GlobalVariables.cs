using RogueSharp.Random;

namespace Dungeon_Crawler
{
    public class GlobalVariables
    {
        public static readonly int MapWidth = 50;
        public static readonly int MapHeight = 30;
        public static readonly int SpriteWidth = 64;
        public static readonly int SpriteHeight = 64;
        public static readonly IRandom Random = new DotNetRandom();
        public static bool DebugMode { get; set; }
        public static readonly CameraManager Camera = new CameraManager();
        public static CombatManager CombatManager;
        public static GUI Gui;
    }
}
