using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using PulseBackend.DAL;
using PulseBackend.Models;

namespace PulseBackend.API
{
    public class SessionsController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post()
        {
            string ClientId = string.Empty, Name = string.Empty, dateTaken = string.Empty, username = string.Empty;
            var httpRequest = HttpContext.Current.Request;
            username = Helper.GetUserName(httpRequest);
            if (string.IsNullOrEmpty(username))
            {
                using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                {
                    w.WriteLine("API/Sessions/Post");
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

                if(tmpKey == "ClientID")
                {
                    ClientId = tmpValue;
                }

                if(tmpKey == "Name")
                {
                    Name = tmpValue;
                }

                if(tmpKey == "DateTaken")
                {
                    dateTaken = tmpValue;
                }
            }

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
                    new SqlParameter("@ClientID", ClientId),
                    new SqlParameter("@DateTaken", dateTaken),
                    new SqlParameter("@Name", Name),
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
                                            w.WriteLine("API/Sessions/Post");
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
                                    w.WriteLine("API/Sessions/Post");
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
                                w.WriteLine("API/Sessions/Post");
                                w.WriteLine("date:" + DateTime.Now);
                                w.WriteLine("User can not create");
                                w.WriteLine("----------------------------------------");
                            }
                            throw new HttpException(500, "Database error please contact support");
                        }
                    }


                    if (UserInDB == true)
                    {
                        var result = db.Database.SqlQuery<int>("CreateSession @ClientID, @DateTaken, @Name", sqlParams).ToList();
                        if (result != null)
                        {
                            if (result.Any())
                            {
                                Session tmpAnswer = new Session();
                                tmpAnswer.Id = result.FirstOrDefault();
                                return Ok(tmpAnswer);
                            }
                            else
                            {
                                using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                                {
                                    w.WriteLine("API/Sessions/Post");
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
                                w.WriteLine("API/Sessions/Post");
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
                            w.WriteLine("API/Sessions/Post");
                            w.WriteLine("date:" + DateTime.Now);
                            w.WriteLine("User can not create");
                            w.WriteLine("----------------------------------------");
                        }
                        throw new HttpException(500, "Database error please contact support");
                    }
                }
            }
            catch(Exception ex)
            {
                using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                {
                    w.WriteLine("API/Sessions/Create");
                    w.WriteLine("date:" + DateTime.Now);
                    w.WriteLine(ex.Message);
                    w.WriteLine("----------------------------------------");
                }
                throw new HttpException(500, "Database error please contact support");
            }
        }

        [HttpGet]
        public IHttpActionResult Get(string ID)
        {
            string ClientID = string.Empty, username = string.Empty;
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
            List<Session> session = new List<Session>();


            if (!string.IsNullOrEmpty(ID))
            {
                ClientID = ID;
            }
            else
            {
                return NotFound();
            }
           
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
                   new SqlParameter("@ClientID", ClientID),
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
                                           w.WriteLine("API/Sessions/Get");
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
                                   w.WriteLine("API/Sessions/Get");
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
                               w.WriteLine("API/Sessions/Get");
                               w.WriteLine("date:" + DateTime.Now);
                               w.WriteLine("User can not create");
                               w.WriteLine("----------------------------------------");
                           }
                           throw new HttpException(500, "Database error please contact support");
                       }
                   }


                   if (UserInDB == true)
                   {
                       var result = db.Database.SqlQuery<Session>("GetSession @ClientID", sqlParams).ToList();
                       if (result != null)
                       {
                           if (result.Any())
                           {
                               foreach (var r in result)
                               {
                                   session.Add(r);
                               }
                           }
                           else
                           {
                               return Ok(session);
                           }
                       }
                       else
                       {
                           return Ok(session);
                       }
                       return Ok(session);
                   }
                   else
                   {
                       using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                       {
                           w.WriteLine("API/Sessions/Get");
                           w.WriteLine("date:" + DateTime.Now);
                           w.WriteLine("User can not create");
                           w.WriteLine("----------------------------------------");
                       }
                       throw new HttpException(500, "Database error please contact support");
                   }
               }
            }
            catch(Exception ex)
            {
                using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(@"\Logs\ErrorLog.txt")))
                {
                    w.WriteLine("API/Sessions/Read");
                    w.WriteLine("date:" + DateTime.Now);
                    w.WriteLine(ex.Message);
                    w.WriteLine("----------------------------------------");
                }
                throw new HttpException(500, "Database error please contact support");
            }
        }
    }

}
