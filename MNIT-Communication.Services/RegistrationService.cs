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
		public async Task<Guid> SendRegistrationRequest(string email)
		{
			var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
			var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
			var queueExists = await namespaceManager.QueueExistsAsync(Queues.Registration);
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
			var emailToSend = message.EmailAddress;
			await SendEmail(emailToSend, message.CorrelationId);
		}

		private async Task StoreToken(string emailAddress, Guid accessToken)
		{
			IShortTermStorage store = new RedisStore();
			await store.StoreKeyValue(emailAddress, accessToken.ToString(), new TimeSpan(72, 0, 0));
		}

		private async Task SendEmail(string email, Guid accessToken)
		{
			ISendEmail mail = new SendGridEmailService();

			var message = "You've got 72 hours to confirm your account via this link: https://mnit-communication.azurewebsites.net/api/User/Confirm/" + accessToken.ToString()
						+ Environment.NewLine
						+ "If you have already selected your alerts they will be automatically added to your account once it is confirmed.";
			await mail.Send(from: "mnit-communication@health.qld.gov.au",
							to: new List<String> { email },
							subject: "Confirm your MNIT Communication account",
							body: message);

		}

		public async Task RequestVerificationOfMobileNumber(string mobileNumber, Guid accessToken)
		{
			var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
			var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
			var queueExists = await namespaceManager.QueueExistsAsync(Queues.MobileNumberVerify);
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
			IUrlShorten urlShortener = new GoogleUrlShortener();
			var cancellationEndpoint = string.Format("https://mnit-communication.azurewebsites.net/api/User/RejectMobile/{0}", accessToken.ToString());
			var shortEndpoint = await urlShortener.Shorten(cancellationEndpoint);

			var mobileToSend = mobileNumber;
			var smsMessage = "MNIT Communication sent this to confirm your number. Not you? Click " + shortEndpoint + " otherwise delete this message.";

			ISendSms sms = new SendTwilioSmsService();
			await sms.SendSimple(mobileNumber, smsMessage);

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