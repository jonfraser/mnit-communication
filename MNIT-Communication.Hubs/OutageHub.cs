﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using MNIT_Communication.Domain;

namespace MNIT_Communication.Hubs
{
	public class OutageHub : Hub
	{
		public async static Task SendNew(AlertSummary outageDetail)
		{
			await Task.Run(() =>
			{
				var hubContext = GlobalHost.ConnectionManager.GetHubContext<OutageHub>();
				hubContext.Clients.All.addOutageUpdateToPage(outageDetail, true, false, false);
			});
		}

		public void UpdateExisting(AlertSummary outageDetail)
		{
			Clients.All.addOutageUpdateToPage(outageDetail, false, true, false);
		}

		public void RemoveExisting(AlertSummary outageDetail)
		{
			Clients.All.addOutageUpdateToPage(outageDetail, false, false, true);
		}
	}
}