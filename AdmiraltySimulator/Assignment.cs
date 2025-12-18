using System;

namespace AdmiraltySimulator
{
    public enum Faction
    {
        UFP,
        KE,
        RR,
        FA,
    }

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        VeryRare,
        Epic,
    }

    public class Assignment
    {
        public string Name { get; set; }
        public Faction Faction { get; set; }
        public Rarity Rarity { get; set; }
        public int ReqEng { get; set; }
        public int ReqTac { get; set; }
        public int ReqSci { get; set; }
        public int RewardCxp { get; set; }
        public int RewardXp { get; set; }
        public int RewardDilithium { get; set; }
        public int RewardEc { get; set; }
        public string RewardOther { get; set; }
        public TimeSpan Duration { get; set; }
        public bool HasCriticalReward { get; set; }
    }
}