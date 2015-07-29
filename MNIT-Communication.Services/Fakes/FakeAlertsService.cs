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


		public Task RaiseAlert(Guid alertableId, string alertDetail, string alertInfoShort)
		{
			Trace.Write("FakeAlertsService.RaiseAlert " + alertableId.ToString());
			return Task.FromResult(0);
		}


		public async Task<IEnumerable<AlertSummary>> GetCurrentAlerts()
		{
			Trace.Write("FakeAlertsService.GetCurrentAlerts");
			return await Task.Run(() => new List<AlertSummary>());
		}
	}
}