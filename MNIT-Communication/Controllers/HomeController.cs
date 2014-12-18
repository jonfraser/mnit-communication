using Microsoft.WindowsAzure;
using MNIT_Communication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

        [HttpPost]
        public ActionResult Index(string emailAddress)
        {
            if(!emailAddress.ToLower().EndsWith("@health.qld.gov.au"))
            {
                throw new NotSupportedException();
            }
            var svc = new RegistrationService();
            svc.SendRegistrationRequest(emailAddress);

            return RedirectToAction("Index");
        }
    }
}