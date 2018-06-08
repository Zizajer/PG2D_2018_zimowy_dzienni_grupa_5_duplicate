namespace Dungeon_Crawler
{
    /*
     * Interface for untargeted attacks (e.g. Exori)
     */
    public interface IUnTargetedAttack : IAttack
    {
        bool Use(Character attacker);
    }
}
