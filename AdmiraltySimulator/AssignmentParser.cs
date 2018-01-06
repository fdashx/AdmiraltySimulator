using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AdmiraltySimulator
{
    public class AssignmentParser
    {
        private readonly ILogger _logger;

        public AssignmentParser(ILogger logger)
        {
            _logger = logger;
        }

        public List<Assignment> GetAssignments(string assignmentFile)
        {
            var assignments = new List<Assignment>();
            List<string> assignmentInfo;

            try
            {
                assignmentInfo = new List<string>(File.ReadAllLines(assignmentFile));
            }
            catch (Exception e)
            {
                _logger.WriteLine("Cannot load assignment file:\n" + e);
                return assignments;
            }

            foreach (var line in assignmentInfo)
            {
                if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
                    continue;

                var assignment = GetAssignment(line);

                if (assignment != null)
                    assignments.Add(assignment);
            }

            return assignments;
        }

        public Assignment GetAssignment(string line)
        {
            var vals = line.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

            if (vals.Length < 10)
            {
                _logger.WriteLine("Incomplete assignment info: " + line);
                return null;
            }

            var name = vals[0].Trim();
            var timeSpanFmt = new[] {"h'h'm'm'", "h'h'", "m'm'"};

            if (!int.TryParse(vals[1].Trim(), out var engReq)
                || !int.TryParse(vals[2].Trim(), out var tacReq)
                || !int.TryParse(vals[3].Trim(), out var sciReq)
                || !int.TryParse(vals[4].Trim(), out var engMod)
                || !int.TryParse(vals[5].Trim(), out var tacMod)
                || !int.TryParse(vals[6].Trim(), out var sciMod)
                || !int.TryParse(vals[7].Trim(), out var critMod)
                || !int.TryParse(vals[8].Trim(), out var maintOff)
                || !TimeSpan.TryParseExact(vals[9].Trim(), timeSpanFmt, CultureInfo.InvariantCulture, out var duration))
            {
                _logger.WriteLine("Invalid assignment info: " + line);
                return null;
            }

            var critRewardMult = 1.5;

            if (vals.Length > 10)
                double.TryParse(vals[10].Trim(), out critRewardMult);

            return new Assignment(name, engReq, tacReq, sciReq, engMod, tacMod, sciMod, critMod, maintOff, duration)
            {
                CritRewardMult = critRewardMult
            };
        }
    }
}