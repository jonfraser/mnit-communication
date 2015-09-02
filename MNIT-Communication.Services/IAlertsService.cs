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
        Task UpdateAlert(UpdateAlertRequest request);
        Task NotifyScheduledAlerts();
        Task<IEnumerable<Alert>> GetAlerts(Func<Alert, bool> predicate);
        Task<IEnumerable<Alert>> GetCurrentAlerts();
        Task<IEnumerable<Alert>> GetPastAlerts();
        Task<IEnumerable<Alert>> GetFutureAlerts();
        Task<IEnumerable<UserProfile>> GetSubscribersFor(params Guid[] alertables);
	}
}
