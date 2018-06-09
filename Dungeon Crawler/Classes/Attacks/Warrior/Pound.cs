namespace Dungeon_Crawler
{
    public class Pound : ICharacterTargetedAttack
    {
        public string Name { get; set; }
        public int Power { get; set; }
        public int Accuracy { get; set; }
        public int CriticalHitProbability { get; set; }
        public int FreezeProbability { get; set; }
        public int BurnProbability { get; set; }

        public bool IsSpecial { get; set; }
        public int ManaCost { get; set; }

        private readonly string AnimationName;

        public Pound()
        {
            Name = "Pound";
            Power = 25;
            Accuracy = 100;
            CriticalHitProbability = 15;
            FreezeProbability = 0;
            BurnProbability = 0;
            IsSpecial = false;
            ManaCost = 0;

            AnimationName = "baseAttackAnim";
        }

        public bool Use(Character attacker, Character defender)
        {
            if (Global.random.Next(100) >= 100 - Accuracy)
            {
                Global.CombatManager.SetAnimation(Name, AnimationName, defender.CellX, defender.CellY);
                Global.CombatManager.Attack(attacker, this, defender);
                if(attacker is Warrior)
                {
                    Warrior war = (Warrior)attacker;
                    war.addResource(20);
                }
                return true; //Attack hit
            }
            else
            {
                Global.Gui.WriteToConsole(attacker.Name + " missed " + Name);
                return false; //Attack missed
            }
        }
    }
}
