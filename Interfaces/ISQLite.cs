using SQLite;

namespace AsadorMoron.Interfaces
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
