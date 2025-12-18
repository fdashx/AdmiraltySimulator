using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdmiraltySimulator;

namespace AdmiraltySimulatorGUI
{
    public class MainVm : LayoutSaveRestore
    {
        private const string DefaultShipsFilePath = "ships.csv";
        private const string DefaultProfilesDirectory = "profiles";
        private const string DefaultAssignmentsFilePath = "assignments.csv";
        private const string DefaultEventsFilePath = "events.csv";
        private const string FileSuffixOwned = "_owned.txt";
        private const string FileSuffixOneTime = "_onetime.txt";
        private const string FmtDuration = "h'h'm'm'";
        private readonly IFileDialogService _fileDialogService;
        private string _critChance;
        private string _critRewardMult;
        private string _duration;
        private string _engMod;
        private string _engReq;
        private string _engSlot;
        private string _eventCrit;
        private string _eventMaintOff;
        private string _oneTimeShipFile;
        private string _ownedShipFile;
        private List<Profile> _profiles;
        private List<ResultVm> _results;
        private string _sciMod;
        private string _sciReq;
        private string _sciSlot;
        private ResultVm _selectedResult;
        private List<string> _selectedShips;
        private List<string> _selectedShipsMaint;
        private string _shipFile;
        private List<ShipVm> _ships;
        private string _successChance;
        private string _tacMod;
        private string _tacReq;
        private string _tacSlot;
        private string _totalCrit;
        private string _totalMaint;
        private string _totalSlot;
        private Profile _selectedProfile;
        private string _newProfileName;
        private List<Assignment> _assignments;
        private List<Event> _events;
        private Assignment _selectedAssignment;
        private Event _selectedEvent;
        private bool _useOneTimeShips;

        public MainVm(
            ILogger logger,
            IFileDialogService fileDialogService,
            ShipManager shipManager,
            AssignmentParser assignmentParser,
            AssignmentSimulator simulator
        )
        {
            Logger = logger;
            _fileDialogService = fileDialogService;
            ShipManager = shipManager;
            AssignmentParser = assignmentParser;
            Simulator = simulator;
            LoadShipFileCmd = new RelayCommand(LoadShips);
            LoadProfileCmd = new RelayCommand(LoadProfile);
            CreateNewProfileCmd = new RelayCommand(CreateNewProfile);
            SaveChangeCmd = new RelayCommand(SaveChanges);
            SimulateCmd = new RelayCommand(Simulate);
            ExecuteCmd = new RelayCommand(ExecuteResult);
            CritRewardMult = "1.5";
            Profiles = new List<Profile>();
            ResetAssignment();
            LoadShipsFile(DefaultShipsFilePath);
            LoadAvailableProfiles();
            LoadAssignmentsAndEvents();
        }

        public ILogger Logger { get; }
        public ShipManager ShipManager { get; }
        public AssignmentParser AssignmentParser { get; }
        public AssignmentSimulator Simulator { get; }

        public List<ShipVm> Ships
        {
            get => _ships;
            private set => SetProperty(ref _ships, value, nameof(Ships));
        }

        public RelayCommand LoadShipFileCmd { get; }
        public RelayCommand LoadProfileCmd { get; }
        public RelayCommand CreateNewProfileCmd { get; }
        public RelayCommand SaveChangeCmd { get; }
        public RelayCommand SimulateCmd { get; }
        public RelayCommand ExecuteCmd { get; }

        public List<ResultVm> Results
        {
            get => _results;
            set => SetProperty(ref _results, value, nameof(Results));
        }

        public ResultVm SelectedResult
        {
            get => _selectedResult;
            set
            {
                SetProperty(ref _selectedResult, value, nameof(SelectedResult));
                DisplayResult(SelectedResult);
            }
        }

        public string ShipFile
        {
            get => _shipFile;
            set => SetProperty(ref _shipFile, value, nameof(ShipFile));
        }

        public List<Profile> Profiles
        {
            get => _profiles;
            set => SetProperty(ref _profiles, value, nameof(Profiles));
        }

