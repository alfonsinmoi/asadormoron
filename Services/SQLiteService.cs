using AsadorMoron.Interfaces;
using SQLite;
using System;
using System.IO;

namespace AsadorMoron.Services
{
    public class SQLiteService : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            try
            {
                var sqliteFilename = "AsadorMoron.db3";
                var path = Path.Combine(FileSystem.AppDataDirectory, sqliteFilename);

                var connection = new SQLiteConnection(path);

                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLiteService] Error: {ex.Message}");
                return null;
            }
        }
    }
}
