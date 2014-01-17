using Web.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Web.Models.Engage
{
    public class Model
    {
        //public EngageIndex()
        //{
        //    this.SparkUser = new BLL.SparkUser();
        //}

        public Model(BLL.SparkUser sparkUser) : base()
        {
            this.SparkUser = sparkUser;
        }

        private List<Campaign> _campaigns;
        public List<Campaign> Campaigns
        {
            get
            {
                if (_campaigns == null)
                {
                    _campaigns = engageCampaigns(this.SparkUser);
                }
                return _campaigns;
            }
        }

        public Guid DefaultCampaignID
        {
            get
            {
                var c = this.Campaigns.LastOrDefault();
                if (c != null)
                {
                    return c.engageCampaignID;
                }
                return new Guid("91cc4a84-1074-3d22-8fbf-d7e8c5927ea4");
            }
        }

        private List<Result> _results;
        public List<Result> Results
        {
            get
            {
                if (_results == null)
                {
                    _results = Result.GetResults(this.SparkUser);
                }
                return _results;
            }
        }
        public BLL.SparkUser SparkUser { get; set; }

        #region Functions
        public static List<Campaign> engageCampaigns(BLL.SparkUser sparkUser)
        {
            SqlParameter[] p = new SqlParameter[] 
            {
                new SqlParameter("@SparkUserID", sparkUser.SparkUserID)
            };
            return SqlHelper.FillEntities<Campaign>("SelectengageCampaigns", p, CommandType.StoredProcedure);
        }

        #endregion

    }
}