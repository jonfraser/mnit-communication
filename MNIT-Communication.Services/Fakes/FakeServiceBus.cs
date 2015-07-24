using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNIT_Communication.Domain;

namespace MNIT_Communication.Services.Fakes
{
    public class FakeServiceBus: IServiceBus
    {
        public async Task SendToQueueAsync<T>(T message, string queueName)
        {
            var registrationService = ServiceLocator.Resolve<IRegistrationService>();
            
            if (queueName == Queues.Registration)
            {
                var castMessage = message as NewUserRegistrationBrokeredMessage;
                await registrationService.ProcessServiceBusRegistrationMessage(castMessage);
            }

            if (queueName == Queues.AlertsRegistration)
            {

            }

            if (queueName == Queues.MobileNumberVerify)
            {
                var castMessage = message as VerifyMobileNumberBrokeredMessage;
                await registrationService.VerifyMobileNumber(castMessage.MobileNumber, castMessage.NewUserRegistrationId);
            }
        }

        public async Task SendToTopicAsync<T>(T message, string topicName)
        {
            var mail = ServiceLocator.Resolve<ISendEmail>();
            
            if (topicName == Topics.Alerts)
            {
                var castMessage = message as AlertBrokeredMessage;

                await mail.Send(from: "mnit-communication-DEV@health.qld.gov.au",
                                        to: new List<String> { "fraser.jc@gmail.com", "sjperske@gmail.com" },
                                        subject: "An alert has been raised!",
                                        body: castMessage.AlertDetail);

            }

        }
    }
}
