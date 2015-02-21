using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
			//Confirm the GUID is correct

			return View();
		}

		[HttpGet]
		//[Authorize]
		public ActionResult Raise()
		{
			//Do nothing, just return the view as we will ajax in the alerts via the rest api
			return View();
		}
    }
}