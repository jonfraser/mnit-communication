using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace MNIT_Communication.Controllers
{
    public class AlertsController : Controller
    {
        [HttpGet]
        public ActionResult NewUser(Guid? id)
        {
            //Do nothing, just return the view as we will ajax in the alerts via the rest api
            return View(id);
        }

		[HttpGet]
		public ActionResult NewUserDone(Guid id)
		{
			//todo: Confirm the GUID is correct
			HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie);
			return View();
		}

		[HttpGet]
		[Authorize]
		public ActionResult Raise()
		{
			//Do nothing, just return the view as we will ajax in the alerts via the rest api
			return View();
		}

		[HttpGet]
		[Authorize]
		public ActionResult Status()
		{
			return View();
		}
    }
}