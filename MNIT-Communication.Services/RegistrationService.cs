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
using Newtonsoft.Json;
using SendGrid;
using StackExchange.Redis;

namespace MNIT_Communication.Services
{
	public class RegistrationService : IRegistrationService
	{
		private readonly List<string> validEmail = new List<string>(){"jon.fraser@health.qld.gov.au",
												   "ian.missenden@health.qld.gov.au",
												   "anthony.kanowski@health.qld.gov.au"};
		private readonly List<string> validMobile = new List<string>() { "+61416272575" };

		public async Task<Guid> SendRegistrationRequest(string email)
		{
			var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
			var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
			var queueExists = namespaceManager.QueueExists(Queues.Registration);
			if (!queueExists)
			{
				await namespaceManager.CreateQueueAsync(Queues.Registration);
			}

			var client = QueueClient.CreateFromConnectionString(connectionString, Queues.Registration);

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
			IShortTermStorage store = new RedisStore();
			await store.StoreKeyValue(emailAddress, accessToken.ToString(), new TimeSpan(72, 0, 0));
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

		public async Task RequestVerificationOfMobileNumber(string mobileNumber, Guid accessToken)
		{
			var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
			var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
			var queueExists = namespaceManager.QueueExists(Queues.MobileNumberVerify);
			if (!queueExists)
			{
				await namespaceManager.CreateQueueAsync(Queues.MobileNumberVerify);
			}

			var client = QueueClient.CreateFromConnectionString(connectionString, Queues.MobileNumberVerify);

			var message = new VerifyMobileNumberBrokeredMessage
			{
				CorrelationId = Guid.NewGuid(),
				MobileNumber = mobileNumber,
				NewUserRegistrationId = accessToken
			};

			await client.SendAsync(new BrokeredMessage(message));
		}

		public async Task VerifyMobileNumber(string mobileNumber, Guid accessToken)
		{
			var cancellationEndpoint = string.Format("https://mnit-communication.azurewebsites.net/api/User/RejectMobile/{0}", accessToken.ToString());
			using (var shortener = new Google.Apis.Urlshortener.v1.UrlshortenerService(new Google.Apis.Services.BaseClientService.Initializer
				{
					ApplicationName = "MNIT Communication",
					ApiKey = CloudConfigurationManager.GetSetting("GoogleApiKey")
				}))
			{
				var shortUrl = shortener.Url.Insert(new Google.Apis.Urlshortener.v1.Data.Url { LongUrl = cancellationEndpoint }).Execute();
				var mobileToSend = validMobile.Contains(mobileNumber) ? mobileNumber : "+61416272575";
				var smsMessage = "MNIT Communication sent this to confirm your number. If you got this by mistake, click " + shortUrl.Id;

				ISendSms sms = new SendTwilioSmsService();
				await sms.SendSimple(mobileNumber, smsMessage);
			}
		}

		public async Task<NewUserProfile> RetrieveNewUserProfile(Guid accessToken)
		{
			IShortTermStorage store = new RedisStore();
			var existingRequest = await store.GetValue<NewUserProfile>(accessToken.ToString());
			if (existingRequest != null)
			{
				return existingRequest;
			}
			return new NewUserProfile { NewUserRegistrationId = accessToken };

		}

		public async Task InsertOrUpdateNewUserProfile(NewUserProfile request)
		{
			IShortTermStorage store = new RedisStore();
			var existingRequest = await store.GetValue<NewUserProfile>(request.NewUserRegistrationId.ToString());
			if (existingRequest != null)
			{
				existingRequest.ExternalProvider = request.ExternalProvider ?? existingRequest.ExternalProvider;
				existingRequest.EmailAddressExternalProvider = request.EmailAddressExternalProvider ?? existingRequest.EmailAddressExternalProvider;
				existingRequest.EmailAdressInternal = request.EmailAdressInternal ?? existingRequest.EmailAdressInternal;
				existingRequest.MobilePhoneNumber = request.MobilePhoneNumber ?? existingRequest.MobilePhoneNumber;
			}
			var requestForStorage = JsonConvert.SerializeObject(existingRequest ?? request);
			await store.StoreKeyValue(request.NewUserRegistrationId.ToString(), requestForStorage, new TimeSpan(72, 0, 0));
		}


		public async Task<bool> TemporaryAccessTokenExists(Guid newRegistrationId)
		{
			IShortTermStorage store = new RedisStore();
			return await store.KeyExists(newRegistrationId.ToString());
		}
	}

}