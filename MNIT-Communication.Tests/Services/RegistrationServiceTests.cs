using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MNIT_Communication.Services;
using MNIT_Communication.Services.Fakes;
using Moq;
using Xunit;

namespace MNIT_Communication.Tests.Services
{
	public class UserServiceTests
	{
        private static Mock<IRepository> repoMock;
        private static Mock<IShortTermStorage> storeMock;
        private static Mock<ISendEmail> mailMock;
        private static FakeEmailService mailFake = new FakeEmailService();
        private static Mock<ISendSms> smsMock;
        private static Mock<IUrlShorten> urlMock;
        private static Mock<IServiceBus> serviceBusMock; 
        
        public UserServiceTests()
	    {
	        ConfigureMocks();
	    }

	    public static void ConfigureMocks()
	    {
            repoMock = new Mock<IRepository>(); 
            storeMock = new Mock<IShortTermStorage>();
            mailMock = new Mock<ISendEmail>();
            smsMock = new Mock<ISendSms>();
            urlMock = new Mock<IUrlShorten>();  
            serviceBusMock = new Mock<IServiceBus>();  	        
	    }

	    public class ProcessServiceBusRegistrationMessage
	    {
	        public ProcessServiceBusRegistrationMessage()
	        {
	            ConfigureMocks();
	        }
            
            [Fact]
		    public async Task WithValidParamsShouldStoreTokenAndSendEmailWithConfirmationLink()
            {
                var confirmationLink = "https://mnit-communication.azurewebsites.net/api/User/Confirm/";
                var hoursToConfirm = 72;
                
                var service = new UserService(repoMock.Object, storeMock.Object, mailMock.Object, smsMock.Object, urlMock.Object, serviceBusMock.Object);

                var message = new MNIT_Communication.Domain.NewUserRegistrationBrokeredMessage
                {
                    CorrelationId = Guid.NewGuid(),
                    EmailAddress = "jon.fraser@health.qld.gov.au"
                };

                //TODO - Why does this result in a Null Ref?
                //mailMock.Setup( //Attach the Fake email service so we can see the actual output
                //   m => m.Send(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>()))
                //   .Callback<string, List<string>, string, string>((from, to, subject, body) =>
                //   {
                //       mailFake.Send(from, to, subject, body);
                //   });
                
                await service.ProcessServiceBusRegistrationMessage(message);

                storeMock.Verify(m => m.StoreValue(
                    message.EmailAddress, //key
                    message.CorrelationId.ToString(), //value
                    It.Is<TimeSpan>(timeSpan => timeSpan.TotalHours == hoursToConfirm)), //lifespan
                Times.AtLeastOnce);

                mailMock.Verify(m => m.Send(
                    "mnit-communication@health.qld.gov.au",  //from
                    It.Is<List<string>>(addresses => addresses.Contains(message.EmailAddress)), //to
                    "Confirm your MNIT Communication account", //subject
                    It.Is<string>(body => body.Contains(message.CorrelationId.ToString()) &&
                                          body.Contains(confirmationLink))), //body
                Times.AtLeastOnce());
		    }
	    }

        public class VerifyMobileNumber
        {
            public VerifyMobileNumber()
            {
                ConfigureMocks();
            }

            [Fact]
            public async Task WithValidNumberShouldSendSms()
            {
                var service = new UserService(repoMock.Object, storeMock.Object, mailMock.Object, smsMock.Object, urlMock.Object, serviceBusMock.Object);
                var accessToken = Guid.NewGuid();
                var mobileNumber = "+61416272575";
                
                await service.VerifyMobileNumber(mobileNumber, accessToken);


            }
        }

		
	}
}
