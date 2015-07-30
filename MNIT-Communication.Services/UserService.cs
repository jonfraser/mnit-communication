using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
	public class UserService : IUserService
	{
	    private readonly IRepository repository;
	    private readonly IShortTermStorage store;
	    private readonly ISendEmail mail;
	    private readonly ISendSms sms;
	    private readonly IUrlShorten urlShortener;
	    private readonly IServiceBus serviceBus;

	    public UserService(IRepository repository, IShortTermStorage store, ISendEmail mail, ISendSms sms, IUrlShorten urlShortener, IServiceBus serviceBus)
	    {
	        this.repository = repository;
	        this.store = store;
	        this.mail = mail;
	        this.sms = sms;
	        this.urlShortener = urlShortener;
	        this.serviceBus = serviceBus;
	    }

        public UserService(IShortTermStorage store, ISendEmail mail)
        {
            this.store = store;
            this.mail = mail;
        }

        public UserService(ISendSms sms, IUrlShorten urlShortener)
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
		    var newUser = new UserProfile
		    {
		        Id = message.CorrelationId,
		        EmailAdressInternal = message.EmailAddress,
                Confirmed = false
		    };

		    await InsertOrUpdateUserProfile(newUser);
            await SendEmail(newUser);
		}

		private async Task SendEmail(UserProfile newUser)
		{
			var message = "You've got 72 hours to confirm your account via this link: https://mnit-communication.azurewebsites.net/api/User/Confirm/" + newUser.Id.ToString()
						+ Environment.NewLine
						+ "If you have already selected your alerts they will be automatically added to your account once it is confirmed.";
			await mail.Send(from: "mnit-communication@health.qld.gov.au",
							to: new List<String> { newUser.EmailAdressInternal },
							subject: "Confirm your MNIT Communication account",
							body: message);

		}

		public async Task RequestVerificationOfMobileNumber(string mobileNumber, Guid id)
		{
			var message = new VerifyMobileNumberBrokeredMessage
			{
				CorrelationId = Guid.NewGuid(),
				MobileNumber = mobileNumber,
				NewUserRegistrationId = id
			};

            await serviceBus.SendToQueueAsync(message, Queues.MobileNumberVerify);
		}

		public async Task VerifyMobileNumber(string mobileNumber, Guid id)
		{
			var cancellationEndpoint = string.Format("https://mnit-communication.azurewebsites.net/api/User/RejectMobile/{0}", id.ToString());
			var shortEndpoint = await urlShortener.Shorten(cancellationEndpoint);

			var mobileToSend = mobileNumber;
			var smsMessage = "MNIT Communication sent this to confirm your number. Not you? Click " + shortEndpoint + " otherwise delete this message.";

			await sms.SendSimple(mobileNumber, smsMessage);

		}

		public async Task<UserProfile> RetrieveUserProfile(Guid id)
		{
			var registeredUser = await repository.Get<UserProfile>(id);
            if (registeredUser != null)
			{
                //TODO - Cache this value
                return registeredUser;
			}

            //Not found, look in temp store
            var newUser = await store.GetValue<UserProfile>(id.ToString());
            
            //If still not found. Return new Profile
            return newUser ?? new UserProfile { Id = id };
		}

        //WARNING - This method only returns persisted (i.e. Confirmed) UserProfiles
	    public async Task<UserProfile> RetrieveUserProfile(Func<UserProfile, bool> predicate)
	    {
            //TODO - Cache this value
            return (await repository.Get(predicate)).FirstOrDefault();
	    }

        //WARNING - This method only returns persisted (i.e. Confirmed) UserProfiles
	    public async Task<UserProfile> RetrieveUserProfileByExternalId(string externalId)
	    {
            return await RetrieveUserProfile(up => up.ExternalId == externalId);
	    }

		public async Task InsertOrUpdateUserProfile(UserProfile profile)
		{
            var toPersist = await RetrieveUserProfile(profile.Id);

            toPersist.ExternalProvider = profile.ExternalProvider ?? toPersist.ExternalProvider;
		    toPersist.ExternalId = profile.ExternalId ?? toPersist.ExternalId;
            toPersist.EmailAddressExternalProvider = profile.EmailAddressExternalProvider ?? toPersist.EmailAddressExternalProvider;
            toPersist.EmailAdressInternal = profile.EmailAdressInternal ?? toPersist.EmailAdressInternal;
            toPersist.MobilePhoneNumber = profile.MobilePhoneNumber ?? toPersist.MobilePhoneNumber;
		    toPersist.Confirmed = profile.Confirmed;
		    toPersist.AlertSubscriptions = profile.AlertSubscriptions ?? toPersist.AlertSubscriptions;

		    if (!toPersist.Confirmed)
		    {
		        //TODO - recalculate the lifespan? i.e. if there's only 24 hours left of the original 72, is this possible?
                await store.StoreValue(profile.Id.ToString(), toPersist, new TimeSpan(72, 0, 0));
		    }
		    else
		    {
                //TODO - Delete cached value if any
                //TODO - Check that the same ExternalId has not already been used for a Profile? (Someone could register twice) If so, delete and re-insert, or update original?
                await repository.Upsert(toPersist);
		    }
		}


		public async Task<bool> TemporaryAccessTokenExists(Guid id)
		{
			return await store.KeyExists(id.ToString());
		}
	}

}