using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using MNIT_Communication.Domain;
using MNIT_Communication.Services;

namespace MNIT_Communication.Hubs
{
	public class OutageHub : Hub, IOutageHub
	{

	    private IHubContext hubContext;
        public OutageHub()
	    {
            hubContext = GlobalHost.ConnectionManager.GetHubContext<OutageHub>();
        }

        public async Task NotifyChange(Alert outageDetail)
		{
			await Task.Run(() =>
			{
				hubContext.Clients.All.notifyChange(outageDetail);
			});
		}
	}
}