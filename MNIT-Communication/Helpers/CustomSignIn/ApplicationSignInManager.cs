using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace MNIT_Communication.Helpers.CustomSignIn
{
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

	    public override Task SignInAsync(ApplicationUser user, bool isPersistent, bool rememberBrowser)
	    {
	        
            
            return base.SignInAsync(user, isPersistent, rememberBrowser);
	    }
	}
}