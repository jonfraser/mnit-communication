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

namespace MNIT_Communication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Confirmed()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<ActionResult> Index(string emailAddress)
        //{
        //    //if(!emailAddress.ToLower().EndsWith("@health.qld.gov.au"))
        //    //{
        //    //    throw new NotSupportedException();
        //    //}
        //    //var svc = new RegistrationService();
        //    //await svc.SendRegistrationRequest(emailAddress);

        //    ////TODO: Somehow (maybe in the above service) we need to partially register this user before sending them to the next step

        //    return RedirectToAction("Index", "Alerts");
        //}
    }
}