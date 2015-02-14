using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MNIT_Communication.Domain;

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
		
		public async Task<NewUserProfile> RetrieveNewUserProfile(Guid accessToken)
		{
			Trace.Write("FakeRegistrationService.RetrieveNewUserProfile");
			return new NewUserProfile
			{
				NewUserRegistrationId = accessToken,
				EmailAddressExternalProvider = "fraser.jc@gmail.com",
				EmailAdressInternal = "jon.fraser@health.qld.gov.au",
				MobilePhoneNumber = "+61416272575"
			};
		}
	}
}