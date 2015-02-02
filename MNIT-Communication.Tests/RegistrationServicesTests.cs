using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MNIT_Communication.Services;

namespace MNIT_Communication.Tests
{
	[TestClass]
	public class RegistrationServicesTests
	{
		[TestMethod]
		public void ProcessServiceBusRegistrationMessageWithValidParamsShouldStoreTokenAndSendEmail()
		{
			IRegistrationService reg = new RegistrationService();
			reg.ProcessServiceBusRegistrationMessage("https://localhost:44300", new Domain.NewUserRegistrationBrokeredMessage
				{
					CorrelationId = Guid.NewGuid(),
					EmailAddress = "jon.fraser@health.qld.gov.au"
				});
		}

		[TestMethod]
		public void ProcessRegistrationWithValidParamsShouldStoreTokenAndSendEmail()
		{
			IRegistrationService reg = new RegistrationService();
			reg.ProcessRegistrationRequest(Guid.NewGuid(), "fraser.jc@gmail.com");
		}
	}
}
