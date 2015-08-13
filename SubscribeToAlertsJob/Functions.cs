using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using MNIT_Communication.Domain;

namespace SubscribeToAlertsJob
{
    public class Functions
    {
        public static Task ProcessQueueMessage([ServiceBusTrigger(Queues.Registration)] SubscribeToAlertBrokeredMessage message, TextWriter log)
        {
            log.WriteLine("Received message " + message.CorrelationId.ToString() + " from queue for " + message.EmailAddress);
        
            try
            {
                //TODO - implement!!
            }
            catch (Exception ex)
            {
                log.WriteLine(string.Format("Error in ProcessMessageQueue: {0}", ex.ToString()));
                throw;
            }

            return Task.Run(() => {});
        }
    }
}