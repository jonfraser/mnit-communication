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
		//[Authorize(Users = "*")]
		public async Task Raise([FromBody]RaiseAlertRequest request)
		{
			foreach (var alertable in request.AlertableId)
			{
				await alertsService.RaiseAlert(alertable, request.AlertDetail ?? request.AlertInfoShort, request.AlertInfoShort);
			}
		}
	}

	public class RaiseAlertRequest
	{
		public List<Guid> AlertableId { get; set; }
		public string AlertDetail { get; set; }
		public string AlertInfoShort { get; set; }
	}
}