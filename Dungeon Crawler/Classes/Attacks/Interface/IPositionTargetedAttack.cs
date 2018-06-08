using Microsoft.Xna.Framework;

namespace Dungeon_Crawler
{
    /*
     * Interface for attacks targeted at specified position
     */
    public interface IPositionTargetedAttack : IAttack
    {
        bool Use(Character attacker, Vector2 position);
        void Notify(Character defender);
    }
}
