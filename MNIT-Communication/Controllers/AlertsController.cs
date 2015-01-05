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
        public ActionResult NewUser(Guid? newUserRegistrationId)
        {
            //Do nothing, just return the view as we will ajax in the alerts via the rest api
            return View(newUserRegistrationId);
        }
    }
}