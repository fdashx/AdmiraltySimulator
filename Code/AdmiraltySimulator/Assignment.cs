using System;
using System.Collections.Generic;
using System.Linq;

namespace AdmiraltySimulator
{
    public class Assignment
    {
        public Assignment(string name, int engReq, int tacReq, int sciReq, int engMod, int tacMod, int sciMod,
            int critMod, int maintOff, TimeSpan duration)
        {
            Name = name;
            EngRequired = engReq;
            TacRequired = tacReq;
            SciRequired = sciReq;
            EngEventMod = engMod;
            TacEventMod = tacMod;
            SciEventMod = sciMod;
            CritMod = critMod;
            MaintOff = maintOff;
            Duration = duration;
            CritRewardMult = 1.5;
        }

        public string Name { get; }
        public int EngRequired { get; }
        public int TacRequired { get; }
        public int SciRequired { get; }
        public int EngEventMod { get; }
        public int TacEventMod { get; }
        public int SciEventMod { get; }
        public int CritMod { get; }
        public int MaintOff { get; }
        public TimeSpan Duration { get; }
        public double CritRewardMult { get; set; }

        public AssignmentResult Start(IEnumerable<Ship> ships)
        {
            var result = new AssignmentResult(ships)
            {
                EngEventMod = EngEventMod,
                TacEventMod = TacEventMod,
                SciEventMod = SciEventMod,
                MaintOff = MaintOff,
                Duration = Duration
            };

            foreach (var ship in ships)
            {
                foreach (var ability in ship.Abilities)
                    ability.Apply(result, ship);

                result.EngSlotted += ship.EngValue;
                result.TacSlotted += ship.TacValue;
                result.SciSlotted += ship.SciValue;
            }

            // success chance
            result.EngRequired = Math.Max(EngRequired + result.EngEventMod, 0);
            result.TacRequired = Math.Max(TacRequired + result.TacEventMod, 0);
            result.SciRequired = Math.Max(SciRequired + result.SciEventMod, 0);
            var totalRequired = result.EngRequired + result.TacRequired + result.SciRequired;
            var slotted = Math.Min(result.EngSlotted, result.EngRequired)
                          + Math.Min(result.TacSlotted, result.TacRequired)
                          + Math.Min(result.SciSlotted, result.SciRequired);
            result.Success = (double) slotted / totalRequired;

            // slotted and required difference
            result.EngDiff = result.EngSlotted - result.EngRequired;
            result.TacDiff = result.TacSlotted - result.TacRequired;
            result.SciDiff = result.SciSlotted - result.SciRequired;
            result.TotalDiff = Math.Abs(result.EngDiff) + Math.Abs(result.TacDiff) + Math.Abs(result.SciDiff);
            result.TotalSlotted = result.EngSlotted + result.TacSlotted + result.SciSlotted;

            // critical chance, decimals are rounded down
            var engCrit = (int) (Math.Max(result.EngDiff, 0) * (1 + result.EngCritMult));
            var tacCrit = (int) (Math.Max(result.TacDiff, 0) * (1 + result.TacCritMult));
            var sciCrit = (int) (Math.Max(result.SciDiff, 0) * (1 + result.SciCritMult));
            result.TotalCrit = (int) (engCrit + tacCrit + sciCrit + CritMod * (1 + result.EventCritMult));
            result.CritChance = (double) result.TotalCrit / (result.TotalCrit + 2 * totalRequired);
            result.RewardFactor = result.Success * (1 - result.CritChance * (1 - CritRewardMult));

            // maintenance
            for (var i = 0; i < result.ShipsMaint.Count; i++)
            {
                result.ShipsMaint[i] =
                    TimeSpan.FromMinutes((100 - Math.Min(result.MaintOff, 100)) / 100.0 *
                                         result.ShipsMaint[i].TotalMinutes);
                result.TotalMaint += result.ShipsMaint[i];
            }

            return result;
        }
    }

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