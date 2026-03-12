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
        
        public event EventHandler? LanguageChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

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
                    
                    OnPropertyChanged();
                    OnPropertyChanged("Item[]");
                    OnPropertyChanged(nameof(FlowDirection));
                    
                    // Notify all subscribers via event
                    LanguageChanged?.Invoke(this, EventArgs.Empty);
                    
                    // Send message via messenger pattern for in-place refresh
                    WeakReferenceMessenger.Default.Send(new LanguageChangedMessage(value));
                }
            }
        }
 
        public FlowDirection FlowDirection => AppResources.FlowDirection;
 
        public string this[string key] => AppResources.GetString(key);
 
        public string GetString(string key) => AppResources.GetString(key);

        public string GetFormattedString(string key, params object[] args) => AppResources.GetFormattedString(key, args);

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
