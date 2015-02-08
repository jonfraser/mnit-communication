using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using MNIT_Communication.Domain;
using SendGrid;
using StackExchange.Redis;

namespace MNIT_Communication.Services
{
	public class RegistrationService : IRegistrationService
	{
		private readonly string RegistrationQueue = "RegistrationQueue";
		private readonly List<string> validEmail = new List<string>(){"jon.fraser@health.qld.gov.au",
												   "ian.missenden@health.qld.gov.au",
												   "anthony.kanowski@health.qld.gov.au"};
		private readonly List<string> validMobile = new List<string>() { "+61416272575" };

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

		public async Task ProcessServiceBusRegistrationMessage(NewUserRegistrationBrokeredMessage message)
		{
			await StoreToken(message.EmailAddress, message.CorrelationId);
			var emailToSend = validEmail.Contains(message.EmailAddress) ? message.EmailAddress : "fraser.jc@gmail.com";
			await SendEmail(emailToSend, message.CorrelationId);
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
			myMessage.Text = "You've got 72 hours to confirm your account via this link: https://mnit-communication.azurewebsites.net/api/User/Confirm/" + accessToken.ToString();
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
		public async Task VerifyMobileNumber(string mobileNumber, Guid accessToken)
		{
			//TODO: Do this in a few parts, push message onto queue and process it separately
			var cancellationEndpoint = string.Format("https://mnit-communication.azurewebsites.net/api/User/RejectMobile/{0}", accessToken.ToString());
			using (var shortener = new Google.Apis.Urlshortener.v1.UrlshortenerService(new Google.Apis.Services.BaseClientService.Initializer
				{
					ApplicationName = "MNIT Communication",
					ApiKey = CloudConfigurationManager.GetSetting("GoogleApiKey")
				}))
			{
				var shortUrl = shortener.Url.Insert(new Google.Apis.Urlshortener.v1.Data.Url { LongUrl = cancellationEndpoint }).Execute();

				var twilioSmsPrefix = "Sent from your twilio trial account - ";
				var smsMessage = "MNIT Communication sent this to confirm your number. If you got this by mistake, click " + shortUrl.Id;
				if (twilioSmsPrefix.Length + smsMessage.Length > 160)
				{
					throw new ArgumentException("SMS Message must be less than 120 but it was " + smsMessage.Length.ToString());
				}
				var sms = new Twilio.TwilioRestClient("ACcff9c328336b5cd4c892e9d87905d3d4", CloudConfigurationManager.GetSetting("TwilioPassword"));

				var mobileToSend = validMobile.Contains(mobileNumber) ? mobileNumber : "+61416272575";
				sms.SendSmsMessage("+19073122358", mobileToSend, smsMessage);
			}
		}
	}

}