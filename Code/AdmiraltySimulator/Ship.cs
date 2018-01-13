using System;
using System.Collections.Generic;

namespace AdmiraltySimulator
{
    public enum ShipType
    {
        Eng,
        Tac,
        Sci,
        Small,
        Any,
        None
    }

    public enum StatType
    {
        Eng,
        Tac,
        Sci,
        All
    }

    public class Ship
    {
        public static TimeSpan OneTimeShipMaint = TimeSpan.Zero;

        public Ship(string name, ShipType type, int engValue, int tacValue, int sciValue, TimeSpan maintenance,
            IEnumerable<IAbility> abilities)
        {
            Name = name;
            Type = type;
            EngValue = engValue;
            TacValue = tacValue;
            SciValue = sciValue;
            Maintenance = maintenance;
            Abilities = new List<IAbility>(abilities);
        }

        public string Name { get; }
        public ShipType Type { get; }
        public int EngValue { get; }
        public int TacValue { get; }
        public int SciValue { get; }
        public TimeSpan Maintenance { get; }
        public List<IAbility> Abilities { get; }

        public bool IsOwned { get; set; }
        public int OneTimeUses { get; set; }
        public DateTime MaintenanceFinish { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}