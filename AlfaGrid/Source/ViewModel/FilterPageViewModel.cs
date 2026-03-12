using AlfaGrid.Source.Models;
using AlfaGrid.Source.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlfaGrid.Source.ViewModel
{
    public partial class FilterPageViewModel : BaseViewModel
    {
        private readonly IFilterService _filterService;
        private LocationFilter _workingFilter;

        [ObservableProperty]
        private bool isSortByTime;

        [ObservableProperty]
        private bool isSortByRating;

        [ObservableProperty]
        private bool isAllRatings;

        [ObservableProperty]
        private bool isOnlyAbove;

        [ObservableProperty]
        private int minRating = 3;

        [ObservableProperty]
        private bool is24HoursOpen;

        [ObservableProperty]
        private bool isAvailableNow;

        [ObservableProperty]
        private bool hasFreeParking;

        [ObservableProperty]
        private bool hasWifi;

        [ObservableProperty]
        private bool hasType2AC;

        [ObservableProperty]
        private bool hasCCS2DC;

        [ObservableProperty]
        private bool hasCHAdeMO;

        [ObservableProperty]
        private bool hasType1AC;

        [ObservableProperty]
        private bool hasGBT;

        public FilterPageViewModel(IFilterService filterService)
        {
            _filterService = filterService;
            _workingFilter = filterService.CurrentFilter.Clone();
            LoadCurrentFilters();
        }

        private void LoadCurrentFilters()
        {
            // Load sorting
            IsSortByTime = _workingFilter.SortBy == SortBy.Time;
            IsSortByRating = _workingFilter.SortBy == SortBy.Rating;

            // Load rating filter
            IsAllRatings = _workingFilter.RatingFilter == RatingFilter.AllRatings;
            IsOnlyAbove = _workingFilter.RatingFilter == RatingFilter.OnlyAbove;
            MinRating = _workingFilter.MinRating;

            // Load location filters
            Is24HoursOpen = _workingFilter.Is24HoursOpen;
            IsAvailableNow = _workingFilter.IsAvailableNow;
            HasFreeParking = _workingFilter.HasFreeParking;
            HasWifi = _workingFilter.HasWifi;

            // Load connector type filters
            HasType2AC = _workingFilter.HasType2AC;
            HasCCS2DC = _workingFilter.HasCCS2DC;
            HasCHAdeMO = _workingFilter.HasCHAdeMO;
            HasType1AC = _workingFilter.HasType1AC;
            HasGBT = _workingFilter.HasGBT;
        }

        [RelayCommand]
        private void SelectSortByTime()
        {
            IsSortByTime = true;
            IsSortByRating = false;
            _workingFilter.SortBy = SortBy.Time;
        }

        [RelayCommand]
        private void SelectSortByRating()
        {
            IsSortByTime = false;
            IsSortByRating = true;
            _workingFilter.SortBy = SortBy.Rating;
        }

        [RelayCommand]
        private void SelectAllRatings()
        {
            IsAllRatings = true;
            IsOnlyAbove = false;
            _workingFilter.RatingFilter = RatingFilter.AllRatings;
        }

        [RelayCommand]
        private void SelectOnlyAbove()
        {
            IsAllRatings = false;
            IsOnlyAbove = true;
            _workingFilter.RatingFilter = RatingFilter.OnlyAbove;
        }

        [RelayCommand]
        private void SelectRating(object parameter)
        {
            if (parameter is string ratingStr && int.TryParse(ratingStr, out int rating))
            {
                MinRating = rating;
                _workingFilter.MinRating = rating;
            }
        }

        [RelayCommand]
        private async Task Reset()
        {
            _workingFilter.Reset();
            LoadCurrentFilters();
            
            // Also apply the reset to the service immediately
            _filterService.ApplyFilter(_workingFilter);
        }

        [RelayCommand]
        private async Task Apply()
        {
            try
            {
                IsBusy = true;
                
                // Sync all current UI values to working filter
                _workingFilter.Is24HoursOpen = Is24HoursOpen;
                _workingFilter.IsAvailableNow = IsAvailableNow;
                _workingFilter.HasFreeParking = HasFreeParking;
                _workingFilter.HasWifi = HasWifi;
                _workingFilter.HasType2AC = HasType2AC;
                _workingFilter.HasCCS2DC = HasCCS2DC;
                _workingFilter.HasCHAdeMO = HasCHAdeMO;
                _workingFilter.HasType1AC = HasType1AC;
                _workingFilter.HasGBT = HasGBT;
                
                // Apply the working filter
                _filterService.ApplyFilter(_workingFilter);
                
                // Navigate back
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying filters: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error", "Unable to apply filters. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
