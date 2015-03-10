using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;

namespace MNIT_Communication.Helpers.CustomSignIn
{
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
}