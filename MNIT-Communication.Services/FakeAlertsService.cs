using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MNIT_Communication.Services
{
	public class FakeAlertsService : IAlertsService
	{
		public async System.Threading.Tasks.Task<Guid> RegisterNewUserForInitialAlerts(Guid newUserRegistrationId, string emailAddress, IEnumerable<Guid> alertables)
		{
			Trace.Write("FakeAlertsService.RegisterNewUserForInitialAlerts " + emailAddress);
			return await Task.Run(() => Guid.NewGuid());
		}
	}
}