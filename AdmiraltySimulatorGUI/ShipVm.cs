using System;
using AdmiraltySimulator;

namespace AdmiraltySimulatorGUI
{
    public class ShipVm : BaseVm
    {
        private readonly Ship _ship;

        public ShipVm(Ship ship)
        {
            _ship = ship;
        }

        public string Name => _ship.Name;
        public ShipType Type => _ship.Type;
        public int Eng => _ship.EngValue;
        public int Tac => _ship.TacValue;
        public int Sci => _ship.SciValue;
        public TimeSpan Maintenance => _ship.Maintenance;
        public string Abilities => string.Join(", ", _ship.Abilities);

        public bool IsOwned
        {
            get => _ship.IsOwned;
            set
            {
                _ship.IsOwned = value;
                OnPropertyChanged(nameof(IsOwned));
            }
        }

        public int OneTimeUses
        {
            get => _ship.OneTimeUses;
            set
            {
                _ship.OneTimeUses = value;
                OnPropertyChanged(nameof(OneTimeUses));
            }
        }

        public DateTime MaintenanceFinish
        {
            get => _ship.MaintenanceFinish;
            set
            {
                _ship.MaintenanceFinish = value;
                OnPropertyChanged(nameof(MaintenanceFinish));
            }
        }
    }
}