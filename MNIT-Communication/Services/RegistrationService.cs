using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
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
        public void SendRegistrationRequest(string email)
        {
            var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            var queueExists = namespaceManager.QueueExists(RegistrationQueue);
            if(!queueExists)
            {
                namespaceManager.CreateQueue(RegistrationQueue);
            }

            var client = QueueClient.CreateFromConnectionString(connectionString, RegistrationQueue);
            client.Send(new BrokeredMessage(email));

        }
    }
}