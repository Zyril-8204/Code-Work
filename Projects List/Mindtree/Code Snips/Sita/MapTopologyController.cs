using System.Web.Mvc;
using SITA.IMMS.Entities;
using SITA.IMMS.UI.Process;
using SITA.IMMS.UI.Web.Controllers.Shared;

namespace SITA.IMMS.UI.Web.Controllers
{
    [Authorize]
    public class MapTopologyController : BaseController
    {
        //
        // GET: /Topology/
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Index()
        {
            if (Session["UserSession"] == null)
                return RedirectToAction("Logoff", "Login");

            LoginEntity userEntity = new LoginEntity();
            userEntity = (LoginEntity)Session["UserSession"];
            Site sitetopology = GetTopology(userEntity);
            Session["TopologyData"] = sitetopology;
            return View();
        }

        public Site GetTopology(LoginEntity userEntity)
        {
            TopologyProcessComponent tpcTopology = new TopologyProcessComponent();
            return tpcTopology.GetTopology(userEntity);
        }
    }

}
