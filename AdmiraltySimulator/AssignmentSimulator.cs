using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AdmiraltySimulator
{
    public class AssignmentSimulator
    {
        private readonly ILogger _logger;
        private readonly double _minSuccess;
        private readonly ShipManager _shipManager;
        private bool _calcCombination;
        private List<List<int>> _shipCombinations;
        private List<Ship> _ships;

        public AssignmentSimulator(ILogger logger, ShipManager shipManager, double minSuccess = 0)
        {
            _logger = logger;
            _shipManager = shipManager;
            _calcCombination = true;
            _minSuccess = minSuccess;
        }

        public List<AssignmentResult> GetResults(AssignmentInstance assignmentInstance, bool recalcCombination = false)
        {
            var sw = Stopwatch.StartNew();

            if (_calcCombination || recalcCombination)
            {
                var indexes = new List<int>();
                _ships = _shipManager.GetAvailableShips();

                for (var i = 0; i < _ships.Count; i++)
                    indexes.Add(i);

                _shipCombinations = indexes.Combinations(3);
                _calcCombination = false;
            }

            var results = new List<AssignmentResult>();

            foreach (var combination in _shipCombinations)
                results.Add(assignmentInstance.Start(new[]
                {
                    _ships[combination[0]],
                    _ships[combination[1]],
                    _ships[combination[2]]
                }));

            _logger.WriteLine("Analysed " + results.Count + " ship combinations in " + sw.ElapsedMilliseconds +
                              " milliseconds.");
            return results;
        }

        public List<AssignmentResult> GetTop(List<AssignmentResult> results, IEnumerable<string> orders, int count)
        {
            var sw = Stopwatch.StartNew();
            var orderedResults = results.GetOrdering(orders);
            var topResults = new List<AssignmentResult>();

            for (var i = 0; topResults.Count < count && i < orderedResults.Count; i++)
            {
                var currResult = orderedResults[i];

                if (topResults.Count > 0)
                {
                    var latest = topResults[topResults.Count - 1];

                    if (latest.TotalSlotted == currResult.TotalSlotted
                        && latest.Ships[0].Name == currResult.Ships[0].Name
                        && latest.Ships[1].Name == currResult.Ships[1].Name
                        && latest.Ships[2].Name == currResult.Ships[2].Name)
                        continue;
                }

                if (!(currResult.Success > _minSuccess))
                    continue;

                topResults.Add(currResult);
            }

            _logger.WriteLine("Sorted and filtered " + orderedResults.Count + " results in " + sw.ElapsedMilliseconds +
                              " milliseconds.");
            return topResults;
        }

        public void ExecuteResult(AssignmentResult result)
        {
            for (var i = 0; i < result.Ships.Count; i++)
            {
                if (result.Ships[i].Type == ShipType.None)
                    continue;

                if (result.Ships[i].IsOwned && result.Ships[i].MaintenanceFinish < DateTime.Now)
                    result.Ships[i].MaintenanceFinish = DateTime.Now + result.Duration + result.ShipsMaint[i];
                else if (result.Ships[i].OneTimeUses > 0)
                    result.Ships[i].OneTimeUses--;
                else
                    _logger.WriteLine("Error: " + result.Ships[i].Name + " is no longer available.");
            }

            _calcCombination = true;
        }
    }
}