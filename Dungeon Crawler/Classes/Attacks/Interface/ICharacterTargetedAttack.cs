namespace Dungeon_Crawler
{
    /*
     * Interface for attacks targeted at specified character (defender)
     */
    public interface ICharacterTargetedAttack : IAttack
    {
        bool Use(Character attacker, Character defender);
    }
}
