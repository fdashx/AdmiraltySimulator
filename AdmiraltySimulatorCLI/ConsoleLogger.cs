using System;
using AdmiraltySimulator;

namespace AdmiraltySimulatorCLI
{
    public class ConsoleLogger : ILogger
    {
        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }
    }
}