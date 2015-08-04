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
		public async Task SendNew(Alert outageDetail)
		{
			await Task.Run(() =>
			{
				var hubContext = GlobalHost.ConnectionManager.GetHubContext<OutageHub>();
				hubContext.Clients.All.addOutageUpdateToPage(outageDetail, true, false, false);
			});
		}

		public void UpdateExisting(Alert outageDetail)
		{
			Clients.All.addOutageUpdateToPage(outageDetail, false, true, false);
		}

		public void RemoveExisting(Alert outageDetail)
		{
			Clients.All.addOutageUpdateToPage(outageDetail, false, false, true);
		}
	}
}