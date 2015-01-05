using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using MNIT_Communication.Domain;
using SendGrid;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

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

        public async Task ProcessRegistrationRequest(Guid accessToken, string emailAddress)
        {
            await StoreToken(emailAddress, accessToken);
            await SendEmail("fraser.jc@gmail.com", accessToken);
        }

        private async Task StoreToken(string emailAddress, Guid accessToken)
        {
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(CloudConfigurationManager.GetSetting("RedisConnection"));

            IDatabase cache = connection.GetDatabase();

            await cache.StringSetAsync(emailAddress, accessToken.ToString(), expiry: new TimeSpan(72, 0, 0));

        }

        private async Task SendEmail(string email, Guid accessToken)
        {
            // Create the email object first, then add the properties.
            var myMessage = new SendGridMessage();

            // Add the message properties.
            myMessage.From = new MailAddress("mnit-communication@health.qld.gov.au");

            // Add multiple addresses to the To field.
            List<String> recipients = new List<String> { email };

            myMessage.AddTo(recipients);

            myMessage.Subject = "You requested access?";

            //Add the HTML and Text bodies
            myMessage.Text = "You've got 72 hours to confirm your account via this link: http://mnit-communication.azurewebsites.net/api/User/Confirm/" + accessToken.ToString();
            myMessage.Text += Environment.NewLine;
            myMessage.Text += "If you have already selected your alerts they will be automatically added to your account once it is confirmed.";

            // Create credentials, specifying your user name and password.
            var credentials = new NetworkCredential("azure_853e23752ff2b9ce7c30020b435ea889@azure.com",
                CloudConfigurationManager.GetSetting("SendGridPassword"));

            // Create an Web transport for sending email.
            var transportWeb = new Web(credentials);

            // Send the email.
            await transportWeb.DeliverAsync(myMessage);
        }
    }
}