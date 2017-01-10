using XamarinApp.LocalDb.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Interop;
using Xamarin.Forms;

namespace XamarinApp.LocalDb
{
    public class LocalDbRepo : ViewModelBase
    {
        protected readonly SQLiteAsyncConnection _connection;
        public LocalDbRepo(ISQLitePlatform platform)
        {
            _connection = GetConnection(platform);
        }

        /// <summary>The single point of querying the local DB. All other methods cascade here
        /// 
        /// </summary>
        /// <typeparam name="T">The object type we are working with</typeparam>
        /// <param name="query">Not to include the type, just the basic query portion
        /// <example>"select * from "</example></param>
        /// <param name="clause"></param>
        /// <param name="data"></param>
        /// <param name="maxResults">Default is 1.  If you do not want a LIMIT= condition placed on your query then specify a max of 0. For example, delete does not allow a limit thus LIMIT=1 on a delete throws an SQL error; so specify 0</param>
        /// <returns></returns>
        public List<T> RunQuery<T>(string query, string clause = "",
                                   string data = "", int maxResults = 1) where T : class, new()
        {
            if (_connection == null)
            {
                logger?.Error("RunQuery Error: Connection is null");
                return null;
            }

            var type = typeof(T);
            var limitString = maxResults > 0 ? string.Format(" limit {0};", maxResults) : ";";

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (props.FirstOrDefault() != null)
            {
                try
                {
                    #region Build the query including/omitting clauses as specified

                    var querystring = string.Format("{0} {1}", query, typeof(T).Name);

                    if (!string.IsNullOrWhiteSpace(clause) && !string.IsNullOrWhiteSpace(data))
                    {
                        // If the clause is just the table name assume we are doing a 
                        // 'where blah = wonk'.  If the caller wanted something more specific than
                        // that they need to build it.
                        if (!clause.Contains("where")) clause = " where " + clause;
                        if (!clause.EndsWith(" ") && !clause.Contains("=")) clause += "=";
                    }

                    querystring += string.Format(" {0} {1}",
                                                    clause,
                                                    data);

                    querystring += limitString;

                    #endregion Build the query including/omitting clauses as specified

                    var result =  _connection.QueryAsync<T>(querystring);
                    if (result != null) return result.Result;
                }
                catch (Exception ex)
                {
                    logger?.Error("RunQuery Exception: {message}", ex.Message);
                }
            }
            return new List<T>();// If all else fails, we return an empty list so as to not break binding with a null return
        }

        public async Task CreateDatabaseTableAsync<T>() where T : class
        {
            if (_connection != null) await _connection.CreateTableAsync<T>().ConfigureAwait(false);
        }

        public  List<T> GetAllItems<T>(string clause = "", string data = "") where T : class, new()
        {
            if (_connection == null)
            {
                logger?.Error("GetAllItems Error: Connection is null");
                return null;
            }
            return  GetItems<T>(clause, data, Int32.MaxValue);
        }

        public object GetItem<T>(string clause = "", string data = "") where T : class, new()
        {
            if (_connection == null)
            {
                logger?.Error("GetItem Error: Connection is null");
                return null;
            }
            var result = GetItems<T>(clause, data, 1);
            if (result != null && result.Any()) return result[0];
            return null;
        }

        public  List<T> GetItems<T>(string clause = "", string data = "", int MaxResults = 1) where T : class, new()
        {
            if (_connection == null)
            {
                logger?.Error("GetItems Error: Connection is null");
                return null;
            }
            var type = typeof(T);

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (props.FirstOrDefault() != null)
            {
                try
                {
                    var querystring = string.Format("select * from ");

                    if (!string.IsNullOrWhiteSpace(data) && !data.Contains(@"'"))
                        data = string.Format("'{0}'", data);

                    List<T> result = RunQuery<T>(querystring, clause, data, MaxResults);
                    if (result != null) return result;
                }
                catch (Exception ex)
                {
                    logger?.Error("GetItems Exception: {message}", ex.Message);
                }
            }
            return null;// If all else fails, we return null;
        }

