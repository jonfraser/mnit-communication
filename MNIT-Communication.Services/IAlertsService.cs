using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MNIT_Communication.Domain;
namespace MNIT_Communication.Services
{
	public interface IAlertsService
	{
		Task<Guid> SubscribeToAlerts(Guid userId, IEnumerable<Guid> alertables);

		Task RaiseAlert(RaiseAlertRequest request);

		Task<IEnumerable<Alert>> GetCurrentAlerts();

        Task<IEnumerable<UserProfile>> GetSubscribersFor(params Guid[] alertables);
	}
}
