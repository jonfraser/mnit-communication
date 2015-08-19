using MNIT_Communication.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Security.Claims;
using System.Web.Helpers;
using MNIT_Communication.Services;
using MNIT.ErrorLogging;
using System.Web.Configuration;
using System.Configuration;

namespace MNIT_Communication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
			DependencyConfig.BuildUpContainer();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

			AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }


        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            HandleError(exception);
        }

        protected void HandleError(Exception exception)
        {
            if (exception.Message == string.Empty)
            {
                // redirect and dont log empty exceptions
                Response.Redirect("~/Error/Opps");
                return;
            }
                        
            var errorLogger = ServiceLocator.Resolve<IErrorLogger<Guid>>();
            errorLogger.LogError(exception);

            var defaultRedirect = GetCustomErrorDefaultRedirect();
            Response.Redirect(defaultRedirect);
        }

        private string GetCustomErrorDefaultRedirect()
        {
            try
            {
                CustomErrorsSection customErrorsSection = (CustomErrorsSection)ConfigurationManager.GetSection("system.web/customErrors");
                return customErrorsSection.DefaultRedirect;
            }
            catch
            {
                return string.Empty;
            }
            
        }
    }
}
