using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;

namespace MNIT_Communication.Helpers.CustomSignIn
{
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
			return new ApplicationUser { UserName = login.ProviderKey };
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