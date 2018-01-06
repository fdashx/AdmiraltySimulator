using System.Collections.Generic;
using System.IO;
using System.Windows;
using AdmiraltySimulator;

namespace AdmiraltySimulatorGUI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow _mainView;
        private MainVm _mainVm;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var logger = new ListLogger();
            var shipManager = new ShipManager(logger);
            var fileDialogService = new FileDialogService();
            _mainVm = new MainVm(logger, fileDialogService, shipManager, new AssignmentParser(logger),
                new AssignmentSimulator(logger, shipManager));
            _mainView = new MainWindow {DataContext = _mainVm};
            LoadWindowLayout();
            _mainView.Show();
            _mainVm.LoadGrid("gridLayout.xml");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            var settings = new List<string>
            {
                "window.position=" + _mainView.Left + "," + _mainView.Top + "," + _mainView.Width + "," +
                _mainView.Height,
                "log.height=" + _mainView.LogRow.Height.Value,
                "ships.section.width=" + _mainView.ShipsColumn.Width.Value,
                "assignment.section.width=" + _mainView.AssignmentColumn.Width.Value
            };

            try
            {
                File.WriteAllLines("windowLayout.ini", settings);
                _mainVm.SaveGrid("gridLayout.xml");
            }
            catch
            {
            }
        }

        private void LoadWindowLayout()
        {
            try
            {
                foreach (var line in File.ReadAllLines("windowLayout.ini"))
                {
                    var idx = line.IndexOf("=");

                    if (idx < 0)
                        continue;

                    if (line.StartsWith("window.position"))
                    {
                        var pos = line.Substring(idx + 1).Split(',');
                        _mainView.Left = int.Parse(pos[0]);
                        _mainView.Top = int.Parse(pos[1]);
                        _mainView.Width = int.Parse(pos[2]);
                        _mainView.Height = int.Parse(pos[3]);
                    }
                    else if (line.StartsWith("log.height"))
                    {
                        _mainView.LogRow.Height = new GridLength(double.Parse(line.Substring(idx + 1)));
                    }
                    else if (line.StartsWith("ships.section.width"))
                    {
                        _mainView.ShipsColumn.Width = new GridLength(double.Parse(line.Substring(idx + 1)));
                    }
                    else if (line.StartsWith("assignment.section.width"))
                    {
                        _mainView.AssignmentColumn.Width = new GridLength(double.Parse(line.Substring(idx + 1)));
                    }
                }
            }
            catch
            {
            }
        }
    }
}