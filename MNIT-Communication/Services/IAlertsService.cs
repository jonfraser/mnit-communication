using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace MNIT_Communication.Services
{
	public interface IAlertsService
	{
		Task<Guid> RegisterNewUserForInitialAlerts(Guid newUserRegistrationId, string emailAddress, IEnumerable<Guid> alertables);
	}
}
