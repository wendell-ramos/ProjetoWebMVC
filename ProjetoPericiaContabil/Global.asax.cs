using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ProjetoPericiaContabil.Models;
using System.Linq;

namespace ProjetoPericiaContabil
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            using (var db = new ApplicationDbContext())
            {
                if (!db.Usuarios.Any(u => u.Tipo == "Admin"))
                {
                    db.Usuarios.Add(new Usuario
                    {
                        Nome = "Admin",
                        Email = "admin@admin.com",
                        Senha = "123",
                        Tipo = "Admin"
                    });

                    db.SaveChanges();
                }
            }

        }
    }
}
