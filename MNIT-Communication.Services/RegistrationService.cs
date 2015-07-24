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
	    private readonly IShortTermStorage store;
	    private readonly ISendEmail mail;
	    private readonly ISendSms sms;
	    private readonly IUrlShorten urlShortener;
	    private readonly IServiceBus serviceBus;

	    public RegistrationService(IShortTermStorage store, ISendEmail mail, ISendSms sms, IUrlShorten urlShortener, IServiceBus serviceBus)
	    {
	        this.store = store;
	        this.mail = mail;
	        this.sms = sms;
	        this.urlShortener = urlShortener;
	        this.serviceBus = serviceBus;
	    }

        public RegistrationService(IShortTermStorage store, ISendEmail mail)
        {
            this.store = store;
            this.mail = mail;
        }

        public RegistrationService(ISendSms sms, IUrlShorten urlShortener)
        {
            this.sms = sms;
            this.urlShortener = urlShortener;
        }

	    public async Task<Guid> SendRegistrationRequest(string email)
		{
			var message = new NewUserRegistrationBrokeredMessage
			{
				CorrelationId = Guid.NewGuid(),
				EmailAddress = email
			};

            await serviceBus.SendToQueueAsync(message, Queues.Registration);

			return message.CorrelationId;
		}

		public async Task ProcessServiceBusRegistrationMessage(NewUserRegistrationBrokeredMessage message)
		{
			await StoreToken(message.EmailAddress, message.CorrelationId);
            await SendEmail(message.EmailAddress, message.CorrelationId);
		}

		private async Task StoreToken(string emailAddress, Guid accessToken)
		{
			await store.StoreKeyValue(emailAddress, accessToken.ToString(), new TimeSpan(72, 0, 0));
		}

		private async Task SendEmail(string email, Guid accessToken)
		{
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
			var message = new VerifyMobileNumberBrokeredMessage
			{
				CorrelationId = Guid.NewGuid(),
				MobileNumber = mobileNumber,
				NewUserRegistrationId = accessToken
			};

            await serviceBus.SendToQueueAsync(message, Queues.MobileNumberVerify);
		}

		public async Task VerifyMobileNumber(string mobileNumber, Guid accessToken)
		{
			var cancellationEndpoint = string.Format("https://mnit-communication.azurewebsites.net/api/User/RejectMobile/{0}", accessToken.ToString());
			var shortEndpoint = await urlShortener.Shorten(cancellationEndpoint);

			var mobileToSend = mobileNumber;
			var smsMessage = "MNIT Communication sent this to confirm your number. Not you? Click " + shortEndpoint + " otherwise delete this message.";

			await sms.SendSimple(mobileNumber, smsMessage);

		}

		public async Task<NewUserProfile> RetrieveNewUserProfile(Guid accessToken)
		{
			var existingRequest = await store.GetValue<NewUserProfile>(accessToken.ToString());
			if (existingRequest != null)
			{
				return existingRequest;
			}
			return new NewUserProfile { NewUserRegistrationId = accessToken };

		}

		public async Task InsertOrUpdateNewUserProfile(NewUserProfile request)
		{
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
			return await store.KeyExists(newRegistrationId.ToString());
		}
	}

}