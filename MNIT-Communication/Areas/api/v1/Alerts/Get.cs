using MNIT_Communication.Domain;
using MNIT_Communication.Services;
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
        public async Task<IEnumerable<Alert>> Get()
        {
			return await alertsService.GetCurrentAlerts();
        }
    }
}