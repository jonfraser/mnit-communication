using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace MNIT_Communication.Helpers.CustomSignIn
{
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
}