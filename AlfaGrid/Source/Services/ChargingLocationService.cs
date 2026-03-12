using AlfaGrid.Source.Models;
using AlfaGrid.Source.Helpers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlfaGrid.Source.Services
{
    public class ChargingLocationService : IChargingLocationService
    {
        private List<ChargingLocation>? _cachedLocations;
        private List<ChargingStation>? _cachedStations;

        public async Task<List<ChargingLocation>> GetLocationsAsync()
        {
            if (_cachedLocations != null)
                return _cachedLocations;

            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("locations.json");
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var response = JsonSerializer.Deserialize<LocationsResponse>(json, options);
                _cachedLocations = response?.Data ?? new List<ChargingLocation>();

                // Convert all facility icons to Material Icons
                foreach (var location in _cachedLocations)
                {
                    if (location.Facilities != null && location.Facilities.Any())
                    {
                        System.Diagnostics.Debug.WriteLine($"?? Converting facilities for location: {location.Name}");
                        System.Diagnostics.Debug.WriteLine($"   Before: {location.Facilities.Count} facilities");
                        
                        location.Facilities = location.Facilities
                            .Select(f => {
                                var converted = FacilityIconMapper.ConvertToMaterialIcon(f);
                                System.Diagnostics.Debug.WriteLine($"     {f.Name}: '{f.Icon}' ? '{converted.Icon}' (IsIconFont: {converted.IsIconFont})");
                                return converted;
                            })
                            .ToList();
                        
                        System.Diagnostics.Debug.WriteLine($"   After: {location.Facilities.Count} facilities converted");
                    }
                }

                return _cachedLocations;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading locations: {ex.Message}");
                return new List<ChargingLocation>();
            }
        }

        public async Task<List<ChargingStation>> GetStationsAsync()
        {
            if (_cachedStations != null)
                return _cachedStations;

            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("stations.json");
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();
                
                System.Diagnostics.Debug.WriteLine("=== STATIONS JSON LOADING ===");
                System.Diagnostics.Debug.WriteLine($"JSON Preview: {json.Substring(0, Math.Min(500, json.Length))}...");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var response = JsonSerializer.Deserialize<StationsResponse>(json, options);
                _cachedStations = response?.Data ?? new List<ChargingStation>();
                
                System.Diagnostics.Debug.WriteLine($"? Stations loaded: {_cachedStations.Count}");
                
                foreach (var station in _cachedStations)
                {
                    System.Diagnostics.Debug.WriteLine($"  Station: {station.Name}");
                    System.Diagnostics.Debug.WriteLine($"    Location UID: {station.LocationUid}");
                    System.Diagnostics.Debug.WriteLine($"    Connectors count: {station.Connectors?.Count ?? 0}");
                    
                    if (station.Connectors != null)
                    {
                        foreach (var conn in station.Connectors)
                        {
                            System.Diagnostics.Debug.WriteLine($"      - Connector: PowerType={conn.PowerType?.Value}, Standard={conn.Standard?.Value}, MaxPower={conn.MaxElectricPower}");
                        }
                    }
                }

                return _cachedStations;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Error loading stations: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return new List<ChargingStation>();
            }
        }

        public async Task<ChargingLocation> GetLocationByIdAsync(string locationId)
        {
            var locations = await GetLocationsAsync();
            return locations.FirstOrDefault(l => l.Id == locationId);
        }

        public async Task<List<ChargingLocation>> GetLocationsWithStationsAsync()
        {
            var locations = await GetLocationsAsync();
            var stations = await GetStationsAsync();

            System.Diagnostics.Debug.WriteLine("=== PROCESSING LOCATIONS WITH STATIONS ===");
            System.Diagnostics.Debug.WriteLine($"Locations: {locations.Count}, Stations: {stations.Count}");

            foreach (var location in locations)
            {
                System.Diagnostics.Debug.WriteLine($"\n?? Location: {location.Name} (ID: {location.Id})");
                
                // Find all stations for this location
                location.StationsList = stations
                    .Where(s => !string.IsNullOrEmpty(s.LocationUid) && s.LocationUid == location.Id)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"   Found {location.StationsList.Count} stations for this location");

                // Generate connector groups for UI
                location.ConnectorGroups = GenerateConnectorGroups(location.StationsList);
                
                System.Diagnostics.Debug.WriteLine($"   Generated {location.ConnectorGroups.Count} connector groups:");
                foreach (var group in location.ConnectorGroups)
                {
                    System.Diagnostics.Debug.WriteLine($"     ? {group.ConnectorType} - {group.PowerRating}kW");
                    System.Diagnostics.Debug.WriteLine($"        Standard: {group.Standard}");
                    System.Diagnostics.Debug.WriteLine($"        Image: {group.ImageSource}");
                    System.Diagnostics.Debug.WriteLine($"        Count: {group.AvailableConnectors}/{group.TotalConnectors}");
                }
            }

            return locations;
        }

        private List<ConnectorGroup> GenerateConnectorGroups(List<ChargingStation> stations)
        {
            System.Diagnostics.Debug.WriteLine("\n  === GENERATING CONNECTOR GROUPS ===");
            
            if (stations == null || !stations.Any())
            {
                System.Diagnostics.Debug.WriteLine("  ? No stations provided");
                return new List<ConnectorGroup>();
            }

            var groups = new Dictionary<string, ConnectorGroup>();

            foreach (var station in stations)
            {
                System.Diagnostics.Debug.WriteLine($"  Processing station: {station.Name}");
                
                if (station.Connectors == null || !station.Connectors.Any())
                {
                    System.Diagnostics.Debug.WriteLine($"    ? Station has no connectors");
                    continue;
                }

                System.Diagnostics.Debug.WriteLine($"    Station has {station.Connectors.Count} connectors");

                foreach (var connector in station.Connectors)
                {
                    if (connector.PowerType == null || connector.Standard == null)
                    {
                        System.Diagnostics.Debug.WriteLine($"      ? Connector has null PowerType or Standard");
                        continue;
                    }

                    // Get connector type and power rating
                    var powerType = GetConnectorType(connector.PowerType.Value);
                    var powerRating = connector.MaxElectricPowerNumeric;
                    var standard = connector.Standard.Value;
                    var key = $"{powerType}_{powerRating}";

                    System.Diagnostics.Debug.WriteLine($"      Connector: Type={powerType}, Power={powerRating}kW, Standard={standard}");

                    if (!groups.ContainsKey(key))
                    {
                        var imageSource = GetImageSource(connector.PowerType.Value, standard);
                        System.Diagnostics.Debug.WriteLine($"      ? Creating new group: {key}");
                        System.Diagnostics.Debug.WriteLine($"        Image: {imageSource}");
                        
                        groups[key] = new ConnectorGroup
                        {
                            ConnectorType = powerType,
                            Standard = GetStandardDisplayName(standard),
                            ImageSource = imageSource,
                            PowerRating = powerRating,
                            TotalConnectors = 0,
                            AvailableConnectors = 0
                        };
                    }

                    // Increment total connectors count
                    groups[key].TotalConnectors++;
                    System.Diagnostics.Debug.WriteLine($"      ? Group {key} now has {groups[key].TotalConnectors} connectors");
                }
            }

            var result = groups.Values
                .OrderBy(g => g.ConnectorType == "AC" ? 0 : 1)
                .ThenBy(g => g.PowerRating)
                .ToList();

            System.Diagnostics.Debug.WriteLine($"  ? Total groups created: {result.Count}");
            return result;
        }

        private string GetConnectorType(string powerType)
        {
            if (string.IsNullOrEmpty(powerType))
                return "Unknown";

            return powerType.Contains("AC") ? "AC" : "DC";
        }

        private string GetStandardDisplayName(string standard)
        {
            if (string.IsNullOrEmpty(standard))
                return "Unknown";

            return standard switch
            {
                "IEC_62196_T2" => "Type 2",
                "IEC_62196_T2_COMBO" => "CCS2",
                _ => standard
            };
        }

        private string GetImageSource(string powerType, string standard)
        {
            // For now, use dotnet_bot.png as placeholder for all connectors
            // TODO: Replace with actual connector images: type_2.png and combo_2_ccs.png
            
            System.Diagnostics.Debug.WriteLine($"?? Image requested for: {powerType} + {standard}");
            
            // Return a guaranteed-to-exist image for testing
            string imageName = "dotnet_bot.png";

            //Original image mapping - uncomment when images are added:
            if (string.IsNullOrEmpty(powerType) || string.IsNullOrEmpty(standard))
            {
                System.Diagnostics.Debug.WriteLine($"      ? GetImageSource: Missing powerType or standard");
                return "dotnet_bot.png";
            }

            // AC + Type 2 ? type_2.png
            if (powerType.Contains("AC") && standard == "IEC_62196_T2")
            {
                imageName = "type_2.png";
            }
            // DC + CCS2 ? combo_2_ccs.png
            else if (powerType == "DC" && standard == "IEC_62196_T2_COMBO")
            {
                imageName = "combo_2_ccs.png";
            }
            else
            {
                imageName = "dotnet_bot.png";
            }


            System.Diagnostics.Debug.WriteLine($"?? Using fallback image: {imageName}");
            return imageName;
        }

        // Response wrapper classes for JSON deserialization
        private class LocationsResponse
        {
            public List<ChargingLocation>? Data { get; set; }
            public int Count { get; set; }
            public int Offset { get; set; }
            public int Limit { get; set; }
        }

        private class StationsResponse
        {
            public List<ChargingStation>? Data { get; set; }
            public int Count { get; set; }
            public int Offset { get; set; }
            public int Limit { get; set; }
        }
    }
}
