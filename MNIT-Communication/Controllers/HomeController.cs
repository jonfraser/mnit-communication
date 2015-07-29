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
using MNIT_Communication.Domain;
using System.Net.Http;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.AspNet.Identity;
using MNIT_Communication.Helpers.CustomSignIn;

namespace MNIT_Communication.Controllers
{
	public class HomeController : Controller
	{
        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current.Request.IsAuthenticated)
                return RedirectToRoute(new {Controller = "Alerts", Action = "Status"});
            
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie);
			return View();
		}
        
        public async Task<ActionResult> SetUserProfile(dynamic id)
		{
			using (var client = new HttpClient())
			{
                var scheme = HttpContext.Request.Url.Scheme + "://";
			    var authority = HttpContext.Request.Url.Authority;
                
                //TODO - why did this route stop working //i.e /api/User/UserProfile/[GUID_HERE]. Had to move to query string below :(
                //var url = Url.HttpRouteUrl("DefaultApi",
                //        new
                //        {
                //            action = "UserProfile",
                //            controller = "User",
                //            id = id
                //        });

			    var url = @"/api/User/UserProfile?id=" + id.ToString();

			    var type = typeof(UserProfile);
                
                var getProfileUri = scheme + authority + url;
                var response = await client.GetStringAsync(getProfileUri);
                var userProfile = JsonConvert.DeserializeObject(response, type);

                return View(userProfile);
			}
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
		    if (string.IsNullOrEmpty(returnUrl))
		        returnUrl = "/";
            
            // Request a redirect to the external login provider
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Home", new { ReturnUrl = returnUrl }));
		}

		[AllowAnonymous]
		public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
		{
			var owinAuth = HttpContext.GetOwinContext().Authentication;
			owinAuth.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie);
			
			var loginInfo = await owinAuth.GetExternalLoginInfoAsync();
			if (loginInfo == null)
			{
				return new HttpUnauthorizedResult();
			}
			var SignInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);

			//TODO: this would theoretically be called whenever someone auths from any stage in teh app,
			//not just initial setup - need to check for this
			if (!string.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith("/Home/SetUserProfile"))
			{
				var id = new Guid(StringHelpers.PullGuidOffEndOfUrl(returnUrl));

				using (var client = new HttpClient())
				{
					var putProfileUri = HttpContext.Request.Url.Scheme +
										"://" +
										HttpContext.Request.Url.Authority +
										Url.HttpRouteUrl("DefaultApi", new { action = "UserProfile", controller = "User" });
					//TODO - if Response = failure?
                    var response = await client.PutAsJsonAsync(putProfileUri, new UserProfile
					{
						Id = id,
                        EmailAddressExternalProvider = loginInfo.Email,
                        ExternalProvider = loginInfo.Login.LoginProvider,
                        ExternalId = loginInfo.Login.ProviderKey
					});
				}
			}

			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			throw new UnauthorizedAccessException();
		}

		public ActionResult Confirmed()
		{
			return View();
		}

		public ActionResult MobileNumberRemoved()
		{
			return View();
		}
	}

}