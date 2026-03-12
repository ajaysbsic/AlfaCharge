using AlfaGrid.Source.Models;

namespace AlfaGrid.Source.Services
{
    public interface IFilterService
    {
        LocationFilter CurrentFilter { get; }
        event EventHandler FiltersChanged;
        void ApplyFilter(LocationFilter filter);
        void ResetFilters();
        List<ChargingLocation> ApplyFilters(List<ChargingLocation> locations);
    }

    public class FilterService : IFilterService
    {
        public LocationFilter CurrentFilter { get; private set; } = new LocationFilter();
        
        public event EventHandler FiltersChanged;

        public void ApplyFilter(LocationFilter filter)
        {
            CurrentFilter = filter.Clone();
            FiltersChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ResetFilters()
        {
            CurrentFilter.Reset();
            FiltersChanged?.Invoke(this, EventArgs.Empty);
        }

        public List<ChargingLocation> ApplyFilters(List<ChargingLocation> locations)
        {
            if (locations == null || !locations.Any())
                return new List<ChargingLocation>();

            var filtered = locations.AsEnumerable();

            // Apply Rating Filter
            if (CurrentFilter.RatingFilter == RatingFilter.OnlyAbove)
            {
                filtered = filtered.Where(l => l.SiteRating >= CurrentFilter.MinRating);
            }

            // Apply 24 Hours Filter
            if (CurrentFilter.Is24HoursOpen)
            {
                filtered = filtered.Where(l => l.OperatingHours != null && 
                    (l.OperatingHours.Contains("24") || l.OperatingHours.Contains("24hrs")));
            }

            // Apply Available Now Filter
            if (CurrentFilter.IsAvailableNow)
            {
                filtered = filtered.Where(l => l.ConnectorGroups != null && 
                    l.ConnectorGroups.Any(c => c.AvailableConnectors > 0));
            }

            // Apply Free Parking Filter
            if (CurrentFilter.HasFreeParking)
            {
                filtered = filtered.Where(l => l.Facilities != null && 
                    l.Facilities.Any(f => f.Name.ToLowerInvariant().Contains("parking")));
            }

            // Apply Wifi Filter
            if (CurrentFilter.HasWifi)
            {
                filtered = filtered.Where(l => l.Facilities != null && 
                    l.Facilities.Any(f => f.Name.ToLowerInvariant().Contains("wifi")));
            }

            // Apply Connector Type Filters
            var connectorFilters = new List<string>();
            if (CurrentFilter.HasType2AC) connectorFilters.Add("Type 2");
            if (CurrentFilter.HasCCS2DC) connectorFilters.Add("CCS2");
            if (CurrentFilter.HasCHAdeMO) connectorFilters.Add("CHAdeMO");
            if (CurrentFilter.HasType1AC) connectorFilters.Add("Type 1");
            if (CurrentFilter.HasGBT) connectorFilters.Add("GB/T");

            if (connectorFilters.Any())
            {
                filtered = filtered.Where(l => l.ConnectorGroups != null && 
                    l.ConnectorGroups.Any(c => connectorFilters.Any(filter => 
                        c.Standard != null && c.Standard.Contains(filter, StringComparison.OrdinalIgnoreCase))));
            }

            // Apply Sorting
            if (CurrentFilter.SortBy == SortBy.Time)
            {
                // Sort by distance (assuming locations have Latitude/Longitude)
                // For now, keep original order
                filtered = filtered.OrderBy(l => l.Name);
            }
            else if (CurrentFilter.SortBy == SortBy.Rating)
            {
                filtered = filtered.OrderByDescending(l => l.SiteRating);
            }

            return filtered.ToList();
        }
    }
}
