using System;
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
        private const string gridLayoutFile = "gridLayout.xml";
        private const string windowLayoutFile = "windowLayout.ini";
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
            _mainView = new MainWindow { DataContext = _mainVm };
            _mainView.Loaded += (sender, args) =>
            {
                LoadWindowLayout(logger);

                if (File.Exists(gridLayoutFile))
                {
                    _mainVm.LoadGrid(gridLayoutFile);
                }
            };
            _mainView.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                var cfg = new WindowLayoutConfig()
                {
                    WindowLeft = _mainView.Left,
                    WindowTop = _mainView.Top,
                    WindowWidth = _mainView.Width,
                    WindowHeight = _mainView.Height,
                    LogHeight = _mainView.LogRow.ActualHeight,
                    ShipsSectionWidth = _mainView.ShipsColumn.ActualWidth,
                    AssignmentSectionWidth = _mainView.AssignmentColumn.ActualWidth,
                    IsMaximized = _mainView.WindowState == WindowState.Maximized,
                };
                _mainVm.SaveGrid(gridLayoutFile);
                File.WriteAllLines(windowLayoutFile, cfg.SaveToString());
            }
            finally
            {
                base.OnExit(e);
            }
        }

        private void LoadWindowLayout(ILogger logger)
        {
            if (!File.Exists(windowLayoutFile))
            {
                return;
            }

            try
            {
                var cfg = WindowLayoutConfig.LoadFromString(File.ReadLines(windowLayoutFile));
                _mainView.Left = cfg.WindowLeft;
                _mainView.Top = cfg.WindowTop;
                _mainView.Width = cfg.WindowWidth;
                _mainView.Height = cfg.WindowHeight;
                _mainView.LogRow.Height = new GridLength(cfg.LogHeight);
                _mainView.ShipsColumn.Width = new GridLength(cfg.ShipsSectionWidth);
                _mainView.AssignmentColumn.Width = new GridLength(cfg.AssignmentSectionWidth);
                _mainView.WindowState = cfg.IsMaximized ? WindowState.Maximized : WindowState.Normal;
            }
            catch (Exception e)
            {
                logger.WriteLine("Failed to load window layout: " + e);
            }
        }
    }
}