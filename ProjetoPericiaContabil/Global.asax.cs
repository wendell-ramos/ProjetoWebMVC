using ProjetoPericiaContabil.Helpers;
using ProjetoPericiaContabil.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

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
                string senhaAdmin = CriptoHelper.HashSHA256("123456");

                var admin = db.Usuarios.FirstOrDefault(u => u.Email == "admin@admin.com");

                if (admin == null)
                {
                    db.Usuarios.Add(new Usuario
                    {
                        Nome = "Admin",
                        Email = "admin@admin.com",
                        Senha = senhaAdmin,
                        Tipo = "Admin",
                        Cargo = null
                    });

                    db.SaveChanges();
                }
                else
                {
                    admin.Nome = "Admin";
                    admin.Senha = senhaAdmin;
                    admin.Tipo = "Admin";
                    admin.Cargo = null;

                    db.SaveChanges();
                }
            }
        }
    }
}