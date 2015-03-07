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

namespace MNIT_Communication.Controllers
{
	public class HomeController : Controller
	{
		private IRegistrationService registrationService;

		public HomeController(IRegistrationService regSvc)
		{
			registrationService = regSvc;
		}

		public ActionResult Index()
		{
			return View();
		}

		public async Task<ActionResult> SetUserProfile(Guid id)
		{
			using (var client = new HttpClient())
			{
				var getProfileUri = HttpContext.Request.Url.Scheme +
									"://" +
									HttpContext.Request.Url.Authority +
									Url.HttpRouteUrl("DefaultApi", new { action = "NewUserProfile", controller = "User", newUserRegistrationId = id.ToString() });
				var response = await client.GetStringAsync(getProfileUri);
				var userProfile = JsonConvert.DeserializeObject(response, typeof(NewUserProfile));
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
			// Request a redirect to the external login provider
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Home", new { ReturnUrl = returnUrl }));
		}

		[AllowAnonymous]
		public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
		{
			var owinAuth = HttpContext.GetOwinContext().Authentication;
			var loginInfo = await owinAuth.GetExternalLoginInfoAsync();
			if (loginInfo == null)
			{
				return new HttpUnauthorizedResult();
			}
			var SignInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);

			//TODO: this would theoretically be called whenever someone auths from any stage in teh app,
			//not just initial setup - need to check for this
			if (returnUrl.StartsWith("/Home/SetUserProfile"))
			{
				var newRegistrationIdFromReturnUrl = new Guid(StringHelpers.PullGuidOffEndOfUrl(returnUrl));

				using (var client = new HttpClient())
				{
					var putProfileUri = HttpContext.Request.Url.Scheme +
										"://" +
										HttpContext.Request.Url.Authority +
										Url.HttpRouteUrl("DefaultApi", new { action = "NewUserProfile", controller = "User" });
					var response = await client.PutAsJsonAsync(putProfileUri, new NewUserProfile
						{
							EmailAddressExternalProvider = loginInfo.Email,
							NewUserRegistrationId = newRegistrationIdFromReturnUrl,
							ExternalProvider = loginInfo.Login.LoginProvider
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

	public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
	{
		public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
			: base(userManager, authenticationManager)
		{
		}

		//public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
		//{
		//	return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
		//}

		public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
		{
			return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
		}
	}

	public class ApplicationUser : IUser<string>// : IdentityUser
	{
		private string _username;
		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			// Add custom user claims here
			return userIdentity;
		}

		public string Id
		{
			get { return UserName; }
		}
		
		public string UserName
		{
			get
			{
				return _username;
			}
			set
			{
				this._username = value;
			}
		}
	}

	public class ApplicationUserManager : UserManager<ApplicationUser>
	{
		public ApplicationUserManager(IUserStore<ApplicationUser> store)
			: base(store)
		{
		}

		public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
		{
			var manager = new ApplicationUserManager(new NoUserStore());//<string>(context.Get<ApplicationNoDbContext>()));
			//new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
			
			
			return manager;
		}
	}

	public class ApplicationNoDbContext// : IdentityDbContext<ApplicationUser>
	{
		public ApplicationNoDbContext()
		{
		}

		public static ApplicationNoDbContext Create()
		{
			return new ApplicationNoDbContext();
		}
	}

	public class NoUserStore : IUserStore<ApplicationUser>, IUserLoginStore<ApplicationUser>, IUserLockoutStore<ApplicationUser, string>, IUserTwoFactorStore<ApplicationUser, string>
	{

		public Task CreateAsync(ApplicationUser user)
		{
			throw new NotImplementedException();
		}

		public Task DeleteAsync(ApplicationUser user)
		{
			throw new NotImplementedException();
		}

		public async Task<ApplicationUser> FindByIdAsync(string userId)
		{
			return new ApplicationUser();
		}

		public Task<ApplicationUser> FindByNameAsync(string userName)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(ApplicationUser user)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login)
		{
			throw new NotImplementedException();
		}

		public async Task<ApplicationUser> FindAsync(UserLoginInfo login)
		{
			return new ApplicationUser{UserName = login.ProviderKey};
		}

		public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user)
		{
			throw new NotImplementedException();
		}

		public Task RemoveLoginAsync(ApplicationUser user, UserLoginInfo login)
		{
			throw new NotImplementedException();
		}

		public Task<int> GetAccessFailedCountAsync(ApplicationUser user)
		{
			throw new NotImplementedException();
		}

		public async Task<bool> GetLockoutEnabledAsync(ApplicationUser user)
		{
			return false;
		}

		public Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
		{
			throw new NotImplementedException();
		}

		public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
		{
			throw new NotImplementedException();
		}

		public Task ResetAccessFailedCountAsync(ApplicationUser user)
		{
			throw new NotImplementedException();
		}

		public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
		{
			throw new NotImplementedException();
		}

		public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
		{
			throw new NotImplementedException();
		}

		public async Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user)
		{
			return false;
		}

		public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled)
		{
			throw new NotImplementedException();
		}
	}

}