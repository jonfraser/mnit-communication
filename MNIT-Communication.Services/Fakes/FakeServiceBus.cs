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
            var userService = ServiceLocator.Resolve<IUserService>();
            
            if (queueName == Queues.Registration)
            {
                var castMessage = message as NewUserRegistrationBrokeredMessage;
                await userService.ProcessServiceBusRegistrationMessage(castMessage);
            }

            if (queueName == Queues.AlertsSubscription)
            {
                var castMessage = message as SubscribeToAlertBrokeredMessage;
                var store = ServiceLocator.Resolve<IShortTermStorage>();
            }

            if (queueName == Queues.MobileNumberVerify)
            {
                var castMessage = message as VerifyMobileNumberBrokeredMessage;
                await userService.VerifyMobileNumber(castMessage.MobileNumber, castMessage.NewUserRegistrationId);
            }
        }

        public async Task SendToTopicAsync<T>(T message, string topicName)
        {
            var mail = ServiceLocator.Resolve<ISendEmail>();
            var sms = ServiceLocator.Resolve<ISendSms>();
            
            if (topicName == Topics.Alerts)
            {
                var castMessage = message as AlertBrokeredMessage;
                
                await mail.Send(from: "mnit-communication@health.qld.gov.au",
                                        to: new List<String> { "fraser.jc@gmail.com" },
                                        subject: "An alert has been raised!",
                                        body: castMessage.AlertDetail);

                
                var mobileNumber = "0416272575";
                await sms.SendSimple(mobileNumber, castMessage.AlertInfoShort);

            }

        }
    }
}
