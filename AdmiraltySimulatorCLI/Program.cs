using System;
using System.Collections.Generic;
using System.Globalization;
using AdmiraltySimulator;

namespace AdmiraltySimulatorCLI
{
    public class Program
    {
        private static string _profile;
        private static string _shipFile = "Ships.txt";
        private static string _assignmentFile;
        private static double _minSuccess;
        private static int _numResults = 5;
        private static string[] _orderBy = {"reward", "maint", "diff"};

        public static void Main(string[] args)
        {
            if (!ParseArgs(args))
            {
                PrintHelp();
                Console.WriteLine("\nPress any key to quit.");
                Console.ReadLine();
                return;
            }

            var logger = new ConsoleLogger();
            var shipManager = new ShipManager(logger);

            if (shipManager.LoadShips(_shipFile))
            {
                shipManager.LoadOwned(_profile + "_owned.txt");
                shipManager.LoadOneTimeUse(_profile + "_onetime.txt");
            }
            else
            {
                Console.WriteLine("\nPress any key to quit.");
                Console.ReadLine();
                return;
            }

            var assignmentParser = new AssignmentParser(logger);
            var simulator = new AssignmentSimulator(logger, shipManager, _minSuccess);

            if (!string.IsNullOrWhiteSpace(_assignmentFile))
            {
                // multiple assignment analysis mode
                var assignments = assignmentParser.GetAssignments(_assignmentFile);

                foreach (var assignment in assignments)
                    PrintResults(assignment, simulator.GetTop(simulator.GetResults(assignment), _orderBy, _numResults));

                Console.WriteLine("\nPress any key to quit.");
                Console.ReadLine();
                return;
            }

            // progressive analysis mode
            Console.WriteLine("\nBegin enter assignment in format:");
            Console.WriteLine(
                "(Assignment name),(required eng),(required tac),(required sci),(event mod eng),(event mod tac),(event mod sci),(event mod crit),(event maint off),(duration),[crit reward mult]");

            while (true)
            {
                Console.Write("Enter assignment or 'q' to save and quit: ");
                var assignmentLine = Console.ReadLine();

                if (assignmentLine?.ToLower() == "q")
                    SaveAndQuit(shipManager);

                var assignment = assignmentParser.GetAssignment(assignmentLine);

                if (assignment == null) continue;

                var results = simulator.GetTop(simulator.GetResults(assignment), _orderBy, _numResults);
                PrintResults(assignment, results);
                Console.Write(
                    "Enter result rank to execute that assignment, 'q' to save and quit, any other key to skip: ");
                var input = Console.ReadLine();

                if (input?.ToLowerInvariant() == "q")
                    SaveAndQuit(shipManager);

                if (int.TryParse(input, out var executeNum) && executeNum > 0 &&
                    executeNum <= Math.Min(_numResults, results.Count))
                    simulator.ExecuteResult(results[executeNum - 1]);
            }
        }

        private static void SaveAndQuit(ShipManager shipManager)
        {
            var input = "q";

            while (input?.ToLowerInvariant() == "q")
            {
                if (shipManager.SaveShips(_profile + "_owned.txt", _profile + "_onetime.txt"))
                    Environment.Exit(0);

                Console.WriteLine("Failed to save profile, 'q' to try again, or any other key to quit");
                input = Console.ReadLine();
            }

            Environment.Exit(0);
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine(AppDomain.CurrentDomain.FriendlyName + " (profile name) [optional args]");
            Console.WriteLine("\nOptional args are:");
            Console.WriteLine(
                "\t-ships [file path]\t\t\tfile with ship information, default is \"ships.txt\" in the exe location");
            Console.WriteLine("\t-assignments [assignments path]\t\tanalyse all assignments in file individually");
            Console.WriteLine(
                "\t-minSuccess [value]\t\t\tspecify the minimum success rate (between 0 to 1) for the result to be listed");
            Console.WriteLine("\t-numResults [value]\t\t\tspecify the top number of results to list, default is 5");
            Console.WriteLine(
                "\t-orderBy [val1,val2,val3...]\t\tspecify how to sort result by, default is reward then maint then diff");
            Console.WriteLine(
                "\t-oneTimeShipMaint [value]\t\tgive a maintenance value to one time use ships, max 23h59m");
        }

        private static bool ParseArgs(string[] args)
        {
            if (args.Length > 0)
                _profile = args[0];
            else
                return false;

            for (var i = 1; i < args.Length; i++)
                try
                {
                    switch (args[i].ToLower())
                    {
                        case "-minsuccess":
                            _minSuccess = double.Parse(args[++i]);
                            break;
                        case "-ships":
                            _shipFile = args[++i];
                            break;
                        case "-assignments":
                            _assignmentFile = args[++i];
                            break;
                        case "-numresults":
                            _numResults = int.Parse(args[++i]);
                            break;
                        case "-orderby":
                            _orderBy = args[++i].Split(',');
                            break;
                        case "-onetimeshipmaint":
                            var timeSpanFmt = new[] {"h'h'm'm'", "h'h'", "m'm'"};
                            Ship.OneTimeShipMaint =
                                TimeSpan.ParseExact(args[++i], timeSpanFmt, CultureInfo.InvariantCulture);
                            break;
                        default:
                            return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }

            return true;
        }

        private static void PrintResults(Assignment assignment, IReadOnlyList<AssignmentResult> results)
        {
            var s = assignment.Name + ", Crit reward " + Math.Round(assignment.CritRewardMult, 2) + "x" + "\n";

            for (var i = 0; i < results.Count; i++)
            {
                var r = results[i];

                s += "\n\t" + (i + 1) + ". Success: " + Math.Round(r.Success * 100, 2)
                     + "%, Critical: " + Math.Round(r.CritChance * 100, 2)
                     + "%, Reward factor: " + Math.Round(r.RewardFactor, 4)
                     + ", Total difference: " + r.TotalDiff
                     + ", Total maintenance: " + r.TotalMaint.ToString("d'd'h'h'm'm'") + "\n";
                s += "\t\tEng: " + r.EngSlotted + "/" + r.EngRequired + "\n";
                s += "\t\tTac: " + r.TacSlotted + "/" + r.TacRequired + "\n";
                s += "\t\tSci: " + r.SciSlotted + "/" + r.SciRequired + "\n";

                for (var j = 0; j < r.Ships.Count; j++)
                {
                    var ship = r.Ships[j];
                    s += "\t\t - " + ship.Name;

                    if (ship.Type != ShipType.None)
                    {
                        s += ", " + r.ShipsMaint[j].ToString("h'h'm'm'");

                        if (r.ShipIsOneTime[j])
                            s += ", use one time";
                    }

                    s += "\n";
                }
            }

            Console.WriteLine(s + "\n");
        }
    }
}