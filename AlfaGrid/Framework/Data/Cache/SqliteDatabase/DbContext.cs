using AlfaGrid.Framework.Data.Cache.Helper;
using SQLite;

namespace AlfaGrid.Framework.Data.Cache.SqliteDatabase
{
    public class DbContext
    {
        public static string LocalFilePath; // Set this before creating from platform project
        public SQLiteAsyncConnection Database { get; }
        /// <summary>
        /// Initialized a new DbContext
        /// </summary>
        public DbContext()
        {
            Database = new SQLiteAsyncConnection(
                Path.Combine(
                    FileSystem.AppDataDirectory,
                    DatabasePathHelper.GetDatabaseFileName()));
            //Database = new SQLiteAsyncConnection(
            //    Path.Combine(
            //        Helper.DatabasePathHelper.GetLocalFilePath(),
            //        Helper.DatabasePathHelper.GetDatabaseFileName()));
        }

        /// <summary>
        /// Creates a table for a given type in sql lite
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<CreateTableResult> CreateTableAsync<T>() where T : new()
        {
            return await Database.CreateTableAsync<T>();
        }

        /// <summary>
        /// Gets a table by it's type from the db.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public AsyncTableQuery<T> Set<T>() where T : new()
        {
            return Database.Table<T>();
        }
    }
}