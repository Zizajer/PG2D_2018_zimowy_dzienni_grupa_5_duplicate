using Microsoft.Xna.Framework.Content;

namespace Dungeon_Crawler
{
    class Ranger : Player
    {
        public Ranger(ContentManager content, int cellSize, int playerCurrentMapLevel, string name) : base(content, cellSize, playerCurrentMapLevel, name)
        {
        }
        public override void setAttacks()
        {
            BaseAttack = new Pound();
            ProjectileAttack = new ShootArrow();
            UnTargetedAttack = new Annihilation();
        }
    }
}
