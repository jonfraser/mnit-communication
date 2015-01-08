using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MNIT_Communication.Areas.api.v1
{
	public partial class AlertsController : ApiController
	{
		[HttpPost]
		public async Task<Guid> AssignNew([FromBody]AssignNewAlertsRequest request)
		{
			return await alertsService.RegisterNewUserForInitialAlerts(request.newUserRegistrationId, "email", request.alertables);
		}
	}

	public class AssignNewAlertsRequest
	{
		public Guid newUserRegistrationId { get; set; }
		public IEnumerable<Guid> alertables { get; set; }
	}
}