using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AlfaGrid.Source.Messages
{
    /// <summary>
    /// Message sent when the application language changes.
    /// All pages/viewmodels subscribed to this message will refresh their UI.
    /// </summary>
    public class LanguageChangedMessage : ValueChangedMessage<string>
    {
        public LanguageChangedMessage(string newLanguage) : base(newLanguage)
        {
        }

        /// <summary>
        /// Gets the new language code (e.g., "en", "ar")
        /// </summary>
        public string NewLanguage => Value;
    }
}
