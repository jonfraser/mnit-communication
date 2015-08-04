using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MNIT_Communication.Domain;
using MNIT_Communication.Hubs;
using MNIT_Communication.Services;

namespace MNIT_Communication.Areas.api.v1
{
	public partial class AlertsController : ApiController
	{
		[HttpPost]
		//[Authorize(Users = "*")]
		public async Task Raise([FromBody]RaiseAlertRequest request)
		{
            await alertsService.RaiseAlert(request);
		}
	}

	
}