        /// <summary>Basic "where clause = value" query.
        /// Deletes an item from the database
        /// </summary>
        /// <typeparam name="T">A model to pass to the database</typeparam>
        /// <param name="clause">The database field property we are searching against</param>
        /// <param name="value">The value of the clause</param>
        /// <returns>A string that will be set by method</returns>
        public  bool DeleteItem<T>(string clause, string value) where T : class, new()
        {
            if (_connection == null)
            {
                logger?.Error("DeleteItem Error: Connection is null");
                return false;
            }
            var type = typeof(T);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (props.FirstOrDefault() != null)
            {
                var result =  RunQuery<T>("delete from ", "where " + clause + " = ", value, 0);
                if (result != null && result.Count > 0)
                {
                    string successMessage = Xapp.GetResource("DB_DeleteSuccess") as string;
                    logger?.Information(successMessage);
                    return true;
                }
            }
            else
            {
                logger?.Warning("DeleteItem: class of {message} is not a database model or class", typeof(T).Name);
            }
            return false;
        }

        public bool CreateTable<T>() where T : class, new()
        {
            if (_connection == null)
            {
                logger?.Error("CreateTable Error: Connection is null");
                return false;
            }
            var type = typeof(T);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (props.FirstOrDefault() != null)
            {
                try
                {
                    var querystring = string.Format("SELECT name FROM sqlite_master WHERE type='table' AND name='{0}'",
                        typeof(T).Name);

                    var result = _connection.QueryAsync<T>(querystring);
                    if (result != null && result.Result.Count > 0)
                    {
                        // Table exists
                    }
                    else
                    {
                        CreateDatabaseTableAsync<T>().ConfigureAwait(true);
                        string successMessage = Xapp.GetResource("DB_TableSuccess") as string;
                    logger?.Information("{name}: {message}", type.ToString(), successMessage);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    logger?.Information("{name}: {message}", type.ToString(), ex.Message);
                }

                return false;
            }

            logger?.Warning("SaveItem: class of {message} is not a database model or class", typeof(T).Name);

            return false;
        }

        /// <summary>
        /// Updates an existing item in the database
        /// </summary>
        /// <typeparam name="T">A model to pass to the database</typeparam>
        /// <param name="obj">An instanced version of the model that will be used to access the database</param>
        /// <returns>A string that will be set by method</returns>
        public bool UpdateItem<T>(T obj) where T : class, new()
        {
            if (_connection == null)
            {
                logger?.Error("UpdateItem Error: Connection is null");
                return false;
            }
            var type = typeof(T);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (props.FirstOrDefault() != null)
            {
                try
                {
                    CreateTable<T>();
                    _connection.UpdateAsync(obj).ConfigureAwait(true);//Exception if failed
                    string successMessage = Xapp.GetResource("DB_UpdateSuccess") as string;
                    logger?.Information(successMessage);
                }
                catch (Exception ex)
                {
                    logger?.Error("UpdateItem failed> {ex}", ex);
                }
                return true;
            }

            logger?.Warning("UpdateItem: class of {message} is not a database model or class", typeof(T).Name);

            return false;
        }

        /// <summary>
        /// Saves an item to the database.
        /// ALWAYS RETURN FALSE because its internal workings are sync.
        /// </summary>
        /// <typeparam name="T">A model to pass to the database</typeparam>
        /// <param name="obj">An instanced version of the model that will be used to access the database</param>
        /// <returns>A string that will be set by method</returns>
        public bool SaveItem<T>(T obj) where T : class, new()
        {
            if (_connection == null)
            {
                logger?.Error("SaveItem Error: Connection is null");
                return false;
            }
            var type = typeof(T);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (props.FirstOrDefault() != null)
            {
                try
                {
                    CreateTable<T>();
                    var blah = _connection.InsertAsync(obj);//Throws exception if failed
                    string successMessage = Xapp.GetResource("DB_SaveSuccess") as string;
                    logger?.Information("{name}: {message}", obj.ToString(),successMessage);
                }
                catch (Exception ex)
                {
                    logger?.Information("{name}: {message}", obj.ToString(), ex.Message);
                }
                return true;
            }

            logger?.Warning("SaveItem: class of {message} is not a database model or class", typeof(T).Name);

            return false;
        }

        protected virtual SQLiteAsyncConnection GetConnection(ISQLitePlatform platform)
        {
            var con = new SQLiteClient().GetConnection(platform);
            ConfigureLogger(Xapp.ModuleNames.LocalDbRepo);
            logger?.Debug("Log file created");
            //logger?.Debug("SQLite database connection established");

            return con;
        }
    }
}