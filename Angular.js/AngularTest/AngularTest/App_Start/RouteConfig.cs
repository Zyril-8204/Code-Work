using System.Web.Mvc;
using System.Web.Routing;

namespace IdentitySample
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "routeOne",
                url: "routesTest/One",
                defaults: new { controller = "RoutesTest", action = "One" });

            routes.MapRoute(
                name: "routeTwo",
                url: "routesTest/Two/{id}",
                defaults: new { controller = "RoutesTest", action = "Two", id = UrlParameter.Optional });
            
            routes.MapRoute(
                name: "routeThree",
                url: "routesTest/Three",
                defaults: new { controller = "RoutesTest", action = "Three" });
            
            routes.MapRoute(
                name: "login",
                url: "Accounts/Login",
                defaults: new { controller = "Account", action = "Login" });

            routes.MapRoute(
                name: "register",
                url: "Accounts/Register",
                defaults: new { controller = "Account", action = "Register" });

            routes.MapRoute(
                name: "routeFour",
                url: "routesTest/Four",
                defaults: new { controller = "RoutesTest", action = "Four" });

            // a catch all to send bad url request to the home page. (could be custom 404 if we wanted but this is easier.)
            routes.MapRoute(
                name: "Default",
                url: "{*url}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}