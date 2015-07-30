using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MNIT_Communication.Attributes;
using MNIT_Communication.Domain;
using MNIT_Communication.Services;

namespace MNIT_Communication.Controllers
{
    //TODO - UserProfileConfirmedAttribute should be applied here via Autofac
    [Authorize]
    public class AlertsController : Controller
    {
        private readonly IUserService userService;
        private readonly IRuntimeContext runtimeContext;

        public AlertsController(IUserService userService, IRuntimeContext runtimeContext)
        {
            this.userService = userService;
            this.runtimeContext = runtimeContext;
        }

        [HttpGet]
        public async Task<ActionResult> Subscribe(Guid? id)
        {
            UserProfile userProfile;
            userProfile = id.HasValue ? await userService.RetrieveUserProfile(id.Value) : runtimeContext.CurrentProfile;

            if (userProfile == null)
            {
                return RedirectToRoute(new {controller = "Account", action = "Unconfirmed"});
            }

            //Do nothing, just return the view as we will ajax in the alerts via the rest api
            return View(userProfile);
        }
        
		[HttpGet]
        [UserProfileConfirmed]
		public ActionResult Raise()
		{
			//Do nothing, just return the view as we will ajax in the alerts via the rest api
			return View();
		}

		[HttpGet]
        [UserProfileConfirmed]
		public ActionResult Status()
		{
			return View();
		}
    }
}