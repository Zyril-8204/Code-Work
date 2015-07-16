using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using theEducators.Models;
using theEducators.Helpers;
using System.Text;
using System.Configuration;

namespace theEducators.Controllers
{
    public class ArticleController : Controller
    {
        //
        // GET: /Article/Index
        [Authorize]  //Clients can't see non-Client Articles
        public ActionResult Index()
        {
            User CurrentUser = Helpers.Cookie.GetUserInfoWithOutPassword();
            if (CurrentUser == null)
                return RedirectToAction("Login", "Login");
            else
                ViewBag.User = CurrentUser;


            ViewBag.Number = 1;
            ViewBag.ApprovedArticleList = GetAllApprovedArticles();

            if (CurrentUser.Role == "Admin" || CurrentUser.Role == "Course Director")
            {
                ViewBag.ArticleWaitingForApprovalList = GetAllArticlesWaitingForApproval();
            }
            else
            {
                ViewBag.ArticleWaitingForApprovalList = new LinkedList<ArticlesWaitingForApporval>();
            }
            return View();
        }

        public static Article GetApprovalArticleByID(int ID)
        {
            Article result = new Article();
            Innovation_Pilot_Educators_Context db = new Innovation_Pilot_Educators_Context();

            return (from articles in db.ArticlesWaitingForApporval where articles.Id == ID select articles).First();
        }

        //
        // GET: /Article/WaitingForApproval
        [Authorize(Roles = "Admin, Course Director")]
        public ActionResult WaitingForApproval()
        {
            User CurrentUser = Helpers.Cookie.GetUserInfoWithOutPassword();
            if (CurrentUser == null)
                return RedirectToAction("Login", "Login");
            else
                ViewBag.User = CurrentUser;
            ViewBag.Number = 1;

            IList<ArticlesWaitingForApporval> ApprovedArticleList = GetAllArticlesWaitingForApproval();
            return View(ApprovedArticleList);
        }

        //
        // GET: /Article/Create
        [Authorize] //must be logged in as some kind of user to create articles at all
        public ActionResult Create()
        {
            User CurrentUser = Helpers.Cookie.GetUserInfoWithOutPassword();
            if (CurrentUser == null)
                return RedirectToAction("Login", "Login");
            else
                ViewBag.User = CurrentUser;
            var tags = TagController.GetAllTags();
            List<string> tagNames = new List<string>();
            foreach (var item in tags)
            {
                tagNames.Add(item.Name);
            }
            ViewData["Tags"] = tagNames;
            return View();
        }

        //
        // POST: /Article/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Article newArticle)
        {
            User CurrentUser = Helpers.Cookie.GetUserInfoWithOutPassword();
            if (CurrentUser == null)
                return RedirectToAction("Login", "Login");
            else
                ViewBag.User = CurrentUser;

            try
            {
                if (ModelState.IsValid)
                {
                    SqlParameter[] sqlParams =
                {
                    new SqlParameter("@IsClientArticle",  newArticle.IsClientArticle),
                    new SqlParameter("@Title",newArticle.Title),
                    new SqlParameter("@Summary",newArticle.Summary),
                    new SqlParameter("@Content",newArticle.Content),
                    new SqlParameter("@CreatedByName",CurrentUser.Name),
                    new SqlParameter("@LastEditedByName",CurrentUser.Name),
                    new SqlParameter("@CreatedByID",CurrentUser.Id),
                    new SqlParameter("@LastEditedByID",CurrentUser.Id),
                    new SqlParameter("@Tag",newArticle.Tag),
                    new SqlParameter("@OriginalArticleId", DBNull.Value)
                };

                    using (Innovation_Pilot_Educators_Context db = new Innovation_Pilot_Educators_Context())
                    {
                        var result = db.Database.ExecuteSqlCommand("CreateArticleWaitingForApproval @IsClientArticle, @Title, @Summary, @Content, @CreatedByName, @LastEditedByName, @CreatedByID, @LastEditedByID, @Tag, @OriginalArticleId", sqlParams);
                    }

                }
                else
                    return View(newArticle);

                if (newArticle.IsClientArticle)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Training");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.ToString());
                return View(newArticle);
            }
        }

        //
        // GET: /Article/View/5
        //No Authorize tab b/c the rules for viewing are a bit more complicated
        //so it's handled manually inside the the function manually
        public ActionResult View(int ID)
        {
            Article article = new Article();
            article = GetArticleByID(ID);

            User CurrentUser = Helpers.Cookie.GetUserInfoWithOutPassword();
            if (CurrentUser == null && !article.IsClientArticle)
                return RedirectToAction("Login", "Login");

            ViewBag.User = CurrentUser;
            return View(article);
        }

