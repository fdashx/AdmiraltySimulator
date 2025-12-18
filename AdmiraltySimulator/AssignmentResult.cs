using System;
using System.Collections.Generic;
using System.Linq;

namespace AdmiraltySimulator
{
    public class AssignmentResult
    {
        public AssignmentResult(IEnumerable<Ship> ships)
        {
            Ships = new List<Ship>(ships);
            ShipsMaint = new List<TimeSpan>();
            ShipIsOneTime = new List<bool>();
            var ownedShipsUsed = new List<string>();

            foreach (var ship in Ships)
                if (ship.Type == ShipType.None)
                {
                    ShipsMaint.Add(TimeSpan.Zero);
                    ShipIsOneTime.Add(true);
                }
                else if (ship.IsOwned && !ownedShipsUsed.Contains(ship.Name) && ship.MaintenanceFinish < DateTime.Now)
                {
                    ShipsMaint.Add(ship.Maintenance);
                    ownedShipsUsed.Add(ship.Name);
                    ShipIsOneTime.Add(false);
                }
                else
                {
                    ShipsMaint.Add(Ship.OneTimeShipMaint);
                    ShipIsOneTime.Add(true);
                }
        }

        public int EngRequired { get; set; }
        public int TacRequired { get; set; }
        public int SciRequired { get; set; }
        public int EngSlotted { get; set; }
        public int TacSlotted { get; set; }
        public int SciSlotted { get; set; }
        public int TotalSlotted { get; set; }
        public int EngEventMod { get; set; }
        public int TacEventMod { get; set; }
        public int SciEventMod { get; set; }
        public List<Ship> Ships { get; }
        public double Success { get; set; }
        public int EngDiff { get; set; }
        public int TacDiff { get; set; }
        public int SciDiff { get; set; }
        public int TotalDiff { get; set; }
        public double EngCritMult { get; set; }
        public double TacCritMult { get; set; }
        public double SciCritMult { get; set; }
        public double EventCritMult { get; set; }
        public int MaintOff { get; set; }
        public TimeSpan Duration { get; set; }
        public double CritChance { get; set; }
        public double RewardFactor { get; set; }
        public List<TimeSpan> ShipsMaint { get; }
        public TimeSpan TotalMaint { get; set; }
        public int TotalCrit { get; set; }
        public List<bool> ShipIsOneTime { get; }

        public override string ToString()
        {
            return "Success: " + Math.Round(Success * 100, 2)
                               + ", Critical: " + Math.Round(CritChance * 100, 2)
                               + ", Total diff: " + TotalDiff
                               + ", Ships: " + string.Join(", ", Ships.Select(s => s.Name));
        }
    }
}