        public Profile SelectedProfile
        {
            get => _selectedProfile;
            set => SetProperty(ref _selectedProfile, value, nameof(SelectedProfile));
        }

        public string NewProfileName
        {
            get => _newProfileName;
            set => SetProperty(ref _newProfileName, value, nameof(NewProfileName));
        }

        public string OwnedShipFile
        {
            get => _ownedShipFile;
            set => SetProperty(ref _ownedShipFile, value, nameof(OwnedShipFile));
        }

        public string OneTimeShipFile
        {
            get => _oneTimeShipFile;
            set => SetProperty(ref _oneTimeShipFile, value, nameof(OneTimeShipFile));
        }

        public string CritRewardMult
        {
            get => _critRewardMult;
            set => SetProperty(ref _critRewardMult, value, nameof(CritRewardMult));
        }

        public string EngReq
        {
            get => _engReq;
            set => SetProperty(ref _engReq, value, nameof(EngReq));
        }

        public string EngMod
        {
            get => _engMod;
            set => SetProperty(ref _engMod, value, nameof(EngMod));
        }

        public string EngSlot
        {
            get => _engSlot;
            private set => SetProperty(ref _engSlot, value, nameof(EngSlot));
        }

        public string TacReq
        {
            get => _tacReq;
            set => SetProperty(ref _tacReq, value, nameof(TacReq));
        }

        public string TacMod
        {
            get => _tacMod;
            set => SetProperty(ref _tacMod, value, nameof(TacMod));
        }

        public string TacSlot
        {
            get => _tacSlot;
            private set => SetProperty(ref _tacSlot, value, nameof(TacSlot));
        }

        public string SciReq
        {
            get => _sciReq;
            set => SetProperty(ref _sciReq, value, nameof(SciReq));
        }

        public string SciMod
        {
            get => _sciMod;
            set => SetProperty(ref _sciMod, value, nameof(SciMod));
        }

        public string SciSlot
        {
            get => _sciSlot;
            private set => SetProperty(ref _sciSlot, value, nameof(SciSlot));
        }

        public string TotalSlot
        {
            get => _totalSlot;
            private set => SetProperty(ref _totalSlot, value, nameof(TotalSlot));
        }

        public string EventCrit
        {
            get => _eventCrit;
            set => SetProperty(ref _eventCrit, value, nameof(EventCrit));
        }

        public string TotalCrit
        {
            get => _totalCrit;
            private set => SetProperty(ref _totalCrit, value, nameof(TotalCrit));
        }

        public string EventMaintOff
        {
            get => _eventMaintOff;
            set => SetProperty(ref _eventMaintOff, value, nameof(EventMaintOff));
        }

