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
                {
                    continue;
                }

                var assignment = GetAssignment(line);

                if (assignment != null)
                {
                    assignments.Add(assignment);
                }
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

            if (!TryParseInvariant.Int(vals[1].Trim(), out var engReq)
                || !TryParseInvariant.Int(vals[2].Trim(), out var tacReq)
                || !TryParseInvariant.Int(vals[3].Trim(), out var sciReq)
                || !TryParseInvariant.Int(vals[4].Trim(), out var engMod)
                || !TryParseInvariant.Int(vals[5].Trim(), out var tacMod)
                || !TryParseInvariant.Int(vals[6].Trim(), out var sciMod)
                || !TryParseInvariant.Int(vals[7].Trim(), out var critMod)
                || !TryParseInvariant.Int(vals[8].Trim(), out var maintOff)
                || !TimeSpan.TryParseExact(vals[9].Trim(), timeSpanFmt, CultureInfo.InvariantCulture, out var duration))
            {
                _logger.WriteLine("Invalid assignment info: " + line);
                return null;
            }

            var critRewardMult = 1.5;

            if (vals.Length > 10)
            {
                TryParseInvariant.Double(vals[10].Trim(), out critRewardMult);
            }

            return new Assignment(name, engReq, tacReq, sciReq, engMod, tacMod, sciMod, critMod, maintOff, duration)
            {
                CritRewardMult = critRewardMult
            };
        }
    }
}