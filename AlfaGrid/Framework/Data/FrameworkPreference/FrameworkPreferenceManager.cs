namespace AlfaGrid.Framework.Data.FrameworkPreference
{
    public class FrameworkPreferenceManager : IFrameworkPreferenceManager
    {
        public void Setstring(string key, string value)
        {
            Preferences.Set(key, value);
        }
        public string GetString(string key)
        {
            return Preferences.Get(key, string.Empty);
        }

        public void SetInt(string key, int value)
        {
            Preferences.Set(key, value);
        }
        public int GetInt(string key)
        {
            return Preferences.Get(key, 0);
        }

        public void SetBoolean(string key, bool value)
        {
            Preferences.Set(key, value);
        }
        public bool GetBoolean(string key)
        {
            return Preferences.Get(key, false);
        }

        public void SetDouble(string key, double value)
        {
            Preferences.Set(key, value);
        }
        public double GetDouble(string key)
        {
            return Preferences.Get(key, 0);
        }

        public void SetFloat(string key, float value)
        {
            Preferences.Set(key, value);
        }
        public float GetFloat(string key)
        {
            return Preferences.Get(key, 0);
        }

        public void SetLong(string key, long value)
        {
            Preferences.Set(key, value);
        }
        public long GetLong(string key)
        {
            return Preferences.Get(key, 0);
        }
    }
}

