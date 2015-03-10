using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNIT_Communication.Helpers.CustomSignIn
{
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
}