        public string Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value, nameof(Duration));
        }

        public string TotalMaint
        {
            get => _totalMaint;
            private set => SetProperty(ref _totalMaint, value, nameof(TotalMaint));
        }

        public string SuccessChance
        {
            get => _successChance;
            private set => SetProperty(ref _successChance, value, nameof(SuccessChance));
        }

        public string CritChance
        {
            get => _critChance;
            private set => SetProperty(ref _critChance, value, nameof(CritChance));
        }

        public List<string> SelectedShips
        {
            get => _selectedShips;
            private set => SetProperty(ref _selectedShips, value, nameof(SelectedShips));
        }

        public List<string> SelectedShipsMaint
        {
            get => _selectedShipsMaint;
            private set => SetProperty(ref _selectedShipsMaint, value, nameof(SelectedShipsMaint));
        }

        public List<Assignment> Assignments
        {
            get => _assignments;
            set => SetProperty(ref _assignments, value, nameof(Assignments));
        }

        public Assignment SelectedAssignment
        {
            get => _selectedAssignment;
            set
            {
                SetProperty(ref _selectedAssignment, value, nameof(SelectedAssignment));
                PopulateAssignmentInfo(SelectedAssignment);
            }
        }

        public List<Event> Events
        {
            get => _events;
            set => SetProperty(ref _events, value, nameof(Events));
        }

        public Event SelectedEvent
        {
            get => _selectedEvent;
            set
            {
                SetProperty(ref _selectedEvent, value, nameof(SelectedEvent));
                PopulateEventInfo(SelectedEvent);
            }
        }

        public bool UseOneTimeShips
        {
            get => _useOneTimeShips;
            set => SetProperty(ref _useOneTimeShips, value, nameof(UseOneTimeShips));
        }

        public void SaveGrid(string file)
        {
            try
            {
                SaveLayout(file);
            }
            catch (Exception e)
            {
                Logger.WriteLine("Cannot save layout:\n" + e);
            }
        }

        public void LoadGrid(string file)
        {
            try
            {
                LoadLayout(file);
            }
            catch (Exception e)
            {
                Logger.WriteLine("Cannot load layout:\n" + e);
            }
        }

        private void LoadShips()
        {
            var file = _fileDialogService.OpenFile();

            if (file == null)
                return;

            LoadShipsFile(file);
        }

        private void LoadShipsFile(string filePath)
        {
            if (!ShipManager.LoadShips(filePath))
                return;

            var ships = new List<ShipVm>();

            foreach (var ship in ShipManager.GetAllShips())
                ships.Add(new ShipVm(ship));

            Ships = ships;
            ShipFile = Path.GetFullPath(filePath);
        }

        private void LoadAvailableProfiles()
        {
            try
            {
                Profiles = new List<Profile>(Directory.EnumerateFiles(DefaultProfilesDirectory, "*" + FileSuffixOwned)
                    .Select(f => new Profile
                    {
                        Name = Path.GetFileName(f).Replace(FileSuffixOwned, ""),
                        FilePath = f
                    }));
            }
            catch (Exception e)
            {
                Logger.WriteLine("Cannot load available profiles:\n" + e);
            }
        }

        private void LoadProfile()
        {
            OwnedShipFile = SelectedProfile?.FilePath;

            if (OwnedShipFile == null)
                return;

            ShipManager.LoadOwned(OwnedShipFile);
            var oneTimeShipFile = Path.Combine(Path.GetDirectoryName(SelectedProfile?.FilePath) ?? string.Empty,
                SelectedProfile.Name + FileSuffixOneTime);

            if (File.Exists(oneTimeShipFile))
            {
                OneTimeShipFile = oneTimeShipFile;
                ShipManager.LoadOneTimeUse(OneTimeShipFile);
            }

            RefreshShips();
        }

        public void CreateNewProfile()
        {
            if (string.IsNullOrWhiteSpace(NewProfileName))
            {
                Logger.WriteLine("No new profile name specified.");
                return;
            }

            var profilePathOwned = Path.Combine(DefaultProfilesDirectory, NewProfileName + FileSuffixOwned);

            if (File.Exists(profilePathOwned))
            {
                Logger.WriteLine($"Profile {NewProfileName} already exists.");
                return;
            }

            var profilePathOneTime = Path.Combine(DefaultProfilesDirectory, NewProfileName + FileSuffixOneTime);
            File.Create(profilePathOwned).Dispose();
            File.Create(profilePathOneTime).Dispose();
            Logger.WriteLine("New profile created.");
            LoadAvailableProfiles();
            SelectedProfile = Profiles.FirstOrDefault(p => p.Name == NewProfileName);
            LoadProfile();
            NewProfileName = null;
        }

        private void RefreshShips()
        {
            Ships = new List<ShipVm>(Ships);
        }

        private void SaveChanges()
        {
            ShipManager.SaveShips(OwnedShipFile, OneTimeShipFile);
        }

        public void Simulate()
        {
            var inputs = new List<string>
            {
                "assignment",
                EngReq,
                TacReq,
                SciReq,
                EngMod,
                TacMod,
                SciMod,
                EventCrit,
                EventMaintOff,
                Duration,
                CritRewardMult
            };
            var assignment = AssignmentParser.GetAssignment(string.Join(",", inputs));

            if (assignment == null)
                return;

            var resultVms = new List<ResultVm>();
            var results = Simulator.GetResults(assignment, true, UseOneTimeShips);

            foreach (var result in Simulator.GetTop(results, new[] { "name" }, results.Count))
                resultVms.Add(new ResultVm(result));

            Results = resultVms;
        }

        public void ExecuteResult()
        {
            if (SelectedResult != null)
            {
                Simulator.ExecuteResult(SelectedResult.Result);
                var assignmentName = SelectedAssignment?.Name ?? "custom assignment";
                var eventName = SelectedEvent != null ? $" with \"{SelectedEvent.Name}\"" : string.Empty;
                Logger.WriteLine($"Executed \"{assignmentName}\"{eventName} using result:\n" + SelectedResult.Result);
            }

            SelectedResult = null;
            RefreshShips();
            ResetAssignment();
            Results = new List<ResultVm>();
            SelectedAssignment = null;
            SelectedEvent = null;
        }

        private void DisplayResult(ResultVm result)
        {
            EngMod = result?.EngEventMod.ToString();
            EngSlot = result?.EngSlotted + " / " + result?.EngRequired;
            TacMod = result?.TacEventMod.ToString();
            TacSlot = result?.TacSlotted + " / " + result?.TacRequired;
            SciMod = result?.SciEventMod.ToString();
            SciSlot = result?.SciSlotted + " / " + result?.SciRequired;
            TotalSlot = result?.TotalSlotted + " / " +
                        (result?.EngRequired + result?.TacRequired + result?.SciRequired);
            TotalCrit = result?.TotalCrit.ToString();
            TotalMaint = result?.TotalMaint.ToString("d'd'h'h'm'm'");
            SuccessChance = result?.Success + "%";
            CritChance = result?.CritChance + "%";
            SelectedShips = result?.Result.Ships.Select(s => s.ToString()).ToList();

            var r = result?.Result;

            if (r == null)
            {
                SelectedShipsMaint = null;
                return;
            }

            var shipsMaint = new List<string>();

            for (var i = 0; i < r.Ships.Count; i++)
            {
                var ship = r.Ships[i];
                var maint = r.ShipsMaint[i].ToString(FmtDuration);

                if (ship.Type == ShipType.None)
                    maint = "";
                else if (r.ShipIsOneTime[i])
                    maint = "(1x)";

                shipsMaint.Add(maint);
            }

            SelectedShipsMaint = shipsMaint;
        }

        private void ResetAssignment()
        {
            EngMod = "0";
            TacMod = "0";
            SciMod = "0";
            EventCrit = "0";
            EventMaintOff = "0";
        }

        private void LoadAssignmentsAndEvents()
        {
            Assignments = AssignmentParser.LoadAssignmentsFromFile(DefaultAssignmentsFilePath);
            Events = AssignmentParser.LoadEventsFromFile(DefaultEventsFilePath);
        }

        private void PopulateAssignmentInfo(Assignment assignment)
        {
            if (assignment == null)
            {
                return;
            }
            
            CritRewardMult = assignment.HasCriticalReward ? "1.5" : "1"; // EC and Dil are 50% more when critical
            EngReq = assignment.ReqEng.ToString();
            TacReq = assignment.ReqTac.ToString();
            SciReq = assignment.ReqSci.ToString();
            Duration = assignment.Duration.ToString(FmtDuration);
            UseOneTimeShips = assignment.HasCriticalReward; // Don't waste one time ships on filler assignments
        }

        private void PopulateEventInfo(Event selectedEvent)
        {
            if (selectedEvent == null)
            {
                return;
            }
            
            EngMod = selectedEvent.ModEng.ToString();
            TacMod = selectedEvent.ModTac.ToString();
            SciMod = selectedEvent.ModSci.ToString();
            EventCrit = selectedEvent.ModCrit.ToString();
            EventMaintOff = selectedEvent.MaintOff.ToString();
        }
    }
}