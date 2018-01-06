using System.Linq;

namespace AdmiraltySimulator
{
    public interface IAbility
    {
        void Apply(AssignmentResult assignment, Ship abilityShip);
    }

    public class IgnoreEventMod : IAbility
    {
        public IgnoreEventMod(StatType type)
        {
            StatType = type;
        }

        public StatType StatType { get; }

        public void Apply(AssignmentResult assignment, Ship abilityShip)
        {
            switch (StatType)
            {
                case StatType.Eng:
                    assignment.EngEventMod = 0;
                    break;
                case StatType.Tac:
                    assignment.TacEventMod = 0;
                    break;
                case StatType.Sci:
                    assignment.SciEventMod = 0;
                    break;
                case StatType.All:
                    assignment.EngEventMod = 0;
                    assignment.TacEventMod = 0;
                    assignment.SciEventMod = 0;
                    break;
            }
        }

        public override string ToString()
        {
            return "Ignore +/- " + StatType;
        }
    }

    public class BonusStatPerShip : IAbility
    {
        public BonusStatPerShip(StatType statType, int value, ShipType shipType)
        {
            StatType = statType;
            Value = value;
            ShipType = shipType;
        }

        public StatType StatType { get; }
        public int Value { get; }
        public ShipType ShipType { get; }

        public void Apply(AssignmentResult assignment, Ship abilityShip)
        {
            var bonusValue = 0;

            foreach (var ship in assignment.Ships)
                if (ship != abilityShip
                    && (ship.Type == ShipType || ShipType == ShipType.Any && ship.Type != ShipType.Small &&
                        ship.Type != ShipType.None))
                    bonusValue += Value;

            switch (StatType)
            {
                case StatType.Eng:
                    assignment.EngSlotted += bonusValue;
                    break;
                case StatType.Tac:
                    assignment.TacSlotted += bonusValue;
                    break;
                case StatType.Sci:
                    assignment.SciSlotted += bonusValue;
                    break;
                case StatType.All:
                    assignment.EngSlotted += bonusValue;
                    assignment.TacSlotted += bonusValue;
                    assignment.SciSlotted += bonusValue;
                    break;
            }
        }

        public override string ToString()
        {
            return "+" + Value + " " + StatType + " per " + ShipType + " ship";
        }
    }

    public class BonusStatAlone : IAbility
    {
        public BonusStatAlone(StatType statType, int value)
        {
            StatType = statType;
            Value = value;
        }

        public StatType StatType { get; }
        public int Value { get; }

        public void Apply(AssignmentResult assignment, Ship abilityShip)
        {
            if (assignment.Ships.Count(s => s.Type != ShipType.None) == 1)
                switch (StatType)
                {
                    case StatType.Eng:
                        assignment.EngSlotted += Value;
                        break;
                    case StatType.Tac:
                        assignment.TacSlotted += Value;
                        break;
                    case StatType.Sci:
                        assignment.SciSlotted += Value;
                        break;
                    case StatType.All:
                        assignment.EngSlotted += Value;
                        assignment.TacSlotted += Value;
                        assignment.SciSlotted += Value;
                        break;
                }
        }

        public override string ToString()
        {
            return "+" + Value + " " + StatType + " when alone";
        }
    }

    public class BonusCritStat : IAbility
    {
        public BonusCritStat(StatType statType, double value)
        {
            StatType = statType;
            Value = value - 1;
        }

        public StatType StatType { get; }
        public double Value { get; }

        public void Apply(AssignmentResult assignment, Ship abilityShip)
        {
            switch (StatType)
            {
                case StatType.Eng:
                    assignment.EngCritMult += Value;
                    break;
                case StatType.Tac:
                    assignment.TacCritMult += Value;
                    break;
                case StatType.Sci:
                    assignment.SciCritMult += Value;
                    break;
                case StatType.All:
                    assignment.EngCritMult += Value;
                    assignment.TacCritMult += Value;
                    assignment.SciCritMult += Value;
                    break;
            }
        }

        public override string ToString()
        {
            return "x" + (Value + 1) + " critical ratings from " + StatType;
        }
    }

    public class BonusCritEvent : IAbility
    {
        public BonusCritEvent(double value)
        {
            Value = value - 1;
        }

        public double Value { get; }

        public void Apply(AssignmentResult assignment, Ship abilityShip)
        {
            assignment.EventCritMult += Value;
        }

        public override string ToString()
        {
            return "x" + (Value + 1) + " critical ratings from events";
        }
    }

    public class MaintOffPerShip : IAbility
    {
        public MaintOffPerShip(ShipType shipType, int value)
        {
            Value = value;
            ShipType = shipType;
        }

        public int Value { get; }
        public ShipType ShipType { get; }

        public void Apply(AssignmentResult assignment, Ship abilityShip)
        {
            var bonusValue = 0;

            foreach (var ship in assignment.Ships)
                if (ship != abilityShip
                    && (ship.Type == ShipType || ShipType == ShipType.Any && ship.Type != ShipType.Small &&
                        ship.Type != ShipType.None))
                    bonusValue += Value;

            assignment.MaintOff += bonusValue;
        }

        public override string ToString()
        {
            return "-" + Value + "% off maintenance per " + ShipType + " ship";
        }
    }
}