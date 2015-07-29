using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MNIT_Communication.Services;

namespace MNIT_Communication.Controllers
{
    public class AlertsController : Controller
    {
        private readonly IUserService userService;

        public AlertsController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult> Subscribe(Guid id)
        {
            var userProfile = await userService.RetrieveUserProfile(id);
            //Do nothing, just return the view as we will ajax in the alerts via the rest api
            return View(userProfile);
        }

		[HttpGet]
		public ActionResult NewUserDone(Guid id)
		{
			//TODO: Confirm the GUID is correct
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