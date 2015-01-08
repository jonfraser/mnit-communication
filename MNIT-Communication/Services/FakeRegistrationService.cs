using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MNIT_Communication.Services
{
	public class FakeRegistrationService : IRegistrationService
	{
		public async System.Threading.Tasks.Task ProcessRegistrationRequest(Guid accessToken, string emailAddress)
		{
			Trace.Write("FakeRegistrationService.ProcessRegistrationRequest " + emailAddress);
			return;
		}

		public async System.Threading.Tasks.Task<Guid> SendRegistrationRequest(string email)
		{
			Trace.Write("FakeRegistrationService.SendRegistrationRequest " + email);
			return await Task.Run(() => Guid.NewGuid());
		}
	}
}