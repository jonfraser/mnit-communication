using Microsoft.WindowsAzure;
using MNIT_Communication.Attributes;
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
using System.Security;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.AspNet.Identity;
using MNIT_Communication.Helpers.CustomSignIn;

namespace MNIT_Communication.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly Lazy<IUserService> userService;
        private readonly IAuditService auditService;

        public AccountController(IRuntimeContext runtimeContext, Lazy<IUserService> userService, IAuditService auditService) : base(runtimeContext)
        {
            this.userService = userService;
            this.auditService = auditService;
        }

        [AllowAnonymous]
        public async Task<ActionResult> Login(string ReturnUrl)
        {
			ViewBag.ReturnUrl = ReturnUrl;
            return await BaseView();
        }

		[HttpPost]
        [AllowAnonymous]
		public async Task<RedirectResult> LogOff()
		{
            return await Task.Run(() =>
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.ApplicationCookie);
		        return Redirect("/");
            });
		}

        public async Task<ActionResult> SetUserProfile(Guid? id)
        {
            var userProfile = id.HasValue ? await userService.Value.RetrieveUserProfile(id.Value) : 
                                            await runtimeContext.CurrentProfile();

            return await BaseView(userProfile);
        }

        [AllowAnonymous]
        public async Task<ActionResult> LinkExternalAccount(Guid? id)
        {
            if (id.HasValue) //Still in the Regsitration workflow, pass the id down
                return await BaseView(id.Value);

            if (await runtimeContext.HasProfile()) //Must be editing existing Link, get Profile and pass that Id
                return await BaseView((await runtimeContext.CurrentProfile()).Id);

            //If we get here, an un-authed user is playing silly buggers
            throw new SecurityException("User attempted to browse a page whilst not authenticated!");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = "/";

            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
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
            var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            await signInManager.ExternalSignInAsync(loginInfo, isPersistent: false);

            var externalId = loginInfo.Login.ProviderKey;

            await auditService.LogAuditEventAsync(new AuditEvent
            {
                AuditType = AuditType.UserLogin,
                Details = string.Format("Someone with an External Id {0} has just logged in via {1}", externalId, loginInfo.Login.LoginProvider)
            });

            //If the user has logged in, but has no (Confirmed) Profile, redirect to the 'Unconfirmed' page 
            //TODO: this would theoretically be called whenever someone auths from any stage in teh app, not just initial setup - need to check for this
            if (!string.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith("/Account/SetUserProfile"))
            {
                using (var client = new HttpClient())
                {
                    var putProfileUri = HttpContext.Request.Url.Scheme +
                                        "://" +
                                        HttpContext.Request.Url.Authority +
                                        Url.HttpRouteUrl("DefaultApi", new { action = "UserProfile", controller = "User" });
                    //TODO - if Response = failure?
                    var id = new Guid(StringHelpers.PullGuidOffEndOfUrl(returnUrl));
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

        [HttpGet]
        public async Task<ActionResult> NewUserDone(Guid id)
        {
            var userProfile = await userService.Value.RetrieveUserProfile(id);
            var secret = userProfile.ConfirmationSecret;
            
            //TODO: Confirm the GUID is correct
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie);
            return await BaseView(secret);
        }

        [AllowAnonymous]
        public async Task<ActionResult> Confirmed()
        {
            return await BaseView();
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmationFailed()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie);
            return await BaseView();
        }

        [Authorize]
        public async Task<ActionResult> Unconfirmed()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie);
            return await BaseView();
        }

        public async Task<ActionResult> MobileNumberRemoved()
        {
            return await BaseView();
        }

        [UserProfileConfirmed]
        public async Task<ActionResult> RequestAdmin()
        {
            var admins = await userService.Value.ListAdministrators();
            return await BaseView(admins);
        }

        [IsAdministrator]
        public async Task<ActionResult> GrantAdmin(Guid id)
        {
            var administratorId = (await runtimeContext.CurrentProfile()).Id; //The logged in user should be an administrator

            await userService.Value.GrantAdmin(id, administratorId);

            var userGrantedTo = await userService.Value.RetrieveUserProfile(id);

            return await BaseView(userGrantedTo);
        }
    }
}