using AlfaGrid.Framework.FrameworkConfig;

namespace AlfaGrid.Framework.Data.Cache.Helper
{
    public static class DatabasePathHelper
    {
        public static string GetLocalFilePath()
        {
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string libFolder = Path.Combine(docFolder, "..", "Library", "Databases");

                if (!Directory.Exists(libFolder))
                {
                    Directory.CreateDirectory(libFolder);
                }
                return libFolder;
            }
            else if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            else
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
        }

        public static string GetDatabaseFileName()
        {
            return FrameworkConfigManager.Instance.FrameworkConfig.DbCacheName;
        }
    }
}