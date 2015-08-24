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
        
		public async Task RaiseAlert(RaiseAlertRequest request)
		{
            await Task.Run(() =>
            {
                foreach (var alertable in request.Alertables)
		        {
		            Trace.Write("FakeAlertsService.RaiseAlert " + alertable.Id.ToString());
		        }
            
			    return new Alert();
            });
		}

        public async Task UpdateAlert(UpdateAlertRequest request)
        {
            await Task.Run(() =>
            {
               Trace.Write("FakeAlertsService.RaiseAlert " + request.AlertId.ToString());
               return new Alert();
            });
        }

        public async Task<IEnumerable<Alert>> GetCurrentAlerts()
		{
			Trace.Write("FakeAlertsService.GetCurrentAlerts");
			return await Task.Run(() => new List<Alert>());
		}

	    public async Task<IEnumerable<UserProfile>> GetSubscribersFor(params Guid[] alertables)
	    {
            Trace.Write("FakeAlertsService.GetSubscribersFor");
            return await Task.Run(() => new List<UserProfile>());
	    }
	}
}