        //
        // GET: /Article/Edit/5
        [Authorize] //clients can't edit
        public ActionResult Edit(int id)
        {
            User CurrentUser = Helpers.Cookie.GetUserInfoWithOutPassword();
            if (CurrentUser == null)
                return RedirectToAction("Login", "Login");
            else
                ViewBag.User = CurrentUser;

            var tags = TagController.GetAllTags();
            List<string> tagNames = new List<string>();
            foreach (var item in tags)
            {
                tagNames.Add(item.Name);
            }
            ViewBag.Tags = tagNames;

            try
            {

                Article article = GetArticleByID(id);

                if (article == null)
                {
                    return HttpNotFound();
                }
                //else
                return View(article);
            }
            catch (Exception e)
            {

                ModelState.AddModelError("", e.ToString());
                return View(GetArticleByID(id));
            }
        }

        //
        // POST: /Article/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Article editedArticle)
        {
            User CurrentUser = Helpers.Cookie.GetUserInfoWithOutPassword();
            if (CurrentUser == null)
                return RedirectToAction("Login", "Login");
            else
                ViewBag.User = CurrentUser;

            try
            {
                if (ModelState.IsValid) //connected to validation
                {
                    using (Innovation_Pilot_Educators_Context db = new Innovation_Pilot_Educators_Context())
                    {
                        ArticlesWaitingForApporval newWaitingArticle = editedArticle.CreateArticleWaitingForApproval();

                        SqlParameter[] sqlParams =
                    {
                        new SqlParameter("@IsClientArticle",  newWaitingArticle.IsClientArticle),
                        new SqlParameter("@Title",newWaitingArticle.Title),
                        new SqlParameter("@Summary",newWaitingArticle.Summary),
                        new SqlParameter("@Content",newWaitingArticle.Content),
                        new SqlParameter("@CreatedByName",newWaitingArticle.CreatedByName),
                        new SqlParameter("@LastEditedByName",newWaitingArticle.LastEditedByName),
                        new SqlParameter("@CreatedByID",newWaitingArticle.CreatedByID),
                        new SqlParameter("@LastEditedByID",newWaitingArticle.LastEditedByID),
                        new SqlParameter("@Tag",newWaitingArticle.Tag),
                        new SqlParameter("@OriginalArticleId", newWaitingArticle.OriginalArticleId)
                    };

                        var result = db.Database.ExecuteSqlCommand("CreateArticleWaitingForApproval @IsClientArticle, @Title, @Summary, @Content, @CreatedByName, @LastEditedByName, @CreatedByID, @LastEditedByID, @Tag, @OriginalArticleId", sqlParams);
                    }
                }

                if (editedArticle.IsClientArticle)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Training");
                }
            }
            catch (Exception e)
            {
                var tags = TagController.GetAllTags();
                List<string> tagNames = new List<string>();
                foreach (var item in tags)
                {
                    tagNames.Add(item.Name);
                }
                ViewBag.Tags = tagNames;


                ModelState.AddModelError("", e.ToString());
                return View(editedArticle);
            }
        }

