using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MNIT_Communication.Domain;

namespace MNIT_Communication.Services.Fakes
{
	public class FakeAlertsService : IAlertsService
	{
		public async Task<Guid> SubscribeToAlerts(Guid userId, IEnumerable<Guid> alertables)
		{
			Trace.Write("FakeAlertsService.RegisterNewUserForInitialAlerts " + userId);
			return await Task.Run(() => Guid.NewGuid());
		}
        
		public Task RaiseAlert(RaiseAlertRequest request)
		{
		    foreach (var alertable in request.Alertables)
		    {
		        Trace.Write("FakeAlertsService.RaiseAlert " + alertable.Id.ToString());
		    }
            
			return Task.FromResult(new Alert());
		}
        
		public async Task<IEnumerable<Alert>> GetCurrentAlerts()
		{
			Trace.Write("FakeAlertsService.GetCurrentAlerts");
			return await Task.Run(() => new List<Alert>());
		}
	}
}