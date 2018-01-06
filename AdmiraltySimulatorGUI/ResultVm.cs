using System;
using System.Collections.Generic;
using AdmiraltySimulator;

namespace AdmiraltySimulatorGUI
{
    public class ResultVm : BaseVm
    {
        public ResultVm(AssignmentResult result)
        {
            Result = result;
            var ships = new List<string>();
            var shipsMaint = new List<string>();

            for (var i = 0; i < Result.Ships.Count; i++)
            {
                var ship = Result.Ships[i];
                var s = ship.Name;

                if (ship.Type != ShipType.None && Result.ShipIsOneTime[i])
                    s = "(1x)" + s;

                ships.Add(s);
                shipsMaint.Add(Result.ShipsMaint[i].ToString("h'h'm'm'"));
            }

            Ships = string.Join(", ", ships);
            ShipsMaint = string.Join(", ", shipsMaint);
        }

        public AssignmentResult Result { get; }
        public int EngRequired => Result.EngRequired;
        public int TacRequired => Result.TacRequired;
        public int SciRequired => Result.SciRequired;
        public int EngSlotted => Result.EngSlotted;
        public int TacSlotted => Result.TacSlotted;
        public int SciSlotted => Result.SciSlotted;
        public int TotalSlotted => Result.TotalSlotted;
        public int EngEventMod => Result.EngEventMod;
        public int TacEventMod => Result.TacEventMod;
        public int SciEventMod => Result.SciEventMod;
        public string Ships { get; }
        public double Success => Math.Round(Result.Success * 100, 2);
        public int EngDiff => Result.EngDiff;
        public int TacDiff => Result.TacDiff;
        public int SciDiff => Result.SciDiff;
        public int TotalDiff => Result.TotalDiff;
        public double EngCritMult => Math.Round(Result.EngCritMult, 1);
        public double TacCritMult => Math.Round(Result.TacCritMult, 1);
        public double SciCritMult => Math.Round(Result.SciCritMult, 1);
        public double EventCritMult => Math.Round(Result.EventCritMult, 1);
        public int MaintOff => Result.MaintOff;
        public TimeSpan Duration => Result.Duration;
        public double CritChance => Math.Round(Result.CritChance * 100, 2);
        public double RewardFactor => Math.Round(Result.RewardFactor, 4);
        public string ShipsMaint { get; }
        public TimeSpan TotalMaint => Result.TotalMaint;
        public int TotalCrit => Result.TotalCrit;
    }
}