        //
        // GET: /Article/Delete/5
        [Authorize(Roles = "Admin, Course Director")]
        public ActionResult Delete(int id)
        {
            User CurrentUser = Helpers.Cookie.GetUserInfoWithOutPassword();
            if (CurrentUser == null)
                return RedirectToAction("Login", "Login");
            else
                ViewBag.User = CurrentUser;

            try
            {
                Article ArticleToDelete = GetArticleByID(id);
                if (ArticleToDelete == null)
                {
                    return HttpNotFound();
                }
                //else
                return View(ArticleToDelete);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.ToString());
                return View(GetArticleByID(id));
            }
        }

        //
        // POST: /Article/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User CurrentUser = Helpers.Cookie.GetUserInfoWithOutPassword();
            if (CurrentUser == null)
                return RedirectToAction("Login", "Login");
            else
                ViewBag.User = CurrentUser;

            try
            {
                if (ModelState.IsValid)
                {
                    using (Innovation_Pilot_Educators_Context db = new Innovation_Pilot_Educators_Context())
                    {
                        db.Database.ExecuteSqlCommand(string.Format("DELETE FROM Articles WHERE Id = '{0}'", id));
                    }
                    return RedirectToAction("Index");
                }
                //else
                return View(GetArticleByID(id));
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.ToString());
                return View(GetArticleByID(id));
            }
        }

        [HttpGet] //lets Admin / Course Director view waiting article 
        public ActionResult ApproveArticle(int ID) //so they can approve/edit/reject it
        {
            Article article = new Article();
            article = GetApprovalArticleByID(ID);

            User CurrentUser = Helpers.Cookie.GetUserInfoWithOutPassword();
            if (CurrentUser == null && !article.IsClientArticle)
                return RedirectToAction("Login", "Login");

            ViewBag.User = CurrentUser;
            return View(article);
        }

        //
        // GET: /Article/Approve/5           //really POST now?
        [Authorize(Roles = "Admin, Course Director")]
        public ActionResult Approve(int id)
        {
            User CurrentUser = Helpers.Cookie.GetUserInfoWithOutPassword();
            if (CurrentUser == null)
                return RedirectToAction("Login", "Login");
            else
                ViewBag.User = CurrentUser;

            try
            {

                if (ModelState.IsValid)
                {
                    using (Innovation_Pilot_Educators_Context db = new Innovation_Pilot_Educators_Context())
                    {

                        //This version gets the correct values for OriginalArticleId & ArticleWaitingForApprovalId, but no articles are copied / moved in the db at all - maddening!!!!
                            var Result = db.Database.SqlQuery<int?>("SELECT OriginalArticleId FROM ArticlesWaitingForApporval WHERE Id = " +id.ToString()).ToList().FirstOrDefault();
                        

                        SqlParameter[] sqlParams = new SqlParameter[2];
                        sqlParams[0] = new SqlParameter("@ArticleWaitingForApprovalId", id);

                        if (Result == null)
                        {
                            sqlParams[1] = new SqlParameter("@OriginalArticleId", DBNull.Value);
                        }
                        else
                        {
                            sqlParams[1] = new SqlParameter("@OriginalArticleId", Result);
                        }

                        db.Database.ExecuteSqlCommand("EXEC ApproveArticle @ArticleWaitingForApprovalId, @OriginalArticleId", sqlParams);

                    }

                }



                /*  //not sure why this did not work
                 *  //the open connection always failed....
                int originalArticleId = -5;
                string connectionString = ConfigurationManager.ConnectionStrings["Innovation_Pilot_Educators_Context"].ConnectionString;

                SqlConnection con = new SqlConnection(connectionString);
                
                    SqlCommand cmd1 = new SqlCommand(string.Format("SELECT OriginalArticleId FROM ArticlesWaitingForApporval WHERE Id = '{0}'", id), con);

                    con.Open();
                    originalArticleId = (Int32)cmd1.ExecuteScalar();

                    SqlParameter[] sqlParams = new SqlParameter[2];
                    sqlParams[0] = new SqlParameter("@ArticleWaitingForApprovalId", id);
                    if (originalArticleId < 1)
                    {
                        sqlParams[1] = new SqlParameter("@OriginalArticleId", DBNull.Value);
                    }
                    else
                    {
                        sqlParams[1] = new SqlParameter("@OriginalArticleId", originalArticleId);
                    }

                    SqlCommand cmd2 = new SqlCommand(string.Format("EXEC ApproveArticle @ArticleWaitingForApprovalId ='{0}', @OriginalArticleId = '{1}'", sqlParams[0], sqlParams[1]), con);
                    cmd2.ExecuteNonQuery();
                    con.Close();
                 * */




                //ALWAYS returns -1!!!!!!!!!!!!!
                //  int originalArticleId = db.ExecuteSqlCommand(string.Format("SELECT OriginalArticleId FROM ArticlesWaitingForApproval WHERE Id = '{0}'", id));









                /* //original version always came back null, probably b/c the edmx does not 
                 * //recoginize the OriginalArticleId column
                 * //this meant the ApproveArticle stored procedure would copy the edited 
                 * //article to the main Article table, but not delted the old copy in the 
                 * //Article table
                 
                             int? originalArticleId = db.ArticlesWaitingForApporval.Find(id).OriginalArticleId;

            //this test is necessary b/c if you try to put 0 or null directly into a new SqlParameter(string, object) constructor, it parses the string incorrectly, 'as OriginalAritcleId = ' and then nothing after it
            if (originalArticleId == null || originalArticleId < 1)
            {
              SqlParameter[] sqlParams =
              {
                  new SqlParameter("@ArticleWaitingForApprovalId", id),
                  new SqlParameter("@OriginalArticleId", DBNull.Value) //DBNull.Value makes it work
              };

              var result = db.Database.ExecuteSqlCommand("ApproveArticle @ArticleWaitingForApprovalId, @OriginalArticleId", sqlParams);
            }
            else
            {
              SqlParameter[] sqlParams =
              {
                  new SqlParameter("@ArticleWaitingForApprovalId", id),
                  new SqlParameter("@OriginalArticleId", originalArticleId)
              };

              var result = db.Database.ExecuteSqlCommand("ApproveArticle @ArticleWaitingForApprovalId, @OriginalArticleId", sqlParams);
            }
                 
                 * 
                 * */


                return RedirectToAction("WaitingForApproval");
            }
            catch (SqlException e)
            {
                ModelState.AddModelError("", e.ToString());
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.ToString());
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /Article/Reject/5
        [Authorize(Roles = "Admin, Course Director")]
        public ActionResult Reject(int id)
        {
            User CurrentUser = Helpers.Cookie.GetUserInfoWithOutPassword();
            if (CurrentUser == null)
                return RedirectToAction("Login", "Login");
            else
                ViewBag.User = CurrentUser;

            //could possibly send message for why it was rejected later on

            using (Innovation_Pilot_Educators_Context db = new Innovation_Pilot_Educators_Context())
            {
                db.Database.ExecuteSqlCommand(string.Format("DELETE FROM ArticlesWaitingForApporval WHERE Id = '{0}';", id));
            }

            return RedirectToAction("Index");
        }

        //
        // GET: /Article/TitleSearch/
        [HttpGet] //should be default, but just to be safe
        public ActionResult TitleSearch()
        {
            User CurrentUser = Helpers.Cookie.GetUserInfoWithOutPassword();
            if (CurrentUser == null)
                return RedirectToAction("Login", "Login");
            else
                ViewBag.User = CurrentUser;

            TitleSearchViewModel CurrentSearch = new TitleSearchViewModel();
            try
            {
                return View(CurrentSearch);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.ToString());
                return View(CurrentSearch);
            }
        }

        //
        // POST: /Article/TitleSearch/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TitleSearch(TitleSearchViewModel request, bool showArticleWaitingForApproval = false)
        {
            User CurrentUser = Helpers.Cookie.GetUserInfoWithOutPassword();
            if (CurrentUser == null)
                return RedirectToAction("Login", "Login");
            else
                ViewBag.User = CurrentUser;

            try
            {
                if (ModelState.IsValid) //connected to validation
                {
                    if (request.IsEmpty()) //catch block will add this error message to the model state 
                        throw new Exception("No Search Criteria entered"); //and redisplay it for the user


                    //instantiating the lists first & then adding to them ensrues that 
                    //even if there are no results you wont get a null reference 
                    //exception in the view
                    IEnumerable<Article> ApprovedArticleList = new List<Article>();
                    ApprovedArticleList = FindMatchingApprovedArticles(request.GetTitleyWordList(), includeTags: false);
                    ViewBag.ApprovedArticleList = ApprovedArticleList;

                    IEnumerable<ArticlesWaitingForApporval> ArticleWaitingForApprovalList = new List<ArticlesWaitingForApporval>();
                    if (showArticleWaitingForApproval)
                    {
                        if (CurrentUser.Role == "Admin" || CurrentUser.Role == "Course Director")
                        {
                            ArticleWaitingForApprovalList = FindMatchingArticlesWaitingForApproval(request.GetTitleyWordList());
                        }
                    }
                    ViewBag.ArticleWaitingForApprovalList = ArticleWaitingForApprovalList;
                    ViewBag.Number = 1;
                    return View("Index");
                }
                else
                {
                    return View(request);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.ToString());
                return View(request);
            }
        }

        //
        // GET: /Article/Search/
        [HttpGet] //should be default, but just to be safe
        public ActionResult Search()
        {
            User CurrentUser = Helpers.Cookie.GetUserInfoWithOutPassword();
            if (CurrentUser == null)
                return RedirectToAction("Login", "Login");
            else
                ViewBag.User = CurrentUser;

            SearchViewModel CurrentSearch = new SearchViewModel();
            try
            {
                CurrentSearch.Tags = GetTagSelectList();
                return View(CurrentSearch);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.ToString());
                return View(CurrentSearch);
            }
        }

        //
        // POST: /Article/Search/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(SearchViewModel request)
        {
            User CurrentUser = Helpers.Cookie.GetUserInfoWithOutPassword();
            if (CurrentUser == null)
                return RedirectToAction("Login", "Login");
            else
                ViewBag.User = CurrentUser;

            try
            {
                if (ModelState.IsValid) //connected to validation
                {
                    if (request.IsEmpty()) //catch block will add this error message to the model state 
                        throw new Exception("No Search Criteria entered"); //and redisplay it for the user


                    //instantiating the lists first & then adding to them ensrues that 
                    //even if there are no results you wont get a null reference 
                    //exception in the view
                    IEnumerable<Article> ApprovedArticleList = new List<Article>();
                    ApprovedArticleList = FindMatchingApprovedArticles(request.GetCombinedKeyWordList());


                    IEnumerable<ArticlesWaitingForApporval> ArticleWaitingForApprovalList = new List<ArticlesWaitingForApporval>();
                    ViewBag.ApprovedArticleList = GetAllApprovedArticles();

                    if (CurrentUser.Role == "Admin" || CurrentUser.Role == "Course Director")
                    {
                        ArticleWaitingForApprovalList = FindMatchingArticlesWaitingForApproval(request.GetCombinedKeyWordList());
                    }

                    ViewBag.ArticleWaitingForApprovalList = ArticleWaitingForApprovalList;

                    ViewBag.Number = 1;

                    return View("Index");
                }
                else
                {
                    return View(request);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.ToString());
                return View(request);
            }
        }

        public static IEnumerable<Article> FindMatchingApprovedArticles(IList<string> keyWordsLst, bool includeTags = true)
        {
            IEnumerable<Article> SearchResult = new List<Article>();
            using (Innovation_Pilot_Educators_Context db = new Innovation_Pilot_Educators_Context())
            {
                if (includeTags)
                {
                    var query = from article in db.Articles
                                from word in keyWordsLst
                                where article.Title.ToLower().Contains(word) || article.Tag.ToLower().Contains(word)
                                select article;

                    SearchResult = query.ToList();
                }
                else
                {
                    var query = from article in db.Articles
                                from word in keyWordsLst
                                where article.Title.ToLower().Contains(word)
                                select article;

                    SearchResult = query.ToList();
                }
            }
            return SearchResult;
        }

        public static IEnumerable<ArticlesWaitingForApporval> FindMatchingArticlesWaitingForApproval(IList<string> keyWordsLst, bool includeTags = true)
        {
            IEnumerable<ArticlesWaitingForApporval> SearchResult = new List<ArticlesWaitingForApporval>();
            using (Innovation_Pilot_Educators_Context db = new Innovation_Pilot_Educators_Context())
                if (includeTags)
                {
                    var query = from article in db.ArticlesWaitingForApporval
                                from word in keyWordsLst
                                where article.Title.ToLower().Contains(word) || article.Tag.ToLower().Contains(word)
                                select article;

                    SearchResult = query.ToList();
                }
                else
                {
                    var query = from article in db.ArticlesWaitingForApporval
                                from word in keyWordsLst
                                where article.Title.ToLower().Contains(word)
                                select article;

                    SearchResult = query.ToList();
                }

            return SearchResult;
        }

        public static string[] GetTagArray()
        {
            using (Innovation_Pilot_Educators_Context db = new Innovation_Pilot_Educators_Context())
            {
                var query = from tag in db.Tags select tag.Name;
                query = query.Distinct(); //***have to look out for duplicate tag names****
                return query.ToArray();
            }
        }

        public static IList<SelectListItem> GetTagSelectList()
        {
            List<SelectListItem> TagSelectList = new List<SelectListItem>();
            List<string> TagList;
            using (Innovation_Pilot_Educators_Context db = new Innovation_Pilot_Educators_Context())
            {
                var query = from tag in db.Tags select tag.Name;
                query = query.Distinct(); //***have to look out for duplicate tag names****
                TagList = query.ToList();
            }

            TagSelectList.Add(new SelectListItem { Text = "", Value = "", Selected = true });

            foreach (string tag in TagList)
            {
                SelectListItem CurrentSelectListItem = new SelectListItem()
                {
                    Text = tag,
                    Value = tag,
                    Selected = false
                };
                TagSelectList.Add(CurrentSelectListItem);
            }
            return TagSelectList;
        }

        public static IList<Article> GetAllApprovedArticles()
        {
            using (Innovation_Pilot_Educators_Context db = new Innovation_Pilot_Educators_Context())
            {
                return db.Articles.ToList();
            }
        }

        public static IList<ArticlesWaitingForApporval> GetAllArticlesWaitingForApproval()
        {
            using (Innovation_Pilot_Educators_Context db = new Innovation_Pilot_Educators_Context())
            {
                return db.ArticlesWaitingForApporval.ToList();
            }
        }

        public static IList<Article> GetTrainingArticlesByTag(string tag)
        {
            IList<Article> resultList = new List<Article>();
            Innovation_Pilot_Educators_Context db = new Innovation_Pilot_Educators_Context();
            var dataResult = from articles in db.Articles where articles.IsClientArticle == false select articles;

            resultList = dataResult.Where(Dr => Dr.Tag.Contains(tag)).ToList();
            return resultList;
        }

        public static IList<Article> GetAllTrainingArticles()
        {
            IList<Article> resultList = new List<Article>();
            Innovation_Pilot_Educators_Context db = new Innovation_Pilot_Educators_Context();
            var dataResult = from articles in db.Articles where articles.IsClientArticle == false select articles;

            resultList = dataResult.ToList();
            return resultList;
        }

        public static IList<Article> GetClientArticlesByTag(string tag)
        {
            IList<Article> resultList = new List<Article>();
            Innovation_Pilot_Educators_Context db = new Innovation_Pilot_Educators_Context();
            var dataResult = from articles in db.Articles where articles.IsClientArticle == true select articles;

            resultList = dataResult.Where(Dr => Dr.Tag.Contains(tag)).ToList();
            return resultList;
        }

        public static IList<Article> GetAllClientArticles()
        {
            IList<Article> resultList = new List<Article>();
            Innovation_Pilot_Educators_Context db = new Innovation_Pilot_Educators_Context();
            var dataResult = from articles in db.Articles where articles.IsClientArticle == true select articles;

            resultList = dataResult.ToList();
            return resultList;
        }

        public static Article GetArticleByID(int ID)
        {
            Innovation_Pilot_Educators_Context db = new Innovation_Pilot_Educators_Context();
            Article article;
            try
            {
                article = (from articles in db.Articles where articles.Id == ID select articles).First();
            }
            catch
            {
                try
                {
                    article = (from articles in db.ArticlesWaitingForApporval where articles.Id == ID select articles).First();
                }
                catch
                {
                    article = null;
                }
            }
            return article;

            //returning ..... .First() is automatically null if not found
            //and copying property by proptery is problamatic if we add/subratct properties
            //if (dataResult != null)
            //{
            //    result.Content = dataResult.Content;
            //    result.CreatedByID = dataResult.CreatedByID;
            //    result.CreatedByName = dataResult.CreatedByName;
            //    result.CreatedOn = dataResult.CreatedOn;
            //    result.Id = dataResult.Id;
            //    result.IsClientArticle = dataResult.IsClientArticle;
            //    result.LastEditedByID = dataResult.LastEditedByID;
            //    result.LastEditedByName = dataResult.LastEditedByName;
            //    result.LastEditedOn = dataResult.LastEditedOn;
            //    result.Summary = dataResult.Summary;
            //    result.Tag = dataResult.Tag;
            //    result.Title = dataResult.Title;
            //    result.UserId = dataResult.UserId;
            //    result.IsApproved = dataResult.IsApproved;
            //    return result;
            //}
            //else return null;
        }

        public static IList<String> GetLastThreeTags()
        {
            List<string> results = new List<string>();
            Innovation_Pilot_Educators_Context db = new Innovation_Pilot_Educators_Context();
            User user = Helpers.Cookie.GetUserInfoWithOutPassword();
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@UserID", user.Id),
            };

            var mostUsedTags = db.Database.ExecuteSqlCommand("GetTop3Tags @UserID", sqlParams);
            return results;
        }

        public static IList<Article> GetRecommendedTrainingArticles()
        {
            IList<Article> resultList = new List<Article>();
            IList<string> strings = GetLastThreeTags();

            Innovation_Pilot_Educators_Context db = new Innovation_Pilot_Educators_Context();
            if (strings.Any())
            {// get all the training articles
                var dataResult = from articles in db.Articles where articles.IsClientArticle == false select articles;
                // sort through each training article looking for articles that match the top three searched tags.
                foreach (var s in strings)
                {
                    foreach (var r in dataResult)
                    {
                        if (r.Tag == s)
                        {
                            resultList.Add(r);
                        }
                    }
                }
            }
            else
            {
                // get all the training articles
                // var dataResult = from articles in db.Articles where articles.IsClientArticle == false select articles;
                // resultList = dataResult.Where(Dr => Dr.Tag == user.tag).ToList();
            }

            return resultList;
        }
    }
}
