using Microsoft.Xna.Framework.Content;

namespace Dungeon_Crawler
{
    class Mage : Player
    {
        public Mage(ContentManager content, int cellSize, int playerCurrentMapLevel, string name) : base(content, cellSize, playerCurrentMapLevel, name)
        {
        }
        public override void setAttacks()
        {
            BaseAttack = new Pound();
            ProjectileAttack = new Iceball();
            UnTargetedAttack = new Annihilation();
        }
    }
}
