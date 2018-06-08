using Microsoft.Xna.Framework.Content;

namespace Dungeon_Crawler
{
    class Warrior : Player
    {
        public Warrior(ContentManager content, int cellSize, int playerCurrentMapLevel, string name) : base(content, cellSize, playerCurrentMapLevel, name)
        {
        }
        public override void setAttacks()
        {
            BaseAttack = new Pound();
            ProjectileAttack = new BigFireball();
            UnTargetedAttack = new Annihilation();
        }
    }
}
