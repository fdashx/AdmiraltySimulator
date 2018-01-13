using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AdmiraltySimulator
{
    public class ShipManager
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, Ship> _ships;

        public ShipManager(ILogger logger)
        {
            _logger = logger;
            _ships = new Dictionary<string, Ship>();
        }

        public List<Ship> GetAvailableShips()
        {
            var ships = new List<Ship>();

            foreach (var ship in _ships.Values)
            {
                var sameShipCount = ship.OneTimeUses;

                if (ship.IsOwned
                    && ship.MaintenanceFinish < DateTime.Now)
                    sameShipCount++;

                for (var i = 0; i < Math.Min(sameShipCount, 3); i++)
                    ships.Add(ship);
            }

            return ships;
        }

        public List<Ship> GetAllShips()
        {
            return new List<Ship>(_ships.Values);
        }

        public bool SaveShips(string ownedShipFile, string oneTimeShipFile)
        {
            var owned = new List<string>();
            var oneTime = new List<string>();
            _ships["No Ship"].IsOwned = false;
            _ships["No Ship"].OneTimeUses = 2;

            foreach (var ship in _ships.Values)
            {
                if (ship.IsOwned)
                {
                    var line = ship.Name;

                    if (ship.MaintenanceFinish > DateTime.Now)
                        line += "," + ship.MaintenanceFinish.ToString("yyyyMMddHHmm");

                    owned.Add(line);
                }

                if (ship.OneTimeUses > 0)
                    oneTime.Add(ship.OneTimeUses + "," + ship.Name);
            }

            try
            {
                File.WriteAllLines(ownedShipFile, owned);
                File.WriteAllLines(oneTimeShipFile, oneTime);
                _logger.WriteLine("Owned ships info saved to \"" + ownedShipFile + "\"\n" +
                                  "One time use ships info saved to \"" + oneTimeShipFile + "\"");
                return true;
            }
            catch (Exception e)
            {
                _logger.WriteLine("Failed to save ship info:\n" + e);
                return false;
            }
        }

        public bool LoadShips(string file)
        {
            try
            {
                _ships.Clear();

                foreach (var line in File.ReadAllLines(file))
                    if (TryParseShip(line, out var ship))
                        _ships[ship.Name] = ship;

                if (!_ships.ContainsKey("No Ship"))
                    _ships["No Ship"] = new Ship("No Ship", ShipType.None, 0, 0, 0, new TimeSpan(0), new IAbility[0]);

                _logger.WriteLine("Loaded ship database \"" + file + "\"");
                return true;
            }
            catch (Exception e)
            {
                _logger.WriteLine("Cannot load ship file:\n" + e);
                return false;
            }
        }

        public void LoadOwned(string file)
        {
            try
            {
                foreach (var ship in _ships.Values)
                {
                    ship.IsOwned = false;
                    ship.MaintenanceFinish = DateTime.MinValue;
                }

                foreach (var line in File.ReadAllLines(file))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var vals = line.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

                    if (vals.Length == 0)
                    {
                        _logger.WriteLine("Invalid owned ship info: " + line);
                        continue;
                    }

                    if (!_ships.TryGetValue(vals[0].Trim(), out var owned))
                    {
                        _logger.WriteLine("Ship not found in database: " + vals[0]);
                        continue;
                    }

                    owned.IsOwned = true;

                    if (vals.Length > 1 && DateTime.TryParseExact(vals[1].Trim(), "yyyyMMddHHmm",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out var maintFinTime))
                        owned.MaintenanceFinish = maintFinTime;
                }

                _logger.WriteLine("Loaded owned ships info \"" + file + "\"");
            }
            catch (Exception e)
            {
                _logger.WriteLine("Cannot load owned ships file:\n" + e);
            }
        }

        public void LoadOneTimeUse(string file)
        {
            try
            {
                foreach (var ship in _ships.Values)
                {
                    ship.OneTimeUses = 0;
                }

                foreach (var line in File.ReadAllLines(file))
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    var vals = line.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

                    if (vals.Length != 2 || !TryParseInvariant.Int(vals[0].Trim(), out var count))
                    {
                        _logger.WriteLine("Invalid one time use ship info: " + line);
                        continue;
                    }

                    if (!_ships.TryGetValue(vals[1].Trim(), out var ship))
                    {
                        _logger.WriteLine("Ship not found in database: " + vals[1]);
                        continue;
                    }

                    ship.OneTimeUses = count;
                }

                _logger.WriteLine("Loaded one time use ship info \"" + file + "\"");
            }
            catch (Exception e)
            {
                _logger.WriteLine("Cannot load one time ships file:\n" + e);
            }
        }

        private bool TryParseShip(string shipInfo, out Ship ship)
        {
            ship = null;

            if (string.IsNullOrWhiteSpace(shipInfo) || shipInfo.StartsWith("#"))
            {
                return false;
            }

            var vals = shipInfo.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

            if (vals.Length < 6)
            {
                _logger.WriteLine("Incomplete ship info: " + shipInfo);
                return false;
            }

            var name = vals[0].Trim();
            var timeSpanFmt = new[] {"h'h'm'm'", "h'h'", "m'm'"};

            if (!Enum.TryParse(vals[1].Trim(), true, out ShipType type)
                || !TryParseInvariant.Int(vals[2].Trim(), out var engVal)
                || !TryParseInvariant.Int(vals[3].Trim(), out var tacVal)
                || !TryParseInvariant.Int(vals[4].Trim(), out var sciVal)
                || !TimeSpan.TryParseExact(vals[5].Trim(), timeSpanFmt, CultureInfo.InvariantCulture,
                    out var maintenance))
            {
                _logger.WriteLine("Invalid ship stats: " + shipInfo);
                return false;
            }

            var abilities = new List<IAbility>();

            for (var i = 6; i < vals.Length; i++)
            {
                if (TryParseAbility(vals[i].Trim(), out var ability))
                {
                    abilities.Add(ability);
                }
                else
                {
                    _logger.WriteLine("Invalid ability: " + vals[i]);
                }
            }

            ship = new Ship(name, type, engVal, tacVal, sciVal, maintenance, abilities);
            return true;
        }

        private bool TryParseAbility(string abilityInfo, out IAbility ability)
        {
            var vals = abilityInfo.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            ability = null;
            StatType statType;

            if (vals.Length == 2 && vals[0].ToLowerInvariant().Contains("ignore"))
            {
                if (!Enum.TryParse(vals[1], true, out statType))
                {
                    return false;
                }

                ability = new IgnoreEventMod(statType);
                return true;
            }

            if (vals.Length != 3)
            {
                return false;
            }

            if (!TryParseInvariant.Double(vals[0], out var value))
            {
                return false;
            }

            if (vals[1].ToLowerInvariant() == "maint"
                && Enum.TryParse(vals[2], true, out ShipType shipType))
            {
                ability = new MaintOffPerShip(shipType, (int) value);
                return true;
            }

            if (vals[1].ToLowerInvariant() == "crit")
            {
                if (vals[2].ToLowerInvariant() == "event")
                {
                    ability = new BonusCritEvent(value);
                    return true;
                }

                if (Enum.TryParse(vals[2], true, out statType))
                {
                    ability = new BonusCritStat(statType, value);
                    return true;
                }

                return false;
            }

            if (!Enum.TryParse(vals[1], true, out statType))
            {
                return false;
            }

            if (vals[2].ToLowerInvariant() == "alone")
            {
                ability = new BonusStatAlone(statType, (int) value);
                return true;
            }

            if (Enum.TryParse(vals[2], true, out shipType))
            {
                ability = new BonusStatPerShip(statType, (int) value, shipType);
                return true;
            }

            return false;
        }
    }
}