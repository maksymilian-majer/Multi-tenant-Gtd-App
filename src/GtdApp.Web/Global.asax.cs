namespace GtdApp.Web
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Autofac;
    using Autofac.Integration.Mvc;
    using Autofac.Integration.Web;

    using GtdApp.Entities;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static ContainerProvider _containterProvider;

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            var container = ConfigureAutofacContainer();
            _containterProvider = new ContainerProvider(container);

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        private static IContainer ConfigureAutofacContainer()
        {
            var builder = new ContainerBuilder();

            var connectionString = ConfigurationManager.ConnectionStrings["tenantRepositoryConnection"].ConnectionString;

            builder.Register(
                (c, p) => 
                    new TenantRepository(connectionString))
                    .SingleInstance();
            
            builder.Register(
                (c, p) =>
                new GtdAppDataContext(
                    c.Resolve<TenantRepository>().GetTenantIdBySubdomain(GetSubdomain())))
                .InstancePerLifetimeScope();

            builder.RegisterType<RazorViewEngine>().As<IViewEngine>();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // adds injections for the HTTP abstractions, ie.
            // HttpContextBase
            // HttpRequestBase
            // HttpResponseBase
            // HttpServerUtilityBase
            // HttpSessionStateBase
            builder.RegisterModule(new AutofacWebTypesModule());

            return builder.Build();
        }

        private static string GetSubdomain()
        {
            var host = HttpContext.Current.Request.Headers["host"] /*.Url.Host*/;
            var domainParts = host.Split('.');

#if DEBUG
            if (host.Contains("localhost") && domainParts.Length != 2)
            {
                throw new HttpException(
                    404,
                    "Nie istnieje konto o podanym adresie");
            }

#endif
            if (!host.Contains("localhost") && domainParts.Length != 3)
            {
                throw new HttpException(
                    404,
                    "Nie istnieje konto o podanym adresie");
            }

            return domainParts[0];
        }
    }
}