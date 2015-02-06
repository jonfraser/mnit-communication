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
using MNIT_Communication.Helpers;
using Microsoft.Owin.Security;

namespace MNIT_Communication.Controllers
{
	public class HomeController : Controller
	{

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult SetUserProfile(Guid id)
		{
			//TODO: We should be able to get the oauth token from the id (that'll be in redis i guess?)
			return View(id);
		}

		public ActionResult LinkExternalAccount(Guid id)
		{
			return View(id);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin(string provider, string returnUrl)
		{

			// Request a redirect to the external login provider
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Home", new { ReturnUrl = returnUrl }));
		}

		[AllowAnonymous]
		public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
		{
			var loginInfo = await HttpContext.GetOwinContext().Authentication.GetExternalLoginInfoAsync();
			if (loginInfo == null)
			{
				return new HttpUnauthorizedResult();
			}

			var provider = loginInfo.Login.LoginProvider;
			var email = loginInfo.Email;
			//TODO: create an api endpoint to call that will allow setting of an external provider on a user
			//TODO: Store this info
			return Redirect(returnUrl);

		}

		public ActionResult Confirmed()
		{
			return View();
		}

	}
}