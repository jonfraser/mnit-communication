using System.Web.Routing;
using Microsoft.WindowsAzure;
using MNIT_Communication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MNIT_Communication.Helpers;
using Microsoft.Owin.Security;
using MNIT_Communication.Domain;
using System.Net.Http;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.AspNet.Identity;
using MNIT_Communication.Helpers.CustomSignIn;

namespace MNIT_Communication.Controllers
{
    public class HomeController : BaseController
	{
        public HomeController(IRuntimeContext runtimeContext) : base(runtimeContext)
        {
        }

        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            if (System.Web.HttpContext.Current.Request.IsAuthenticated)
                return RedirectToRoute(new {Controller = "Alerts", Action = "Status"});
            
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie);
			return await BaseView();
		}
        
        
	}

}