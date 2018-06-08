namespace Dungeon_Crawler
{
    public interface IAttack
    {
        string Name { get; set; }
        int Power { get; set; }
        int Accuracy { get; set; }
        int CriticalHitProbability { get; set; }
        int FreezeProbability { get; set; }
        int BurnProbability { get; set; }

        bool IsSpecial { get; set; }
        int ManaCost { get; set; }
    }
}
