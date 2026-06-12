using System.Collections.Generic;
using System.Globalization;

namespace AdmiraltySimulatorGUI
{
    public class WindowLayoutConfig
    {
        public double WindowLeft { get; set; }
        public double WindowTop { get; set; }
        public double WindowWidth { get; set; }
        public double WindowHeight { get; set; }
        public double LogHeight { get; set; }
        public double ShipsSectionWidth { get; set; }
        public double AssignmentSectionWidth { get; set; }
        public bool IsMaximized { get; set; }

        public IEnumerable<string> SaveToString()
        {
            yield return $"{nameof(WindowLeft)}={WindowLeft.ToString(CultureInfo.InvariantCulture)}";
            yield return $"{nameof(WindowTop)}={WindowTop.ToString(CultureInfo.InvariantCulture)}";
            yield return $"{nameof(WindowWidth)}={WindowWidth.ToString(CultureInfo.InvariantCulture)}";
            yield return $"{nameof(WindowHeight)}={WindowHeight.ToString(CultureInfo.InvariantCulture)}";
            yield return $"{nameof(LogHeight)}={LogHeight.ToString(CultureInfo.InvariantCulture)}";
            yield return $"{nameof(ShipsSectionWidth)}={ShipsSectionWidth.ToString(CultureInfo.InvariantCulture)}";
            yield return
                $"{nameof(AssignmentSectionWidth)}={AssignmentSectionWidth.ToString(CultureInfo.InvariantCulture)}";
            yield return $"{nameof(IsMaximized)}={IsMaximized}";
        }

        public static WindowLayoutConfig LoadFromString(IEnumerable<string> lines)
        {
            var config = new WindowLayoutConfig();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var split = line.Split('=');

                if (split.Length != 2)
                {
                    continue;
                }

                switch (split[0])
                {
                    case nameof(WindowLeft):
                        config.WindowLeft = double.Parse(split[1], CultureInfo.InvariantCulture);
                        break;
                    case nameof(WindowTop):
                        config.WindowTop = double.Parse(split[1], CultureInfo.InvariantCulture);
                        break;
                    case nameof(WindowWidth):
                        config.WindowWidth = double.Parse(split[1], CultureInfo.InvariantCulture);
                        break;
                    case nameof(WindowHeight):
                        config.WindowHeight = double.Parse(split[1], CultureInfo.InvariantCulture);
                        break;
                    case nameof(LogHeight):
                        config.LogHeight = double.Parse(split[1], CultureInfo.InvariantCulture);
                        break;
                    case nameof(ShipsSectionWidth):
                        config.ShipsSectionWidth = double.Parse(split[1], CultureInfo.InvariantCulture);
                        break;
                    case nameof(AssignmentSectionWidth):
                        config.AssignmentSectionWidth = double.Parse(split[1], CultureInfo.InvariantCulture);
                        break;
                    case nameof(IsMaximized):
                        config.IsMaximized = bool.Parse(split[1]);
                        break;
                }
            }

            return config;
        }
    }
}