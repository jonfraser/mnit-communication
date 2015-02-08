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
		

		public async System.Threading.Tasks.Task<Guid> SendRegistrationRequest(string email)
		{
			Trace.Write("FakeRegistrationService.SendRegistrationRequest " + email);
			return await Task.Run(() => Guid.NewGuid());
		}


		public async Task ProcessServiceBusRegistrationMessage(Domain.NewUserRegistrationBrokeredMessage message)
		{
			Trace.Write("FakeRegistrationService.ProcessServiceBusRegistrationMessage " + message.CorrelationId);
			return;
		}

		public async Task VerifyMobileNumber(string mobileNumber, Guid accessToken)
		{
			Trace.Write("FakeRegistrationService.VerifyMobileNumber " + mobileNumber);
			return;
		}

		public async Task RequestVerificationOfMobileNumber(string mobileNumber, Guid accessToken)
		{
			Trace.Write("FakeRegistrationService.RequestVerificationOfMobileNumber " + mobileNumber);
			return;
		}
	}
}