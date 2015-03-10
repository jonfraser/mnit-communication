using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace MNIT_Communication.Hubs
{
	public class OutageHub : Hub
	{
		public void Send(string service, DateTime start, string update, DateTime updateDate)
		{
			Clients.All.addOutageUpdateToPage(service, start, update, updateDate);
		}
	}
}