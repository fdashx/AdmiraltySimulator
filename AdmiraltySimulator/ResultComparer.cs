using System.Collections.Generic;
using System.Linq;

namespace AdmiraltySimulator
{
    public static class ResultOrdering
    {
        public static List<AssignmentResult> GetOrdering(this List<AssignmentResult> results,
            IEnumerable<string> ordering)
        {
            IOrderedEnumerable<AssignmentResult> ordered = null;

            foreach (var prop in ordering)
                switch (prop.ToLowerInvariant())
                {
                    case "diff":
                        ordered = ordered?.ThenBy(r => r.TotalDiff) ?? results.OrderBy(r => r.TotalDiff);
                        break;
                    case "reward":
                        ordered = ordered?.ThenByDescending(r => r.RewardFactor) ??
                                  results.OrderByDescending(r => r.RewardFactor);
                        break;
                    case "maint":
                        ordered = ordered?.ThenBy(r => r.TotalMaint) ?? results.OrderBy(r => r.TotalMaint);
                        break;
                    case "name":
                        ordered = ordered?.ThenBy(r => r.Ships[0].Name).ThenBy(r => r.Ships[1].Name)
                                      .ThenBy(r => r.Ships[2].Name)
                                  ?? results.OrderBy(r => r.Ships[0].Name).ThenBy(r => r.Ships[1].Name)
                                      .ThenBy(r => r.Ships[2].Name);
                        break;
                    default:
                        return ordered?.ToList() ?? results;
                }

            return ordered?.ToList() ?? results;
        }
    }
}