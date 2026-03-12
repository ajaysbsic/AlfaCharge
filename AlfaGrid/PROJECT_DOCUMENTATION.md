# ?? AlfaGrid - Project Documentation

> **EV Charging Station Locator Application**  
> Built with .NET MAUI 10.0 | Cross-Platform Mobile App

---

## ?? Table of Contents

1. [Project Overview](#-project-overview)
2. [Features](#-features)
3. [Technology Stack](#-technology-stack)
4. [Project Structure](#-project-structure)
5. [Architecture & Design Patterns](#-architecture--design-patterns)
6. [Data Models](#-data-models)
7. [Services](#-services)
8. [Navigation](#-navigation)
9. [Styling & Theming](#-styling--theming)
10. [Configuration & Setup](#-configuration--setup)
11. [Testing](#-testing)
12. [Deployment](#-deployment)
13. [Known Issues & Solutions](#-known-issues--solutions)
14. [Future Enhancements](#-future-enhancements)

---

## ?? Project Overview

### About AlfaGrid

AlfaGrid is a cross-platform mobile application that helps electric vehicle (EV) owners find, reserve, and use charging stations across Saudi Arabia. The app provides an intuitive map-based interface with real-time availability, advanced filtering, and multi-language support.

### Target Platforms

| Platform | Status | Min Version |
|----------|--------|-------------|
| ? **Android** | Production Ready | API 21+ (Android 5.0) |
| ? **Windows** | Production Ready | Windows 10 19041+ |
| ?? **iOS** | Configured | iOS 15+ |
| ?? **macOS** | Configured | macOS 15+ |

### Key Statistics

- **Languages Supported:** 2 (English, Arabic with RTL)
- **Charging Locations:** 5 (Sample Data)
- **Total Charging Stations:** 23
- **Connector Types:** 5 (Type 2 AC, CCS2 DC, CHAdeMO, Type 1, GB/T)
- **Pages:** 15+
- **ViewModels:** 12+

---

## ? Features

### ??? Interactive Map
- **Google Maps Integration**
- Current location detection
- Custom map pins (charging stations, current location)
- Pin clustering for dense areas
- Info windows on pin click
- Directions to charging stations
- Real-time location tracking

### ?? Charging Station Information
- **Real-time availability status**
- Connector types and power ratings (kW)
- Operating hours (24/7 or scheduled)
- Site ratings and reviews (placeholder)
- Facility information (WiFi, parking, restaurants)
- Contact information
- Pricing/tariff details

### ?? Advanced Filtering
**Sorting Options:**
- By Distance/Time
- By Rating

**Rating Filter:**
- All Ratings
- Minimum Rating (1-5 stars)

**Location Filters:**
- 24 Hours Open
- Available Now
- Free Parking
- WiFi Available

**Connector Type Filters:**
- Type 2 (AC)
- CCS2 (DC)
- CHAdeMO
- Type 1 (AC)
- GB/T

**Additional Features:**
- Filter persistence
- Reset functionality
- Apply/Cancel options

### ?? Multi-Language Support
- **English (en)** - Left-to-Right
- **Arabic (ar)** - Right-to-Left with full RTL support
- Language selector on Login & Settings
- Instant language switching (no restart)
- Persistent language preference
- 250+ translated strings
- FlowDirection automatic adjustment

### ?? User Features
**Authentication:**
- Email/Password login
- User registration
- Session management
- Logout functionality

**Profile Management:**
- Profile editing
- Charging history
- Saved payment methods
- Favorites

**Reservations:**
- Time slot booking (placeholder)
- Payment integration (placeholder)
- Booking confirmation

**QR Code Scanner:**
- Scan QR at charging stations
- Navigate to specific EVSE/connector
- Start charging session

### ?? UI/UX Features
- Material Design icons
- Custom loading overlays
- Pull-to-refresh
- Smooth animations
- Responsive layouts
- Error handling with user-friendly messages

---

## ??? Technology Stack

### Core Framework
```
.NET 10.0
.NET MAUI (Multi-platform App UI)
C# 14.0
```

### Key Libraries & Packages

| Package | Version | Purpose |
|---------|---------|---------|
| `CommunityToolkit.Mvvm` | 8.4.0 | MVVM Pattern, Messaging |
| `Microsoft.Maui.Controls` | 10.0.11 | UI Framework |
| `Microsoft.Maui.Controls.Maps` | 10.0.11 | Google Maps Integration |
| `Microsoft.Maui.Essentials` | 10.0.11 | Device Features |
| `Refit` | 9.0.2 | REST API Client |
| `sqlite-net-pcl` | 1.9.172 | Local Database |
| `Microsoft.Extensions.Logging.Debug` | 10.0.0 | Debugging |

### Development Tools
- **Visual Studio 2022** (17.8+)
- **.NET MAUI Workload**
- **Android SDK** (API 34+)
- **Git** for version control

---

## ?? Project Structure

```
AlfaGrid/
??? Source/
?   ??? Controls/                    # Custom Reusable Controls
?   ?   ??? ChargingLocationCard.xaml
?   ?   ??? ChargingLocationCard.xaml.cs
?   ?   ??? LoadingOverlay.xaml
?   ?   ??? LoadingOverlay.xaml.cs
?   ?
?   ??? Converters/                  # Value Converters & Markup Extensions
?   ?   ??? BoolConverters.cs        # Boolean to color/visibility converters
?   ?   ??? FilterConverters.cs      # Filter-specific converters
?   ?   ??? TranslateExtension.cs    # Localization markup extension
?   ?   ??? InvertedBoolConverter.cs
?   ?
?   ??? Handler/                     # Custom Handlers
?   ?   ??? BorderlessEntry.cs       # Entry without border
?   ?
?   ??? Helpers/                     # Helper Classes
?   ?   ??? ServiceHelper.cs         # DI service provider access
?   ?   ??? FacilityIconMapper.cs    # Map facility names to icons
?   ?   ??? MaterialIcons.cs         # Material icon constants
?   ?
?   ??? Messages/                    # Messenger Pattern Messages
?   ?   ??? LanguageChangedMessage.cs
?   ?
?   ??? Models/                      # Data Models
?   ?   ??? ChargingLocation.cs      # Location with connectors
?   ?   ??? ConnectorGroup.cs        # Grouped connectors by type
?   ?   ??? Facility.cs              # Location facilities
?   ?   ??? LocationFilter.cs        # Filter criteria
?   ?   ??? UserBasicInfo.cs         # User data
?   ?   ??? RoleDetails.cs           # User roles enum
?   ?   ??? CarouselItem.cs          # Carousel data (obsolete)
?   ?
?   ??? Services/                    # Business Logic Services
?   ?   ??? AlertService.cs          # User notifications
?   ?   ??? ChargingLocationService.cs  # Location data management
?   ?   ??? FilterService.cs         # Filter state management
?   ?   ??? LocalizationService.cs   # Multi-language support
?   ?
?   ??? View/                        # UI Pages (XAML)
?   ?   ??? HomePage.xaml            # Main map view
?   ?   ??? FilterPage.xaml          # Filtering interface
?   ?   ??? LocationDetailsPage.xaml # Location details
?   ?   ??? LocationListPage.xaml    # List view
?   ?   ??? LoginPage.xaml           # Authentication
?   ?   ??? RegisterPage.xaml        # User registration
?   ?   ??? SettingsPage.xaml        # App settings
?   ?   ??? ProfilePage.xaml         # User profile
?   ?   ??? QRScannerPage.xaml       # QR code scanner
?   ?   ??? AddCardDetailsPage.xaml  # Payment method
?   ?   ??? MyChargingProfilePage.xaml
?   ?   ??? ReservationsPage.xaml
?   ?   ??? LoadingPage.xaml
?   ?
?   ??? ViewModel/                   # ViewModels (MVVM)
?       ??? BaseViewModel.cs         # Base class for all VMs
?       ??? HomePageViewModel.cs
?       ??? FilterPageViewModel.cs
?       ??? LocationDetailsPageViewModel.cs
?       ??? LoginPageViewModel.cs
?       ??? SettingsPageViewModel.cs
?       ??? AppShellViewModel.cs
?       ??? ... (other ViewModels)
?
??? Resources/
?   ??? Fonts/                       # Custom Fonts
?   ?   ??? OpenSans-Regular.ttf
?   ?   ??? OpenSans-Semibold.ttf
?   ?   ??? MaterialIcons-Regular.ttf
?   ?
?   ??? Images/                      # App Images & Icons
?   ?   ??? charging_station.png
?   ?   ??? current_location_icon.png
?   ?   ??? arrow_left.png
?   ?   ??? ... (100+ icons)
?   ?
?   ??? Localization/                # Multi-Language Support
?   ?   ??? AppResources.cs          # Translation dictionary (250+ strings)
?   ?
?   ??? Raw/                         # JSON Data Files
?   ?   ??? charging_locations.json  # Sample location data
?   ?   ??? charging_stations.json   # Sample station data
?   ?
?   ??? Styles/                      # XAML Styles & Themes
?       ??? Colors.xaml              # Color palette
?       ??? Styles.xaml              # Global styles
?
??? Platforms/                       # Platform-Specific Code
?   ??? Android/
?   ?   ??? MainActivity.cs
?   ?   ??? MainApplication.cs
?   ?   ??? Resources/
?   ??? iOS/
?   ?   ??? AppDelegate.cs
?   ?   ??? Info.plist
?   ??? Windows/
?       ??? App.xaml
?       ??? app.manifest
?
??? App.xaml                         # Application Resources
??? App.xaml.cs                      # App Initialization
??? AppShell.xaml                    # Navigation Shell
??? AppShell.xaml.cs                 # Shell Logic
??? MauiProgram.cs                   # App Startup & DI Configuration
```

---

## ??? Architecture & Design Patterns

### 1. MVVM (Model-View-ViewModel) Pattern

```
????????????????????????????????????????????????
?              MVVM ARCHITECTURE                ?
????????????????????????????????????????????????
?                                               ?
?  ??????????      ???????????????      ???????
?  ?  View  ????????  ViewModel  ????????Model??
?  ? (XAML) ?      ?(Logic/State)?      ?Data ??
?  ??????????      ???????????????      ???????
?      ?                  ?                 ?   ?
?   Display          Commands           Business?
?   Binding          Properties          Logic  ?
?                                               ?
????????????????????????????????????????????????
```

**Benefits:**
- ? Separation of concerns
- ? Testable business logic
- ? Reusable ViewModels
- ? Data binding reduces boilerplate

**Implementation:**
```csharp
// BaseViewModel provides common functionality
public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;
    
    [ObservableProperty]
    private string title = string.Empty;
}

// Concrete ViewModels inherit
public partial class HomePageViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<ChargingLocation> chargingLocations = new();
    
    [RelayCommand]
    private async Task LoadLocationsAsync()
    {
        // Business logic
    }
}
```

### 2. Dependency Injection

**Registration** (`MauiProgram.cs`):
```csharp
builder.Services.AddSingleton<IChargingLocationService, ChargingLocationService>();
builder.Services.AddSingleton<IFilterService, FilterService>();
builder.Services.AddSingleton<ILocalizationService, LocalizationService>();

// Pages & ViewModels
builder.Services.AddTransient<HomePage>();
builder.Services.AddTransient<HomePageViewModel>();
```

**Consumption** (Constructor Injection):
```csharp
public class HomePageViewModel : BaseViewModel
{
    private readonly IChargingLocationService _locationService;
    private readonly IFilterService _filterService;

    public HomePageViewModel(
        IChargingLocationService locationService,
        IFilterService filterService)
    {
        _locationService = locationService;
        _filterService = filterService;
    }
}
```

### 3. Messenger Pattern (Loose Coupling)

**Use Case:** Notify all components when language changes

```csharp
// Send message
WeakReferenceMessenger.Default.Send(new LanguageChangedMessage("ar"));

// Receive message
public class AppShellViewModel : IRecipient<LanguageChangedMessage>
{
    public AppShellViewModel()
    {
        WeakReferenceMessenger.Default.Register<LanguageChangedMessage>(this);
    }

    public void Receive(LanguageChangedMessage message)
    {
        // Update UI
    }
}
```

### 4. Service Layer Pattern

**Interface:**
```csharp
public interface IChargingLocationService
{
    Task<List<ChargingLocation>> GetLocationsAsync();
    Task<List<Station>> GetStationsAsync();
    Task<List<ChargingLocation>> GetLocationsWithStationsAsync();
}
```

**Implementation:**
```csharp
public class ChargingLocationService : IChargingLocationService
{
    public async Task<List<ChargingLocation>> GetLocationsAsync()
    {
        // Load from JSON, API, or database
    }
}
```

### 5. Repository Pattern (Data Access)

Current: JSON files  
Future: REST API with local caching

```csharp
// Current
var json = await FileSystem.OpenAppPackageFileAsync("charging_locations.json");
var data = await JsonSerializer.DeserializeAsync<LocationsResponse>(json);

// Future (with Refit)
var data = await _apiClient.GetLocationsAsync();
```

---

## ?? Data Models

### ChargingLocation

```csharp
public class ChargingLocation : ObservableObject
{
    public string Id { get; set; }
    public string LocationUID { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string OperatingHours { get; set; }
    public double SiteRating { get; set; }
    public string ContactPerson { get; set; }
    public string ContactEmail { get; set; }
    
    public List<ConnectorGroup> ConnectorGroups { get; set; }
    public List<Facility> Facilities { get; set; }
    
    // Computed Properties
    public string AvailabilityStatus { get; }
    public Location Location { get; }
}
```

### ConnectorGroup

```csharp
public class ConnectorGroup : ObservableObject
{
    public string Standard { get; set; }          // "Type 2", "CCS2"
    public string PowerType { get; set; }         // "AC_3_PHASE", "DC"
    public double PowerRating { get; set; }       // kW
    public int TotalConnectors { get; set; }
    public int AvailableConnectors { get; set; }
    public string ImageSource { get; set; }
    
    // Computed Properties
    public string PowerRatingText { get; }        // "22kW"
    public string AvailabilityText { get; }       // "2/5 Available"
}
```

### LocationFilter

```csharp
public class LocationFilter
{
    // Sorting
    public SortBy SortBy { get; set; }            // Time, Rating
    
    // Rating
    public RatingFilter RatingFilter { get; set; } // AllRatings, OnlyAbove
    public int MinRating { get; set; }            // 1-5
    
    // Location Features
    public bool Is24HoursOpen { get; set; }
    public bool IsAvailableNow { get; set; }
    public bool HasFreeParking { get; set; }
    public bool HasWifi { get; set; }
    
    // Connector Types
    public bool HasType2AC { get; set; }
    public bool HasCCS2DC { get; set; }
    public bool HasCHAdeMO { get; set; }
    public bool HasType1AC { get; set; }
    public bool HasGBT { get; set; }
}
```

---

## ?? Services

### IChargingLocationService
**Purpose:** Manage charging location and station data

**Methods:**
- `GetLocationsAsync()` - Load all locations
- `GetStationsAsync()` - Load all stations  
- `GetLocationsWithStationsAsync()` - Combined data

### IFilterService
**Purpose:** Manage filter state and application

**Methods:**
- `GetCurrentFilter()` - Get active filters
- `UpdateFilter(LocationFilter filter)` - Update filters
- `ApplyFilters(List<ChargingLocation> locations)` - Filter locations
- `ResetFilters()` - Clear all filters

**Events:**
- `FiltersChanged` - Triggered when filters update

### ILocalizationService
**Purpose:** Multi-language support

**Properties:**
- `CurrentLanguage` - Get/Set language ("en", "ar")
- `FlowDirection` - Get RTL/LTR direction
- `this[string key]` - Indexer for translations

**Methods:**
- `GetString(string key)` - Get translated string
- `GetFormattedString(string key, params object[] args)` - Formatted string

**Events:**
- `LanguageChanged` - Triggered when language changes

---

## ?? Navigation

### Shell Navigation Structure

```
AppShell
??? LoginPage (No Flyout)
??? RegisterPage (No Flyout)
??? HomePage (Main - Flyout Enabled)
    ??? Flyout Menu
    ?   ??? My Charging Profile
    ?   ??? Reservations
    ?   ??? Settings
    ??? Modal Routes
        ??? LocationListPage
        ??? LocationDetailsPage
        ??? FilterPage
        ??? QRScannerPage
        ??? ProfilePage
        ??? AddCardDetailsPage
```

### Route Registration

```csharp
// AppShell.xaml.cs
Routing.RegisterRoute(nameof(LocationDetailsPage), typeof(LocationDetailsPage));
Routing.RegisterRoute(nameof(FilterPage), typeof(FilterPage));
// ... other routes
```

### Navigation Commands

```csharp
// Navigate to page
await Shell.Current.GoToAsync(nameof(FilterPage));

// Navigate with parameters
var parameters = new Dictionary<string, object>
{
    { "location", selectedLocation }
};
await Shell.Current.GoToAsync(nameof(LocationDetailsPage), parameters);

// Go back
await Shell.Current.GoToAsync("..");

// Go to specific route
await Shell.Current.GoToAsync("//home");
```

---

## ?? Styling & Theming

### Color Palette (White-Label Ready)

```xml
<!-- Resources/Styles/Colors.xaml -->

<!-- Primary Brand -->
<Color x:Key="BrandPrimary">#F79A1B</Color>
<Color x:Key="BrandSecondary">#0066FF</Color>

<!-- Text Colors -->
<Color x:Key="TextPrimary">#0E2A47</Color>
<Color x:Key="TextSecondary">#6D7A8A</Color>
<Color x:Key="TextTertiary">#999999</Color>

<!-- Background -->
<Color x:Key="BackgroundPrimary">#FFFFFF</Color>
<Color x:Key="BackgroundSecondary">#F5F5F5</Color>

<!-- Status -->
<Color x:Key="SuccessColor">#4CAF50</Color>
<Color x:Key="WarningColor">#FF9800</Color>
<Color x:Key="ErrorColor">#F44336</Color>
```

### Typography

```xml
<Style TargetType="Label" x:Key="Headline1">
    <Setter Property="FontFamily" Value="OpenSansSemibold" />
    <Setter Property="FontSize" Value="24" />
    <Setter Property="TextColor" Value="{StaticResource TextPrimary}" />
</Style>

<Style TargetType="Label" x:Key="BodyText">
    <Setter Property="FontFamily" Value="OpenSansRegular" />
    <Setter Property="FontSize" Value="14" />
    <Setter Property="TextColor" Value="{StaticResource TextSecondary}" />
</Style>
```

### Custom Styles

- **Buttons:** Primary (orange), Secondary (blue), Outlined
- **Cards:** Rounded borders with shadow
- **Input Fields:** Borderless with bottom line
- **Loading:** Custom overlay with spinner

---

## ?? Configuration & Setup

### Prerequisites

1. **Visual Studio 2022** (v17.8 or later)
2. **.NET 10 SDK**
3. **.NET MAUI Workload**
   ```bash
   dotnet workload install maui
   ```
4. **Android SDK** (for Android development)

### Clone & Setup

```bash
# Clone repository
git clone <repository-url>
cd AlfaGrid

# Restore packages
dotnet restore

# Build
dotnet build
```

### Run on Android

```bash
# Using CLI
dotnet build -t:Run -f net10.0-android

# Or in Visual Studio
# 1. Set AlfaGrid (net10.0-android) as startup project
# 2. Select Android emulator or device
# 3. Press F5
```

### Run on Windows

```bash
# Using CLI
dotnet build -t:Run -f net10.0-windows10.0.19041.0

# Or in Visual Studio
# 1. Set AlfaGrid (net10.0-windows) as startup project
# 2. Press F5
```

### Google Maps API Key

1. Get API key from [Google Cloud Console](https://console.cloud.google.com/)
2. Enable Maps SDK for Android
3. Add to `Platforms/Android/AndroidManifest.xml`:

```xml
<meta-data 
    android:name="com.google.android.geo.API_KEY" 
    android:value="YOUR_API_KEY_HERE"/>
```

---

## ?? Testing

### Test Data

**Location:** `Resources/Raw/charging_locations.json`

- 5 Charging Locations
- 23 Total Stations
- Real GPS coordinates (Riyadh, Saudi Arabia)

### Test Scenarios

**1. Map Functionality**
- ? Current location detection
- ? Pin placement
- ? Pin click/info window
- ? Carousel sync with map

**2. Filtering**
- ? Apply single filter
- ? Apply multiple filters
- ? Reset filters
- ? Filter persistence

**3. Localization**
- ? Switch to Arabic
- ? Verify RTL layout
- ? Check all translations
- ? Switch back to English

**4. Navigation**
- ? Page transitions
- ? Back button behavior
- ? Flyout menu
- ? Deep linking

---

## ?? Deployment

### Android Release Build

```bash
# Clean
dotnet clean

# Publish
dotnet publish -f net10.0-android -c Release

# Output: bin/Release/net10.0-android/publish/
```

**Sign APK:**
```bash
# Create keystore (first time only)
keytool -genkey -v -keystore alfagrid.keystore -alias alfagrid -keyalg RSA -keysize 2048 -validity 10000

# Sign
jarsigner -verbose -sigalg SHA1withRSA -digestalg SHA1 -keystore alfagrid.keystore app-release-unsigned.apk alfagrid

# Verify
jarsigner -verify -verbose -certs app-release-unsigned.apk
```

### Windows Release Build

```bash
dotnet publish -f net10.0-windows10.0.19041.0 -c Release
```

---

## ?? Known Issues & Solutions

### Issue 1: Map Overlay After Navigation

**Symptom:** Map unresponsive with gray overlay

**Solution:**
```csharp
protected override void OnAppearing()
{
    _viewModel.IsBusy = false;
    TapOverlay.IsVisible = false;
}
```

### Issue 2: Material Icons Not Showing

**Symptom:** Icons appear as boxes

**Solution:**
- Verify `MaterialIcons-Regular.ttf` in `Resources/Fonts/`
- Check font registration in `MauiProgram.cs`
- Use exact name: `FontFamily="MaterialIcons"`

### Issue 3: Filter Reset Not Working

**Symptom:** Filters don't clear

**Solution:** Use uppercase property names (MVVM Toolkit requirement)
```csharp
IsSortByTime = true;  // ? Correct
isSortByTime = true;  // ? Wrong
```

---

## ?? Future Enhancements

### Planned Features

1. **Real-time Updates**
   - WebSocket connection
   - Live availability status
   - Push notifications

2. **Reservation System**
   - Time slot booking
   - Payment integration (Stripe/PayPal)
   - Booking management

3. **Charging Session**
   - Start/stop remotely
   - Real-time monitoring
   - Session history

4. **Social Features**
   - User reviews
   - Photo uploads
   - Share locations

5. **Advanced Routing**
   - Multi-stop planning
   - Battery range calculation
   - Optimal charging stops

6. **Accessibility**
   - Screen reader support
   - High contrast mode
   - Font size adjustment

---

## ?? Additional Documentation

- **[LOCALIZATION_GUIDE.md](LOCALIZATION_GUIDE.md)** - Complete localization tutorial
- **[PERFORMANCE_GUIDE.md](PERFORMANCE_GUIDE.md)** - Performance optimization patterns
- **[CUSTOM_PIN_ICONS_GUIDE.md](CUSTOM_PIN_ICONS_GUIDE.md)** - Custom map pins implementation

---

## ?? Support & Contact

- **Email:** support@alfagrid.com
- **Website:** https://alfagrid.com
- **Documentation:** This file and related guides

---

**Last Updated:** December 2024  
**Version:** 1.0.0  
**Build:** net10.0  
**License:** Proprietary - All Rights Reserved

---

**End of Project Documentation**
