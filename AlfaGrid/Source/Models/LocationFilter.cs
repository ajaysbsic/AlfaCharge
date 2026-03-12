namespace AlfaGrid.Source.Models
{
    public class LocationFilter
    {
        // Sorting
        public SortBy SortBy { get; set; } = SortBy.Time;
        
        // Rating
        public RatingFilter RatingFilter { get; set; } = RatingFilter.AllRatings;
        public int MinRating { get; set; } = 3;
        
        // Location Filters
        public bool Is24HoursOpen { get; set; }
        public bool IsAvailableNow { get; set; }
        public bool HasFreeParking { get; set; }
        public bool HasWifi { get; set; }
        
        // Connector Type Filters
        public bool HasType2AC { get; set; }
        public bool HasCCS2DC { get; set; }
        public bool HasCHAdeMO { get; set; }
        public bool HasType1AC { get; set; }
        public bool HasGBT { get; set; }

        public LocationFilter Clone()
        {
            return new LocationFilter
            {
                SortBy = this.SortBy,
                RatingFilter = this.RatingFilter,
                MinRating = this.MinRating,
                Is24HoursOpen = this.Is24HoursOpen,
                IsAvailableNow = this.IsAvailableNow,
                HasFreeParking = this.HasFreeParking,
                HasWifi = this.HasWifi,
                HasType2AC = this.HasType2AC,
                HasCCS2DC = this.HasCCS2DC,
                HasCHAdeMO = this.HasCHAdeMO,
                HasType1AC = this.HasType1AC,
                HasGBT = this.HasGBT
            };
        }

        public void Reset()
        {
            SortBy = SortBy.Time;
            RatingFilter = RatingFilter.AllRatings;
            MinRating = 3;
            Is24HoursOpen = false;
            IsAvailableNow = false;
            HasFreeParking = false;
            HasWifi = false;
            HasType2AC = false;
            HasCCS2DC = false;
            HasCHAdeMO = false;
            HasType1AC = false;
            HasGBT = false;
        }

        public bool HasAnyFilter()
        {
            return Is24HoursOpen || IsAvailableNow || HasFreeParking || HasWifi ||
                   HasType2AC || HasCCS2DC || HasCHAdeMO || HasType1AC || HasGBT ||
                   RatingFilter == RatingFilter.OnlyAbove;
        }
    }

    public enum SortBy
    {
        Time,
        Rating
    }

    public enum RatingFilter
    {
        AllRatings,
        OnlyAbove
    }
}
