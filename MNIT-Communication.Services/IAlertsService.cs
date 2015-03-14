using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MNIT_Communication.Domain;
namespace MNIT_Communication.Services
{
	public interface IAlertsService
	{
		Task<Guid> RegisterNewUserForInitialAlerts(Guid newUserRegistrationId, string emailAddress, IEnumerable<Guid> alertables);

		Task RaiseAlert(Guid alertableId, string alertDetail, string alertInfoShort);

		Task<IEnumerable<AlertSummary>> GetCurrentAlerts();
	}
}
