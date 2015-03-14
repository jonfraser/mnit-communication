using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MNIT_Communication.Domain;

namespace MNIT_Communication.Services
{
	public class FakeAlertsService : IAlertsService
	{
		public async Task<Guid> RegisterNewUserForInitialAlerts(Guid newUserRegistrationId, string emailAddress, IEnumerable<Guid> alertables)
		{
			Trace.Write("FakeAlertsService.RegisterNewUserForInitialAlerts " + emailAddress);
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