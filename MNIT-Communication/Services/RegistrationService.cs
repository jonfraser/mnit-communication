using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using MNIT_Communication.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MNIT_Communication.Services
{
    public class RegistrationService
    {
        private readonly string RegistrationQueue = "RegistrationQueue";
        public async Task<Guid> SendRegistrationRequest(string email)
        {
            var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            var queueExists = namespaceManager.QueueExists(RegistrationQueue);
            if (!queueExists)
            {
                await namespaceManager.CreateQueueAsync(RegistrationQueue);
            }

            var client = QueueClient.CreateFromConnectionString(connectionString, RegistrationQueue);

            var message = new NewUserRegistrationBrokeredMessage
            {
                CorrelationId = Guid.NewGuid(),
                EmailAddress = email
            };

            await client.SendAsync(new BrokeredMessage(message));

            return message.CorrelationId;
        }
    }
}