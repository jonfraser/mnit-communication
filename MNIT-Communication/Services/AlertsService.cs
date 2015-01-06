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
    public class AlertsService : IAlertsService
    {
        private readonly string AlertsQueue = "AlertsQueue";

        public async Task<Guid> RegisterNewUserForInitialAlerts(Guid newUserRegistrationId, string emailAddress, IEnumerable<Guid> alertables)
        {
            var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            var queueExists = namespaceManager.QueueExists(AlertsQueue);
            if (!queueExists)
            {
                await namespaceManager.CreateQueueAsync(AlertsQueue);
            }

            var client = QueueClient.CreateFromConnectionString(connectionString, AlertsQueue);


            foreach (var alertable in alertables)
            {
                var message = new RegisterAlertBrokeredMessage
                {
                    CorrelationId = Guid.NewGuid(),
                    EmailAddress = emailAddress,
                    AlertableId = alertable
                };
                await client.SendAsync(new BrokeredMessage(message));
            }

            return newUserRegistrationId;
        }

    }
}