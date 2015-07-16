using PulseBackend.DAL;
using PulseBackend.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace PulseBackend.API
{
    public class ClientsController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post()
        {
            string Name = string.Empty, Mission = string.Empty, Vision = string.Empty, MindtreeIndustGroup = string.Empty, BusinessUnit = string.Empty, YearsAgile = string.Empty, AdditionalNotes = string.Empty, DateJoined = string.Empty, username = string.Empty, MID = string.Empty;
            var httpRequest = HttpContext.Current.Request;
            username = Helper.GetUserName(httpRequest);
            if (string.IsNullOrEmpty(username))
            {
                using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                {
                    w.WriteLine("API/Clients/Put");
                    w.WriteLine("date:" + DateTime.Now);
                    w.WriteLine("Username is invalid");
                    w.WriteLine("----------------------------------------");
                }
                throw new HttpException(500, "NTLM ERROR");
            }

            bool UserInDB = false;
            
            foreach (string key in httpRequest.Form.Keys)
            {
                var tmpKey = key;
                var tmpValue = httpRequest.Form[key];

                if (tmpKey == "Name")
                {
                    Name = tmpValue;
                }
                else if (tmpKey == "Mission")
                {
                    Mission = tmpValue;
                }
                else if (tmpKey == "Vision")
                {
                    Vision = tmpValue;
                }
                else if (tmpKey == "MindtreeIndustGroup")
                {
                    MindtreeIndustGroup = tmpValue;
                }
                else if (tmpKey == "BusinessUnit")
                {
                    BusinessUnit = tmpValue;
                }
                else if (tmpKey == "YearsAgile")
                {
                    YearsAgile = tmpValue;
                }
                else if (tmpKey == "AdditionalNotes")
                {
                    AdditionalNotes = tmpValue;
                }
                else if (tmpKey == "DateJoined")
                {
                    DateJoined = tmpValue;
                }
                else if(tmpKey =="MID")
                {
                    MID = tmpValue;
                }   
            }

            if (string.IsNullOrEmpty(MID))
            {
                MID = username +",";
            }
            else
            {
                MID += "," + username + ",";
            }

            DateTime dateJoined = Convert.ToDateTime(DateJoined);
            try
            {
                SqlParameter[] getUserSQLParams =
                    {
                        new SqlParameter("@MID", username)
                    };
                SqlParameter[] createUserSQLParams =
                    {
                        new SqlParameter("@MID", username)
                    };
                SqlParameter[] sqlParams =
                    {
                        new SqlParameter("@AdditionalNotes", AdditionalNotes),
                        new SqlParameter("@BusinessUnit", BusinessUnit),
                        new SqlParameter("@DateJoined",dateJoined),
                        new SqlParameter("@MindtreeIndustGroup",MindtreeIndustGroup),
                        new SqlParameter("@Mission",Mission),
                        new SqlParameter("@Name",Name),
                        new SqlParameter("@Vision",Vision),
                        new SqlParameter("@YearsAgile",YearsAgile),
                        new SqlParameter("@MID",username),
                    };

                foreach (SqlParameter parameter in sqlParams)
                {
                    if (parameter.Value == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                }

                using (ApplicationDbContext db = new ApplicationDbContext())
                {

                    // try to get username
                    var UserExists = db.Database.SqlQuery<string>("GetUser @MID", getUserSQLParams).ToList();
                    if (UserExists != null)
                    {
                        if (UserExists.Any())
                        {
                            foreach (var u in UserExists)
                            {
                                if (!string.IsNullOrEmpty(u))
                                {
                                    // User Exists so we can continue
                                    UserInDB = true;
                                }
                                else
                                {
                                    UserInDB = false;
                                }
                            }
                        }
                        else
                        {
                            UserInDB = false;
                        }
                    }
                    else
                    {
                        UserInDB = false;
                    }
                    // If the user doesn't exist we will create user
                    if (UserInDB == false)
                    {
                        var CreateUser = db.Database.SqlQuery<string>("CreateUser @MID", createUserSQLParams).ToList();
                        if (CreateUser != null)
                        {
                            if (CreateUser.Any())
                            {
                                foreach (var u in CreateUser)
                                {
                                    if (!string.IsNullOrEmpty(u))
                                    {
                                        // User Exists so we can continue
                                        UserInDB = true;
                                    }
                                    else
                                    {
                                        using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                                        {
                                            w.WriteLine("API/Clients/Post");
                                            w.WriteLine("date:" + DateTime.Now);
                                            w.WriteLine("User can not create");
                                            w.WriteLine("----------------------------------------");
                                        }
                                        throw new HttpException(500, "Database error please contact support");
                                    }
                                }
                            }
                            else
                            {
                                using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                                {
                                    w.WriteLine("API/Clients/Post");
                                    w.WriteLine("date:" + DateTime.Now);
                                    w.WriteLine("User can not create");
                                    w.WriteLine("----------------------------------------");
                                }
                                throw new HttpException(500, "Database error please contact support");
                            }
                        }
                        else
                        {
                            using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                            {
                                w.WriteLine("API/Clients/Post");
                                w.WriteLine("date:" + DateTime.Now);
                                w.WriteLine("User can not create");
                                w.WriteLine("----------------------------------------");
                            }
                            throw new HttpException(500, "Database error please contact support");
                        }
                    }


                    if (UserInDB == true)
                    {
                        var result = db.Database.SqlQuery<int>("CreateClient @AdditionalNotes, @BusinessUnit, @DateJoined, @MindtreeIndustGroup, @Mission, @Name, @Vision, @YearsAgile, @MID", sqlParams).ToList();
                        if (result != null)
                        {
                            if (result.Any())
                            {
                                Client tmpClient = new Client();
                                tmpClient.Id = result.FirstOrDefault();
                                SqlParameter[] userClientSQLParams =
                                {
                                    new SqlParameter("@MID", username),
                                    new SqlParameter("@ClientID", tmpClient.Id)
                                };
                                var userClientResult = db.Database.SqlQuery<User>("GetUserClientMapping @MID, @ClientID", userClientSQLParams).ToList();
                                if (userClientResult != null)
                                {
                                    if (userClientResult.Any())
                                    {
                                        User tmpUser = new  User();
                                        tmpUser.Id = userClientResult.FirstOrDefault().Id;
                                        tmpUser.MID = userClientResult.FirstOrDefault().MID;
                                        tmpClient.Users = new List<User>();
                                        tmpClient.Users.Add(tmpUser);
                                    }
                                }
                                return Ok(tmpClient);
                            }
                            else
                            {
                                using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                                {
                                    w.WriteLine("API/Clients/Post");
                                    w.WriteLine("date:" + DateTime.Now);
                                    w.WriteLine("empty object");
                                    w.WriteLine("----------------------------------------");
                                }
                                throw new HttpException(500, "Database error please contact support");
                            }
                        }
                        else
                        {
                            using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                            {
                                w.WriteLine("API/Clients/Post");
                                w.WriteLine("date:" + DateTime.Now);
                                w.WriteLine("Null Object");
                                w.WriteLine("----------------------------------------");
                            }
                            throw new HttpException(500, "Database error please contact support");
                        }
                    }
                    else
                    {
                        using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                        {
                            w.WriteLine("API/Clients/Post");
                            w.WriteLine("date:" + DateTime.Now);
                            w.WriteLine("Error Creating User");
                            w.WriteLine("----------------------------------------");
                        }
                        throw new HttpException(500, "Database error please contact support");
                    }
                }

            }
            catch (Exception ex)
            {
                using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                {
                    w.WriteLine("API/Clients/Post");
                    w.WriteLine("date:" + DateTime.Now);
                    w.WriteLine(ex.Message);
                    w.WriteLine("----------------------------------------");
                }
                throw new HttpException(500, ex.Message);
            }
        }

        /// <summary>
        /// Gets all client information from database
        /// </summary>
        /// <returns>a list of clients</returns>
        [HttpGet]
        public IHttpActionResult Get()
        {
            string username = string.Empty;
            var httpRequest = HttpContext.Current.Request;
            username = Helper.GetUserName(httpRequest);
            if (string.IsNullOrEmpty(username))
            {
                using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                {
                    w.WriteLine("API/Clients/Put");
                    w.WriteLine("date:" + DateTime.Now);
                    w.WriteLine("Username is invalid");
                    w.WriteLine("----------------------------------------");
                }
                throw new HttpException(500, "NTLM ERROR");
            }
            bool UserInDB = false;
            List<Client> clients = new List<Client>();
            try
            {
                SqlParameter[] userSQLParams =
                    {
                        new SqlParameter("@MID", username)
                    };
                SqlParameter[] getUserSQLParams =
                    {
                        new SqlParameter("@MID", username)
                    };
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    // try to get username
                    var UserExists = db.Database.SqlQuery<string>("GetUser @MID", getUserSQLParams).ToList();
                    if (UserExists != null)
                    {
                        if (UserExists.Any())
                        {
                            foreach (var u in UserExists)
                            {
                                if (!string.IsNullOrEmpty(u))
                                {
                                    // User Exists so we can continue
                                    UserInDB = true;
                                }
                                else
                                {
                                    UserInDB = false;
                                }
                            }
                        }
                        else
                        {
                            UserInDB = false;
                        }
                    }
                    else
                    {
                        UserInDB = false;
                    }
                    // If the user doesn't exist we will create user
                    if (UserInDB == false)
                    {
                        var CreateUser = db.Database.SqlQuery<string>("CreateUser @MID", userSQLParams).ToList();
                        if (CreateUser != null)
                        {
                            if (CreateUser.Any())
                            {
                                foreach (var u in CreateUser)
                                {
                                    if (!string.IsNullOrEmpty(u))
                                    {
                                        // User Exists so we can continue
                                        UserInDB = true;
                                    }
                                    else
                                    {
                                        using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                                        {
                                            w.WriteLine("API/Clients/Get");
                                            w.WriteLine("date:" + DateTime.Now);
                                            w.WriteLine("User can not create");
                                            w.WriteLine("----------------------------------------");
                                        }
                                        throw new HttpException(500, "Database error please contact support");
                                    }
                                }
                            }
                            else
                            {
                                using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                                {
                                    w.WriteLine("API/Clients/Get");
                                    w.WriteLine("date:" + DateTime.Now);
                                    w.WriteLine("User can not create");
                                    w.WriteLine("----------------------------------------");
                                }
                                throw new HttpException(500, "Database error please contact support");
                            }
                        }
                        else
                        {
                            using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                            {
                                w.WriteLine("API/Clients/Get");
                                w.WriteLine("date:" + DateTime.Now);
                                w.WriteLine("User can not create");
                                w.WriteLine("----------------------------------------");
                            }
                            throw new HttpException(500, "Database error please contact support");
                        }
                    }


                    if (UserInDB == true)
                    {
                        var result = db.Database.SqlQuery<Client>("GetAllClients").ToList();
                        if (result != null)
                        {
                            if (result.Any())
                            {
                                foreach (var r in result)
                                {
                                    clients.Add(r);
                                }
                                foreach (var c in clients)
                                {
                                    
                                    SqlParameter[] userClientSQLParams =
                                {
                                    new SqlParameter("@MID", username),
                                    new SqlParameter("@ClientID", c.Id)
                                };
                                    var userClientResult = db.Database.SqlQuery<User>("GetUserClientMapping @MID, @ClientID", userClientSQLParams).ToList();
                                    if (userClientResult != null)
                                    {
                                        if (userClientResult.Any())
                                        {
                                            
                                            foreach (var u in userClientResult)
                                            {
                                                c.Users = new List<User>();
                                                User tmpUser = new User();
                                                tmpUser.Id = u.Id;
                                                tmpUser.MID = u.MID;
                                                c.Users.Add(tmpUser);
                                            }
                                        }
                                    }
                                }

                                List<Client> sortedClients = new List<Client>();
                                int i = 0;
                                int j = 0;
                                foreach (var c in clients)
                                {
                                    if (c.Users != null)
                                    {
                                        foreach (var u in c.Users)
                                        {
                                            if (u.MID == username)
                                            {
                                                sortedClients.Add(c);
                                                sortedClients[j].Users = new List<User>();
                                                sortedClients[j].Users.Add(u);
                                            }
                                            j++;
                                        }
                                    }
                                    i++;
                                }

                                return Ok(sortedClients);
                            }
                            else
                            {
                                return Ok(clients);
                            }
                        }
                        else
                        {
                            return Ok(clients);
                        }
                    }
                    else
                    {
                        using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                        {
                            w.WriteLine("API/Clients/Get");
                            w.WriteLine("date:" + DateTime.Now);
                            w.WriteLine("Error Creating User");
                            w.WriteLine("----------------------------------------");
                        }
                        throw new HttpException(500, "Database error please contact support");
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                {
                    w.WriteLine("API/Clients/Get");
                    w.WriteLine("date:" + DateTime.Now);
                    w.WriteLine(ex.Message);
                    w.WriteLine("----------------------------------------");
                }
                throw new HttpException(500, ex.Message);
            }
        }
           

        /// <summary>
        /// Attempts to update the client at the database from an external call
        /// </summary>
        /// <returns> client object with client id</returns>
        [HttpPut]
        public IHttpActionResult Put()
        {

            string clientServiceID = string.Empty, Name = string.Empty, Mission = string.Empty, Vision = string.Empty, MindtreeIndustGroup = string.Empty, BusinessUnit = string.Empty, YearsAgile = string.Empty, AdditionalNotes = string.Empty, DateJoined = string.Empty, MID = string.Empty, username = string.Empty;
            var httpRequest = HttpContext.Current.Request;
            username = Helper.GetUserName(httpRequest);
            if (string.IsNullOrEmpty(username))
            {
                using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                {
                    w.WriteLine("API/Clients/Put");
                    w.WriteLine("date:" + DateTime.Now);
                    w.WriteLine("Username is invalid");
                    w.WriteLine("----------------------------------------");
                }
                throw new HttpException(500, "NTLM ERROR");
            }


            bool UserInDB = false;

            // get the keys we passed and add them to the values we are looking for
            foreach (string key in httpRequest.Form.Keys)
            {
                var tmpKey = key;
                var tmpValue = httpRequest.Form[key];

                if (tmpKey == "ID")
                {
                    clientServiceID = tmpValue;
                }
                else if (tmpKey == "Name")
                {
                    Name = tmpValue;
                }
                else if (tmpKey == "Mission")
                {
                    Mission = tmpValue;
                }
                else if (tmpKey == "Vision")
                {
                    Vision = tmpValue;
                }
                else if (tmpKey == "MindtreeIndustGroup")
                {
                    MindtreeIndustGroup = tmpValue;
                }
                else if (tmpKey == "BusinessUnit")
                {
                    BusinessUnit = tmpValue;
                }
                else if (tmpKey == "YearsAgile")
                {
                    YearsAgile = tmpValue;
                }
                else if (tmpKey == "AdditionalNotes")
                {
                    AdditionalNotes = tmpValue;
                }
                else if (tmpKey == "DateJoined")
                {
                    DateJoined = tmpValue;
                }
                else if (tmpKey == "MID")
                {
                    MID = tmpValue;
                }
            }

            if (string.IsNullOrEmpty(MID))
            {
                MID = username; // MID is blank so lets go ahead and set the current authed user so they own it.
            }
                // TODO
            else
            {
                MID += "," + username + ",";
            }

            // convert the datetime
            DateTime dateJoined = Convert.ToDateTime(DateJoined);

            try
            {
                SqlParameter[] getUserSQLParams =
                    {
                        new SqlParameter("@MID", username)
                    };
                SqlParameter[] createUserSQLParams =
                    {
                        new SqlParameter("@MID", username)
                    };
                SqlParameter[] sqlParams =
                    {
                        new SqlParameter("@ID", clientServiceID),
                        new SqlParameter("@AdditionalNotes", AdditionalNotes),
                        new SqlParameter("@BusinessUnit", BusinessUnit),
                        new SqlParameter("@DateJoined",DateJoined),
                        new SqlParameter("@MindtreeIndustGroup",MindtreeIndustGroup),
                        new SqlParameter("@Mission",Mission),
                        new SqlParameter("@Name",Name),
                        new SqlParameter("@Vision",Vision),
                        new SqlParameter("@YearsAgile",YearsAgile),
                        new SqlParameter("@MID",MID),
                    };

                foreach (SqlParameter parameter in sqlParams)
                {
                    if (parameter.Value == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                }

                using (ApplicationDbContext db = new ApplicationDbContext())
                {

                    // try to get username
                    var UserExists = db.Database.SqlQuery<string>("GetUser @MID", getUserSQLParams).ToList();
                    if (UserExists != null)
                    {
                        if (UserExists.Any())
                        {
                            foreach (var u in UserExists)
                            {
                                if (!string.IsNullOrEmpty(u))
                                {
                                    // User Exists so we can continue
                                    UserInDB = true;
                                }
                                else
                                {
                                    UserInDB = false;
                                }
                            }
                        }
                        else
                        {
                            UserInDB = false;
                        }
                    }
                    else
                    {
                        UserInDB = false;
                    }
                    // If the user doesn't exist we will create user
                    if (UserInDB == false)
                    {
                        var CreateUser = db.Database.SqlQuery<string>("CreateUser @MID", createUserSQLParams).ToList();
                        if (CreateUser != null)
                        {
                            if (CreateUser.Any())
                            {
                                foreach (var u in CreateUser)
                                {
                                    if (!string.IsNullOrEmpty(u))
                                    {
                                        // User Exists so we can continue
                                        UserInDB = true;
                                    }
                                    else
                                    {
                                        using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                                        {
                                            w.WriteLine("API/Clients/Put");
                                            w.WriteLine("date:" + DateTime.Now);
                                            w.WriteLine("User can not create");
                                            w.WriteLine("----------------------------------------");
                                        }
                                        throw new HttpException(500, "Database error please contact support");
                                    }
                                }
                            }
                            else
                            {
                                using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                                {
                                    w.WriteLine("API/Clients/Put");
                                    w.WriteLine("date:" + DateTime.Now);
                                    w.WriteLine("User can not create");
                                    w.WriteLine("----------------------------------------");
                                }
                                throw new HttpException(500, "Database error please contact support");
                            }
                        }
                        else
                        {
                            using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                            {
                                w.WriteLine("API/Clients/Put");
                                w.WriteLine("date:" + DateTime.Now);
                                w.WriteLine("User can not create");
                                w.WriteLine("----------------------------------------");
                            }
                            throw new HttpException(500, "Database error please contact support");
                        }
                    }


                    if (UserInDB == true)
                    {
                        var result = db.Database.SqlQuery<int>("UpdateClient @ID, @AdditionalNotes, @BusinessUnit, @DateJoined, @MindtreeIndustGroup, @Mission, @Name, @Vision, @YearsAgile, @MID", sqlParams).ToList();
                        if (result != null)
                        {
                            if (result.Any())
                            {
                                Client tmpClient = new Client();
                                tmpClient.Id = result.FirstOrDefault();
                                SqlParameter[] userClientSQLParams =
                                {
                                    new SqlParameter("@MID", username),
                                    new SqlParameter("@ClientID", tmpClient.Id)
                                };
                                var userClientResult = db.Database.SqlQuery<User>("GetUserClientMapping @MID, @ClientID", userClientSQLParams).ToList();
                                if (userClientResult != null)
                                {
                                    if (userClientResult.Any())
                                    {
                                        User tmpUser = new User();
                                        tmpUser.Id = userClientResult.FirstOrDefault().Id;
                                        tmpUser.MID = userClientResult.FirstOrDefault().MID;
                                        tmpClient.Users = new List<User>();
                                        tmpClient.Users.Add(tmpUser);
                                    }
                                }
                                return Ok(tmpClient);
                            }
                            else
                            {
                                using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                                {
                                    w.WriteLine("API/Clients/Put");
                                    w.WriteLine("date:" + DateTime.Now);
                                    w.WriteLine("Empty Object");
                                    w.WriteLine("----------------------------------------");
                                }
                                throw new HttpException(500, "Database error please contact support");
                            }
                        }
                        else
                        {
                            using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                            {
                                w.WriteLine("API/Cleints/Put");
                                w.WriteLine("date:" + DateTime.Now);
                                w.WriteLine("Null Object");
                                w.WriteLine("----------------------------------------");
                            }
                            throw new HttpException(500, "Database error please contact support");
                        }
                    }
                    else
                    {
                        using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                        {
                            w.WriteLine("API/Clients/Put");
                            w.WriteLine("date:" + DateTime.Now);
                            w.WriteLine("Error Creating User");
                            w.WriteLine("----------------------------------------");
                        }
                        throw new HttpException(500, "Database error please contact support");
                    }
                }

            }
            catch (Exception ex)
            {
                using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                {
                    w.WriteLine("API/Cleints/Update");
                    w.WriteLine("date:" + DateTime.Now);
                    w.WriteLine(ex.Message);
                    w.WriteLine("----------------------------------------");
                }
                throw new HttpException(500, ex.Message);
            }
        }
    }
}
