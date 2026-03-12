using AlfaGrid.Framework.Data.FrameworkPreference;

namespace AlfaGrid.Source.AppPreferences
{
    public class AppPreferenceManager
    {
        IFrameworkPreferenceManager frameworkPreferenceManager;

        private AppPreferenceManager()
        {
            frameworkPreferenceManager = new FrameworkPreferenceManager();

        }

        private static readonly Lazy<AppPreferenceManager> lazy =
            new(() => new AppPreferenceManager());

        public static AppPreferenceManager Instance => lazy.Value;

        public void SaveInt(AppPreferenceKeyEnum appPreferenceKeyEnum, int value)
        {
            frameworkPreferenceManager.SetInt(appPreferenceKeyEnum.ToString(), value);
        }
        public int GetInt(AppPreferenceKeyEnum appPreferenceKeyEnum)
        {
            return frameworkPreferenceManager.GetInt(appPreferenceKeyEnum.ToString());
        }

        public void Savestring(AppPreferenceKeyEnum appPreferenceKeyEnum, string value)
        {
            frameworkPreferenceManager.Setstring(appPreferenceKeyEnum.ToString(), value);
        }
        public string GetString(AppPreferenceKeyEnum appPreferenceKeyEnum)
        {
            return frameworkPreferenceManager.GetString(appPreferenceKeyEnum.ToString());
        }

        public void SaveBoolean(AppPreferenceKeyEnum appPreferenceKeyEnum, bool value)
        {
            frameworkPreferenceManager.SetBoolean(appPreferenceKeyEnum.ToString(), value);
        }
        public bool GetBoolean(AppPreferenceKeyEnum appPreferenceKeyEnum)
        {
            return frameworkPreferenceManager.GetBoolean(appPreferenceKeyEnum.ToString());
        }

        public void SaveDouble(AppPreferenceKeyEnum appPreferenceKeyEnum, double value)
        {
            frameworkPreferenceManager.SetDouble(appPreferenceKeyEnum.ToString(), value);
        }
        public double GetDouble(AppPreferenceKeyEnum appPreferenceKeyEnum)
        {
            return frameworkPreferenceManager.GetDouble(appPreferenceKeyEnum.ToString());
        }

        public void SaveFloat(AppPreferenceKeyEnum appPreferenceKeyEnum, float value)
        {
            frameworkPreferenceManager.SetFloat(appPreferenceKeyEnum.ToString(), value);
        }
        public float GetFloat(AppPreferenceKeyEnum appPreferenceKeyEnum)
        {
            return frameworkPreferenceManager.GetFloat(appPreferenceKeyEnum.ToString());
        }

        public void SaveLong(AppPreferenceKeyEnum appPreferenceKeyEnum, long value)
        {
            frameworkPreferenceManager.SetLong(appPreferenceKeyEnum.ToString(), value);
        }
        public long GetLong(AppPreferenceKeyEnum appPreferenceKeyEnum)
        {
            return frameworkPreferenceManager.GetLong(appPreferenceKeyEnum.ToString());
        }
    }
}