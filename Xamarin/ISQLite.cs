using SQLite;
using SQLite.Net.Async;
using SQLite.Net.Interop;


namespace XamarinApp.LocalDb
{
    public interface ISQLite
    {
        SQLiteAsyncConnection GetConnection(ISQLitePlatform platform);
    }
}
