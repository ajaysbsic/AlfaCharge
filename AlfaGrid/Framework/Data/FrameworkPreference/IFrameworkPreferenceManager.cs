namespace AlfaGrid.Framework.Data.FrameworkPreference
{
    public interface IFrameworkPreferenceManager
    {
        public void Setstring(string key, string value);
        public string GetString(string key);

        public void SetInt(string key, int value);
        public int GetInt(string key);

        public void SetBoolean(string key, bool value);
        public bool GetBoolean(string key);

        public void SetDouble(string key, double value);
        public double GetDouble(string key);

        public void SetFloat(string key, float value);
        public float GetFloat(string key);

        public void SetLong(string key, long value);
        public long GetLong(string key);

    }
}