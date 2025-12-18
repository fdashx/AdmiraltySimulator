using System;
using System.Collections.Generic;

namespace AdmiraltySimulator
{
    public class AssignmentInstance
    {
        public AssignmentInstance(Assignment assignment, Event assignmentEvent)
        {
            Assignment = assignment;
            Event = assignmentEvent;
            CritRewardMult = Assignment.HasCriticalReward ? 1.5 : 1;
        }

        public Assignment Assignment { get; }
        public Event Event { get; }
        public double CritRewardMult { get; set; }

        public AssignmentResult Start(IEnumerable<Ship> ships)
        {
            var result = new AssignmentResult(ships)
            {
                EngEventMod = Event.ModEng,
                TacEventMod = Event.ModTac,
                SciEventMod = Event.ModSci,
                MaintOff = Event.MaintOff,
                Duration = Assignment.Duration
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
            result.EngRequired = Math.Max(Assignment.ReqEng + result.EngEventMod, 0);
            result.TacRequired = Math.Max(Assignment.ReqTac + result.TacEventMod, 0);
            result.SciRequired = Math.Max(Assignment.ReqSci + result.SciEventMod, 0);
            var totalRequired = result.EngRequired + result.TacRequired + result.SciRequired;
            var slotted = Math.Min(result.EngSlotted, result.EngRequired)
                          + Math.Min(result.TacSlotted, result.TacRequired)
                          + Math.Min(result.SciSlotted, result.SciRequired);
            result.Success = (double)slotted / totalRequired;

            // slotted and required difference
            result.EngDiff = result.EngSlotted - result.EngRequired;
            result.TacDiff = result.TacSlotted - result.TacRequired;
            result.SciDiff = result.SciSlotted - result.SciRequired;
            result.TotalDiff = Math.Abs(result.EngDiff) + Math.Abs(result.TacDiff) + Math.Abs(result.SciDiff);
            result.TotalSlotted = result.EngSlotted + result.TacSlotted + result.SciSlotted;

            // critical chance, decimals are rounded down
            var engCrit = (int)(Math.Max(result.EngDiff, 0) * (1 + result.EngCritMult));
            var tacCrit = (int)(Math.Max(result.TacDiff, 0) * (1 + result.TacCritMult));
            var sciCrit = (int)(Math.Max(result.SciDiff, 0) * (1 + result.SciCritMult));
            result.TotalCrit = (int)(engCrit + tacCrit + sciCrit + Event.ModCrit * (1 + result.EventCritMult));
            result.CritChance = (double)result.TotalCrit / (result.TotalCrit + 2 * totalRequired);
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
}