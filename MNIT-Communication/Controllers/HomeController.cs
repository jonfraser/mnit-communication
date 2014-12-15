﻿using Microsoft.WindowsAzure;
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

            var svc = new RegistrationService();
            svc.SendRegistrationRequest("frasejon", "jon.fraser@health.qld.gov.au");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}