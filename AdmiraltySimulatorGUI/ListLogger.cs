using System;
using System.Collections.ObjectModel;
using AdmiraltySimulator;

namespace AdmiraltySimulatorGUI
{
    public class ListLogger : ILogger
    {
        public ListLogger()
        {
            Logs = new ObservableCollection<string>();
        }

        public ObservableCollection<string> Logs { get; }

        public void WriteLine(string line)
        {
            Logs.Insert(0, DateTime.Now.ToString("HH:mm:ss.ffff") + " - " + line);
        }
    }
}