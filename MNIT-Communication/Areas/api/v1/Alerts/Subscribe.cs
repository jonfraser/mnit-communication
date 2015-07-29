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
		public async Task<Guid> Subscribe([FromBody]AlertsSubscriptionRequest request)
		{
			return await alertsService.SubscribeToAlerts(request.userId, request.alertables);
		}
	}

    public class AlertsSubscriptionRequest
	{
		public Guid userId { get; set; }
		public IEnumerable<Guid> alertables { get; set; }
	}
}