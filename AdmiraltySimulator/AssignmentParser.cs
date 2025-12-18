using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AdmiraltySimulator
{
    public class AssignmentParser
    {
        private static string[] _timeSpanFmt = { "h'h'm'm'", "h'h'", "m'm'" };
        private readonly ILogger _logger;

        public AssignmentParser(ILogger logger)
        {
            _logger = logger;
        }

        public List<AssignmentInstance> GetAssignments(string assignmentFile)
        {
            var assignments = new List<AssignmentInstance>();
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

        public AssignmentInstance GetAssignment(string line)
        {
            var vals = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (vals.Length < 10)
            {
                _logger.WriteLine("Incomplete assignment info: " + line);
                return null;
            }

            var name = vals[0].Trim();


            if (!ParseUtil.TryInt(vals[1], out var engReq)
                || !ParseUtil.TryInt(vals[2], out var tacReq)
                || !ParseUtil.TryInt(vals[3], out var sciReq)
                || !ParseUtil.TryInt(vals[4], out var engMod)
                || !ParseUtil.TryInt(vals[5], out var tacMod)
                || !ParseUtil.TryInt(vals[6], out var sciMod)
                || !ParseUtil.TryInt(vals[7], out var critMod)
                || !ParseUtil.TryInt(vals[8], out var maintOff)
                || !TimeSpan.TryParseExact(vals[9].Trim(), _timeSpanFmt, CultureInfo.InvariantCulture,
                    out var duration))
            {
                _logger.WriteLine("Invalid assignment info: " + line);
                return null;
            }

            var critRewardMult = 1.5;

            if (vals.Length > 10)
            {
                ParseUtil.TryDouble(vals[10].Trim(), out critRewardMult);
            }

            var assignment = new Assignment()
            {
                Name = name,
                ReqEng = engReq,
                ReqTac = tacReq,
                ReqSci = sciReq,
                Duration = duration,
            };
            var assignmentEvent = new Event()
            {
                ModEng = engMod,
                ModTac = tacMod,
                ModSci = sciMod,
                ModCrit = critMod,
                MaintOff = maintOff,
            };
            return new AssignmentInstance(assignment, assignmentEvent)
            {
                CritRewardMult = critRewardMult
            };
        }

        public List<Assignment> LoadAssignmentsFromFile(string assignmentsFilePath)
        {
            var assignments = new List<Assignment>();

            try
            {
                assignments.AddRange(File.ReadLines(assignmentsFilePath).Skip(1)
                    .Select(line => line.Split(','))
                    .Select(fields =>
                    {
                        try
                        {
                            return new Assignment()
                            {
                                Faction = (Faction)Enum.Parse(typeof(Faction), fields[0].Trim(), true),
                                Rarity = (Rarity)Enum.Parse(typeof(Rarity), fields[1].Trim().Replace(" ", ""), true),
                                Name = fields[2].Trim(),
                                ReqEng = ParseUtil.Int(fields[3]),
                                ReqTac = ParseUtil.Int(fields[4]),
                                ReqSci = ParseUtil.Int(fields[5]),
                                RewardCxp = ParseUtil.Int(fields[6]),
                                RewardXp = ParseUtil.Int(fields[7]),
                                RewardDilithium = ParseUtil.Int(fields[8]),
                                RewardEc = ParseUtil.Int(fields[9]),
                                RewardOther = fields[10].Trim(),
                                Duration = TimeSpan.ParseExact(fields[11].Trim().Replace(" ", ""), _timeSpanFmt,
                                    CultureInfo.InvariantCulture),
                                HasCriticalReward = bool.Parse(fields[12].Trim()),
                            };
                        }
                        catch
                        {
                            _logger.WriteLine("Invalid line:\n" + string.Join(",", fields));
                            throw;
                        }
                    }));
                _logger.WriteLine($"Loaded {assignments.Count} assignments from \"{assignmentsFilePath}\"");
            }
            catch (Exception e)
            {
                _logger.WriteLine($"Cannot load assignment file \"{assignmentsFilePath}\":\n{e}");
            }

            return assignments;
        }

        public List<Event> LoadEventsFromFile(string eventsFilePath)
        {
            var events = new List<Event>();

            try
            {
                events.AddRange(File.ReadLines(eventsFilePath).Skip(1).Select(line => line.Split(',')).Select(fields =>
                {
                    try
                    {
                        return new Event()
                        {
                            Name = fields[0].Trim(),
                            ModEng = ParseUtil.Int(fields[1]),
                            ModTac = ParseUtil.Int(fields[2]),
                            ModSci = ParseUtil.Int(fields[3]),
                            ModCrit = ParseUtil.Int(fields[4]),
                            MaintOff = ParseUtil.Int(fields[5].TrimEnd('%').TrimStart('-')),
                            Reward = fields[6].Trim(),
                            RewardEc = ParseUtil.Int(fields[7]),
                            RewardDilithium = ParseUtil.Int(fields[8]),
                        };
                    }
                    catch
                    {
                        _logger.WriteLine("Invalid line:\n" + string.Join(",", fields));
                        throw;
                    }
                }));
                _logger.WriteLine($"Loaded {events.Count} events from \"{eventsFilePath}\"");
            }
            catch (Exception e)
            {
                _logger.WriteLine($"Cannot load events file \"{eventsFilePath}\":\n{e}");
            }

            return events;
        }
    }
}