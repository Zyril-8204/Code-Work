using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Interop;
using Xamarin.Forms;

namespace XamarinApp.LocalDb
{
    public class SQLiteClient : ViewModelBase, ISQLite
    {
        public SQLiteAsyncConnection GetConnection(ISQLitePlatform platform)
        {
            string dbSubdirectory = string.Empty;
            string createdDirectory = string.Empty;
            string dbFileName = string.Empty;
            try
            {
                dbSubdirectory = Xapp.GetResource("LocalDbDirectoryPath") as string;
                createdDirectory = FileHelper.CreateFolder(dbSubdirectory);
                dbFileName = Xapp.GetResource("DB_FileName") as string;
                var fullFilePath = System.IO.Path.Combine(createdDirectory, dbFileName);
                var connectionWithLock = new SQLiteConnectionWithLock(platform, new SQLiteConnectionString(fullFilePath, true));

                var connection = new SQLiteAsyncConnection(() => connectionWithLock);
                return connection;

            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.Error("SQLite Client: {error}", "Unable to set DB info through constants!");
                    logger.Error(ex.Message);
                }
                return null;
            }
        }
    }
}
