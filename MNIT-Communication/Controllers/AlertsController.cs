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
    public class AlertsController : BaseController
    {
        private readonly IUserService userService;

        public AlertsController(IUserService userService, IRuntimeContext runtimeContext) : base(runtimeContext)
        {
            this.userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult> Subscribe(Guid? id)
        {
            UserProfile userProfile;
            userProfile = id.HasValue ? await userService.RetrieveUserProfile(id.Value) : await runtimeContext.CurrentProfile();

            if (userProfile == null)
            {
                return RedirectToRoute(new {controller = "Account", action = "Unconfirmed"});
            }

            //Do nothing, just return the view as we will ajax in the alerts via the rest api
            return await BaseView(userProfile);
        }
        
		[HttpGet]
        [IsAdministrator]
		public async Task<ActionResult> Raise()
		{
			//Do nothing, just return the view as we will ajax in the alerts via the rest api
			return await BaseView();
		}

		[HttpGet]
        [UserProfileConfirmed]
		public async Task<ActionResult> Status()
		{
			return await BaseView();
		}
    }
}