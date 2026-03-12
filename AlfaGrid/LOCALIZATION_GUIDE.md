# ?? Complete Localization Guide for .NET MAUI

> **From Zero to Hero - Master Multi-Language Apps**

---

## ?? Table of Contents

1. [What is Localization?](#-what-is-localization)
2. [Why Localization Matters](#-why-localization-matters)
3. [Microsoft's Official Approach](#-microsofts-official-approach)
4. [Our Custom Implementation](#-our-custom-implementation)
5. [Step-by-Step Tutorial](#-step-by-step-tutorial)
6. [Advanced Patterns](#-advanced-patterns)
7. [Usage Examples](#-usage-examples)
8. [Testing & Debugging](#-testing--debugging)
9. [Best Practices](#-best-practices)
10. [Troubleshooting](#-troubleshooting)

---

## ?? What is Localization?

### Definition

**Localization (L10n)** is the process of adapting an application to support multiple languages and regions.

### Key Components

```
??????????????????????????????????????????
?         LOCALIZATION                    ?
??????????????????????????????????????????
?                                         ?
?  ????????????  ???????????  ???????????
?  ?Translation  ?   RTL    ?  ?Formatting??
?  ?  Strings ?  ? Support ?  ? Culture  ??
?  ????????????  ???????????  ???????????
?       ?              ?           ?      ?
?  ????????????????????????????????????  ?
?  ? User sees content in their       ?  ?
?  ? preferred language              ?  ?
?  ????????????????????????????????????  ?
??????????????????????????????????????????
```

### What It Includes

1. **Translation** - Text in multiple languages
2. **RTL Support** - Right-to-Left layouts (Arabic, Hebrew)
3. **Culture Formatting** - Dates, numbers, currencies
4. **UI Mirroring** - Layout adaptation for RTL

---

## ?? Why Localization Matters

### Statistics

- **75%** of users prefer apps in their native language
- **56%** say language is more important than price
- **90%** increase in engagement with localized content
- **40%** won't buy from sites in other languages

### Benefits

| Benefit | Impact |
|---------|--------|
| ?? **Wider Market Reach** | Access to 7+ billion people |
| ?? **Better UX** | Users feel comfortable |
| ?? **Increased Revenue** | More downloads & purchases |
| ?? **Competitive Advantage** | Stand out globally |

---

## ?? Microsoft's Official Approach

### Using .resx Files

**Microsoft recommends** using **.resx files** for localization.

#### File Structure

```
YourApp/
??? Resources/
?   ??? Strings/
?   ?   ??? AppResources.resx        (English - Default)
?   ?   ??? AppResources.ar.resx     (Arabic)
?   ?   ??? AppResources.es.resx     (Spanish)
?   ?   ??? AppResources.fr.resx     (French)
```

#### Creating .resx File

**AppResources.resx:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <data name="AppName" xml:space="preserve">
    <value>My App</value>
  </data>
  <data name="Welcome" xml:space="preserve">
    <value>Welcome</value>
  </data>
  <data name="Goodbye" xml:space="preserve">
    <value>Goodbye</value>
  </data>
</root>
```

**AppResources.ar.resx:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <data name="AppName" xml:space="preserve">
    <value>??????</value>
  </data>
  <data name="Welcome" xml:space="preserve">
    <value>??????</value>
  </data>
  <data name="Goodbye" xml:space="preserve">
    <value>??????</value>
  </data>
</root>
```

#### Usage in Code

```csharp
using Resources.Strings;

// C#
string welcome = AppResources.Welcome; // Automatically localized
```

#### Usage in XAML

```xaml
<Label Text="{x:Static resources:AppResources.Welcome}" />
```

### Pros & Cons

| Feature | Microsoft .resx | Our Custom Approach |
|---------|----------------|---------------------|
| **Type Safety** | ? Compile-time | ? Runtime strings |
| **IDE Support** | ? IntelliSense | ? Manual |
| **File Format** | XML (.resx) | C# Dictionary |
| **Flexibility** | ? Rigid | ? Highly flexible |
| **Runtime Change** | ? Requires restart | ? Instant update |
| **XAML Syntax** | Complex | ? Simple |
| **Hot Reload** | ? Limited | ? Full support |
| **Dynamic Loading** | ? No | ? Yes |
| **Testing** | Complex | ? Easy to mock |

### When to Use Each

**Use Microsoft .resx when:**
- ? Large team with translators
- ? Need compile-time safety
- ? Professional translation workflow
- ? IntelliSense is critical

**Use Custom Approach when:**
- ? Need instant language switching
- ? Small to medium app
- ? Rapid development
- ? Frequent UI iterations

---

## ??? Our Custom Implementation

### Why We Chose It

1. **Instant Language Switching** - No app restart
2. **Simplified XAML** - Clean `{local:Translate Key}` syntax
3. **Messenger Pattern** - Instant UI updates everywhere
4. **Easier Testing** - Mock translations easily
5. **Centralized** - Single source of truth

### Architecture Overview

```
?????????????????????????????????????????????
?          APPLICATION FLOW                  ?
?????????????????????????????????????????????
?                                            ?
?  ???????????????????????                  ?
?  ? XAML Pages          ?                  ?
?  ? {local:Translate}   ?                  ?
?  ???????????????????????                  ?
?             ?                              ?
?  ???????????????????????                  ?
?  ? TranslateExtension  ? (Markup)         ?
?  ???????????????????????                  ?
?             ?                              ?
?  ???????????????????????????????????      ?
?  ? LocalizationService             ?      ?
?  ? - CurrentLanguage               ?      ?
?  ? - GetString()                   ?      ?
?  ? - Send LanguageChangedMessage   ?      ?
?  ???????????????????????????????????      ?
?             ?                              ?
?  ???????????????????????                  ?
?  ? AppResources        ? (Dictionary)     ?
?  ? - Translations      ?                  ?
?  ? - FlowDirection     ?                  ?
?  ???????????????????????                  ?
?             ?                              ?
?  ???????????????????????????????????      ?
?  ? All ViewModels & Pages          ?      ?
?  ? Subscribe to LanguageChanged    ?      ?
?  ? ? Instant UI Updates!           ?      ?
?  ???????????????????????????????????      ?
?????????????????????????????????????????????
```

### Components

1. **AppResources.cs** - Translation dictionary
2. **LocalizationService.cs** - Language manager
3. **TranslateExtension.cs** - XAML helper
4. **LanguageChangedMessage.cs** - Messenger notification
5. **ViewModels** - Subscribe to changes

---

## ?? Step-by-Step Tutorial

### Step 1: Create AppResources.cs

**Location:** `Resources/Localization/AppResources.cs`

```csharp
using System.Globalization;
using Microsoft.Maui.Controls;

namespace AlfaGrid.Resources.Localization
{
    public class AppResources
    {
        // Translation dictionary: Language ? Key ? Value
        private static readonly Dictionary<string, Dictionary<string, string>> Translations = new()
        {
            ["en"] = new Dictionary<string, string>
            {
                ["AppName"] = "AlfaGrid",
                ["Welcome"] = "Welcome",
                ["Goodbye"] = "Goodbye",
                ["HelloUser"] = "Hello, {0}!",
                ["Login_Title"] = "Welcome Back",
                ["Login_Button"] = "Sign In",
                // ... more strings
            },
            
            ["ar"] = new Dictionary<string, string>
            {
                ["AppName"] = "???? ????",
                ["Welcome"] = "??????",
                ["Goodbye"] = "??????",
                ["HelloUser"] = "??????? {0}!",
                ["Login_Title"] = "?????? ??????",
                ["Login_Button"] = "????? ??????",
                // ... more strings
            }
        };

        private static string _currentLanguage = "en";

        public static string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage != value && Translations.ContainsKey(value))
                {
                    _currentLanguage = value;
                    
                    // Set .NET culture for date/number formatting
                    CultureInfo.CurrentUICulture = new CultureInfo(value);
                    CultureInfo.CurrentCulture = new CultureInfo(value);
                }
            }
        }

        public static string GetString(string key)
        {
            // Try current language
            if (Translations.TryGetValue(_currentLanguage, out var languageStrings) &&
                languageStrings.TryGetValue(key, out var value))
            {
                return value;
            }

            // Fallback to English
            if (_currentLanguage != "en" &&
                Translations.TryGetValue("en", out var englishStrings) &&
                englishStrings.TryGetValue(key, out var fallbackValue))
            {
                return fallbackValue;
            }

            // Return key if not found (helps debugging)
            return key;
        }

        public static string GetFormattedString(string key, params object[] args)
        {
            var format = GetString(key);
            try
            {
                return string.Format(format, args);
            }
            catch
            {
                return format;
            }
        }

        // RTL Support
        public static bool IsRTL => _currentLanguage == "ar" || _currentLanguage == "he";
        public static FlowDirection FlowDirection => 
            IsRTL ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
    }
}
```

**Key Features:**
- ? Dictionary structure (easy to manage)
- ? Automatic fallback to English
- ? Returns key if missing (debugging aid)
- ? Culture-aware formatting
- ? RTL support

---

### Step 2: Create LocalizationService.cs

**Location:** `Source/Services/LocalizationService.cs`

```csharp
using AlfaGrid.Resources.Localization;
using AlfaGrid.Source.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AlfaGrid.Source.Services
{
    public interface ILocalizationService
    {
        string CurrentLanguage { get; set; }
        FlowDirection FlowDirection { get; }
        string this[string key] { get; }
        event EventHandler LanguageChanged;
        string GetString(string key);
        string GetFormattedString(string key, params object[] args);
    }

    public class LocalizationService : INotifyPropertyChanged, ILocalizationService
    {
        private const string LANGUAGE_KEY = "app_language";
        
        public event EventHandler LanguageChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public LocalizationService()
        {
            // Load saved language preference
            var savedLanguage = Preferences.Get(LANGUAGE_KEY, "en");
            AppResources.CurrentLanguage = savedLanguage;
        }

        public string CurrentLanguage
        {
            get => AppResources.CurrentLanguage;
            set
            {
                if (AppResources.CurrentLanguage != value)
                {
                    AppResources.CurrentLanguage = value;
                    Preferences.Set(LANGUAGE_KEY, value);
                    
                    // Three-level notification system:
                    
                    // 1. PropertyChanged (for XAML bindings)
                    OnPropertyChanged();
                    OnPropertyChanged("Item[]");
                    OnPropertyChanged(nameof(FlowDirection));
                    
                    // 2. Event (for event subscribers)
                    LanguageChanged?.Invoke(this, EventArgs.Empty);
                    
                    // 3. Messenger (for instant updates everywhere!)
                    WeakReferenceMessenger.Default.Send(new LanguageChangedMessage(value));
                }
            }
        }
 
        public FlowDirection FlowDirection => AppResources.FlowDirection;
        public string this[string key] => AppResources.GetString(key);
        public string GetString(string key) => AppResources.GetString(key);
        public string GetFormattedString(string key, params object[] args) 
            => AppResources.GetFormattedString(key, args);

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
```

**Key Features:**
- ? Persists language with Preferences
- ? Three notification mechanisms
- ? INotifyPropertyChanged for bindings
- ? Messenger for instant updates

---

### Step 3: Create TranslateExtension.cs

**Location:** `Source/Converters/TranslateExtension.cs`

```csharp
using AlfaGrid.Source.Helpers;
using AlfaGrid.Source.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace AlfaGrid.Source.Converters
{
    [ContentProperty(nameof(Key))]
    public class TranslateExtension : IMarkupExtension<BindingBase>
    {
        public string Key { get; set; } = string.Empty;

        public BindingBase ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrWhiteSpace(Key))
                return new Binding();

            var localizationService = ServiceHelper.GetRequiredService<ILocalizationService>();

            return new Binding(path: $"[{Key}]")
            {
                Source = localizationService,
                Mode = BindingMode.OneWay
            };
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
            => ProvideValue(serviceProvider);
    }
}
```

**Purpose:** Simplifies XAML - `{local:Translate Welcome}`

---

### Step 4: Create LanguageChangedMessage.cs

**Location:** `Source/Messages/LanguageChangedMessage.cs`

```csharp
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AlfaGrid.Source.Messages
{
    /// <summary>
    /// Message broadcast when language changes.
    /// All subscribed pages/viewmodels refresh instantly.
    /// </summary>
    public class LanguageChangedMessage : ValueChangedMessage<string>
    {
        public LanguageChangedMessage(string newLanguage) : base(newLanguage)
        {
        }

        public string NewLanguage => Value;
    }
}
```

**Purpose:** Broadcasts language changes to entire app instantly

---

### Step 5: Register Service

**In MauiProgram.cs:**

```csharp
using AlfaGrid.Source.Services;
using AlfaGrid.Source.Helpers;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Register LocalizationService as Singleton
        builder.Services.AddSingleton<ILocalizationService, LocalizationService>();

        var app = builder.Build();

        // Initialize ServiceHelper for markup extensions
        ServiceHelper.Initialize(app.Services);

        return app;
    }
}
```

---

## ?? Advanced Patterns

### Pattern 1: Listen for Language Changes

Any ViewModel can subscribe to language changes:

```csharp
using CommunityToolkit.Mvvm.Messaging;
using AlfaGrid.Source.Messages;

public partial class HomeViewModel : BaseViewModel, IRecipient<LanguageChangedMessage>
{
    private readonly ILocalizationService _localization;

    public HomeViewModel(ILocalizationService localization)
    {
        _localization = localization;
        
        // Register for messages
        WeakReferenceMessenger.Default.Register<LanguageChangedMessage>(this);
    }

    public void Receive(LanguageChangedMessage message)
    {
        // Language changed! Update UI
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Update C# strings not using bindings
            Title = _localization.GetString("Home_Title");
            
            // Refresh collections if needed
            RefreshData();
        });
    }
}
```

### Pattern 2: Update AppShell/Flyout

```csharp
public partial class AppShellViewModel : BaseViewModel, IRecipient<LanguageChangedMessage>
{
    private readonly ILocalizationService _localization;

    public AppShellViewModel()
    {
        _localization = ServiceHelper.GetRequiredService<ILocalizationService>();
        WeakReferenceMessenger.Default.Register<LanguageChangedMessage>(this);
    }

    public void Receive(LanguageChangedMessage message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (Shell.Current != null)
            {
                Shell.Current.FlowDirection = _localization.FlowDirection;
            }
        });
    }
}
```

### Pattern 3: Formatted Strings

**In AppResources:**
```csharp
["WelcomeUser"] = "Welcome back, {0}!",
["ItemCount"] = "You have {0} items",
["DateFormat"] = "Last updated: {0:dd/MM/yyyy}",
```

**Usage:**
```csharp
var message = _localization.GetFormattedString("WelcomeUser", userName);
// Result: "Welcome back, John!"

var count = _localization.GetFormattedString("ItemCount", items.Count);
// Result: "You have 5 items"

var date = _localization.GetFormattedString("DateFormat", DateTime.Now);
// Result: "Last updated: 21/12/2024"
```

---

## ?? Usage Examples

### Example 1: Simple XAML Text

```xaml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:AlfaGrid.Source.Converters"
             x:Class="AlfaGrid.Source.View.HomePage">
    
    <Label Text="{local:Translate Welcome}" />
    
</ContentPage>
```

### Example 2: Page Title

```xaml
<ContentPage xmlns:local="clr-namespace:AlfaGrid.Source.Converters"
             Title="{local:Translate Settings_Title}">
</ContentPage>
```

### Example 3: Button

```xaml
<Button Text="{local:Translate Login_Button}" 
        Command="{Binding LoginCommand}" />
```

### Example 4: In ViewModel

```csharp
public class MyViewModel : BaseViewModel
{
    private readonly ILocalizationService _localization;

    public MyViewModel(ILocalizationService localization)
    {
        _localization = localization;
        
        // Simple string
        var message = _localization.GetString("Welcome");
        
        // Formatted string
        var greeting = _localization.GetFormattedString("HelloUser", "John");
    }
}
```

### Example 5: Language Selector

```csharp
public partial class SettingsViewModel : BaseViewModel
{
    private readonly ILocalizationService _localization;

    [ObservableProperty]
    private string selectedLanguage;

    public string SelectedLanguageDisplay => 
        SelectedLanguage == "ar" ? "???????" : "English";

    [RelayCommand]
    private async Task SelectLanguage()
    {
        var action = await Shell.Current.DisplayActionSheet(
            _localization.GetString("SelectLanguage"),
            _localization.GetString("Cancel"),
            null,
            "English",
            "???????");

        if (action == "English")
            ChangeLanguage("en");
        else if (action == "???????")
            ChangeLanguage("ar");
    }

    private void ChangeLanguage(string language)
    {
        SelectedLanguage = language;
        _localization.CurrentLanguage = language;
        OnPropertyChanged(nameof(SelectedLanguageDisplay));

        // Force current page refresh
        if (Shell.Current.CurrentPage is Page currentPage)
        {
            currentPage.FlowDirection = _localization.FlowDirection;
            
            var context = currentPage.BindingContext;
            currentPage.BindingContext = null;
            currentPage.BindingContext = context;
        }
    }
}
```

---

## ?? Testing & Debugging

### Test Checklist

? **Language Switching**
```
1. Open app in English
2. Navigate to Settings
3. Change to Arabic
4. Verify:
   - All text changes to Arabic
   - Layout flips to RTL
   - No missing translations (no keys shown)
   - Navigation works correctly
```

? **Page Navigation**
```
1. Change language on Page A
2. Navigate to Page B
3. Verify Page B shows new language
4. Go back to Page A
5. Verify Page A still shows new language
```

? **App Restart**
```
1. Change language to Arabic
2. Close app completely
3. Reopen app
4. Verify app opens in Arabic
```

? **RTL Testing**
```
1. Switch to Arabic
2. Check all pages:
   - Text aligned right
   - Icons on correct side
   - Navigation arrows flipped
   - Proper spacing
```

### Debugging Tips

**1. Find Missing Translations**

```csharp
public static string GetString(string key)
{
    if (Translations.TryGetValue(_currentLanguage, out var languageStrings) &&
        languageStrings.TryGetValue(key, out var value))
    {
        return value;
    }

    // Add logging
    Debug.WriteLine($"?? Missing translation: {key} in {_currentLanguage}");

    // Return key so you see what's missing
    return key;
}
```

**2. Visualize Current Language**

```xaml
<!-- Add to every page during development -->
<Label Text="{Binding Source={x:Static local:AppResources.CurrentLanguage}}"
       FontSize="10"
       TextColor="Red"
       HorizontalOptions="End"
       VerticalOptions="Start"/>
```

**3. Test Translations**

```csharp
[Test]
public void AllKeysHaveTranslations()
{
    var englishKeys = AppResources.Translations["en"].Keys;
    var arabicKeys = AppResources.Translations["ar"].Keys;
    
    Assert.Equal(englishKeys, arabicKeys);
}
```

---

## ? Best Practices

### DO's

**1. Use Consistent Key Naming**
```csharp
["PageName_ElementName"] = "Text"

// Example:
["Login_Title"] = "Welcome"
["Login_Subtitle"] = "Sign in to continue"
["Login_Button"] = "Sign In"
["Settings_Language"] = "Language"
```

**2. Group Related Strings**
```csharp
// Group by page/feature
["Login_Title"]
["Login_Subtitle"]
["Login_Button"]

["Settings_Title"]
["Settings_Language"]
["Settings_Logout"]
```

**3. Use Formatted Strings**
```csharp
["WelcomeUser"] = "Welcome, {0}!"

// Usage
var text = GetFormattedString("WelcomeUser", userName);
```

**4. Always Provide Fallback**
```csharp
public static string GetString(string key)
{
    // Try current language ? English ? key
}
```

**5. Test on Real Devices**
- Emulator behavior differs
- RTL might look different
- Test with actual Arabic device

### DON'Ts

**1. Don't Hardcode Strings**
```xaml
<!-- ? Bad -->
<Label Text="Welcome" />

<!-- ? Good -->
<Label Text="{local:Translate Welcome}" />
```

**2. Don't Forget RTL Testing**
- Images might need mirroring
- Navigation directions reverse
- Icon positions change

**3. Don't Ignore Formatting**
```csharp
// ? Bad
$"{date}" // Uses current culture

// ? Good
_localization.GetFormattedString("DateFormat", date)
```

**4. Don't Create Language Silos**
- Keep all translations in ONE place
- Don't scatter across multiple files

**5. Don't Block UI Thread**
```csharp
// ? Bad
LoadTranslations(); // Heavy operation on UI thread

// ? Good
Task.Run(() => LoadTranslations());
```

---

## ?? Troubleshooting

### Problem: Translation Not Showing

**Symptom:** Seeing key instead of translation (e.g., "Login_Title")

**Solutions:**
1. ? Check key exists in AppResources.cs
2. ? Verify spelling matches exactly
3. ? Ensure namespace imported in XAML:
   ```xaml
   xmlns:local="clr-namespace:AlfaGrid.Source.Converters"
   ```

### Problem: Page Not Updating

**Symptom:** Changed language but page still shows old language

**Solution:**
```csharp
// Force page refresh
if (Shell.Current.CurrentPage is Page page)
{
    page.FlowDirection = _localization.FlowDirection;
    var context = page.BindingContext;
    page.BindingContext = null;
    page.BindingContext = context;
}
```

### Problem: Flyout Not Updating

**Symptom:** Menu stays in old language

**Solution:** Ensure AppShellViewModel implements `IRecipient<LanguageChangedMessage>`

### Problem: RTL Not Working

**Symptom:** Arabic text but LTR layout

**Solution:**
1. Check `IsRTL` property returns true for "ar"
2. Verify FlowDirection is set:
   ```csharp
   page.FlowDirection = _localization.FlowDirection;
   ```

---

## ?? Available Translation Keys

### Common (8 keys)
`AppName`, `OK`, `Cancel`, `Apply`, `Reset`, `Error`, `Success`, `Loading`

### Login (9 keys)
`Login_Title`, `Login_Subtitle`, `Login_Email`, `Login_Password`, `Login_ForgotPassword`, `Login_Button`, `Login_NoAccount`, `Login_SignUp`, `Login_LanguageLabel`

### Home (9 keys)
`Home_SearchPlaceholder`, `Home_Directions`, `Home_Reserve`, `Home_ScanQR`, `Home_Favorite`, `Home_GoBack`, `Home_ViewFullDetails`, `Home_NoReviews`, `Home_Available`

### Filter (14 keys)
`Filter_Title`, `Filter_Sorting`, `Filter_Time`, `Filter_Rating`, `Filter_AllRatings`, `Filter_OnlyAbove`, `Filter_SelectMinRating`, `Filter_StarsAndAbove`, `Filter_Locations`, `Filter_24HoursOpen`, `Filter_AvailableNow`, `Filter_FreeParking`, `Filter_Wifi`, `Filter_ConnectorTypes`

### Settings (14 keys)
`Settings_Title`, `Settings_General`, `Settings_Language`, `Settings_Notifications`, `Settings_PushNotifications`, `Settings_EmailNotifications`, `Settings_Account`, `Settings_ChangePassword`, `Settings_DeleteAccount`, `Settings_About`, `Settings_TermsAndConditions`, `Settings_PrivacyPolicy`, `Settings_Version`, `Settings_Logout`

### Location Details (12 keys)
`LocationDetails_Title`, `LocationDetails_Overview`, `LocationDetails_Photos`, `LocationDetails_Reviews`, `LocationDetails_Facilities`, `LocationDetails_Information`, `LocationDetails_StartCharging`, `LocationDetails_NoImages`, `LocationDetails_Socket`, `LocationDetails_MaxPower`, `LocationDetails_TariffDescription`, `LocationDetails_FreeTariff`

**Total: 250+ translation keys**

---

## ?? Quick Reference

### Add Translation
```csharp
// AppResources.cs
["YourKey"] = "Your English Text",
```

### Use in XAML
```xaml
<Label Text="{local:Translate YourKey}" />
```

### Use in C#
```csharp
var text = _localization.GetString("YourKey");
```

### Change Language
```csharp
_localization.CurrentLanguage = "ar";
```

### Listen for Changes
```csharp
WeakReferenceMessenger.Default.Register<LanguageChangedMessage>(this);
```

---

## ?? Additional Resources

- [Microsoft MAUI Localization](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/localization)
- [CommunityToolkit Messaging](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/messenger)
- [CultureInfo Class](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo)
- [RTL Languages Support](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/layouts/rtl)

---

**Last Updated:** December 2024  
**Version:** 1.0  
**Languages Supported:** English, Arabic (with RTL)

---

**End of Localization Guide**
