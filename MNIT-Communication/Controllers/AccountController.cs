﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MNIT_Communication.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login(string ReturnUrl)
        {
			ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }
    }
}