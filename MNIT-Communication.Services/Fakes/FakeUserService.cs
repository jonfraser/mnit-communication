using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MNIT_Communication.Domain;

namespace MNIT_Communication.Services.Fakes
{
	public class FakeUserService : IUserService
	{
		public async System.Threading.Tasks.Task<Guid> SendRegistrationRequest(string email)
		{
			Trace.Write("FakeRegistrationService.SendRegistrationRequest " + email);
			return await Task.Run(() => Guid.NewGuid());
		}


		public async Task ProcessServiceBusRegistrationMessage(Domain.NewUserRegistrationBrokeredMessage message)
		{
			Trace.Write("FakeRegistrationService.ProcessServiceBusRegistrationMessage " + message.CorrelationId);
			return;
		}

		public async Task VerifyMobileNumber(string mobileNumber, Guid accessToken)
		{
			Trace.Write("FakeRegistrationService.VerifyMobileNumber " + mobileNumber);
			return;
		}

		public async Task RequestVerificationOfMobileNumber(string mobileNumber, Guid accessToken)
		{
			Trace.Write("FakeRegistrationService.RequestVerificationOfMobileNumber " + mobileNumber);
			return;
		}
        
		public async Task<UserProfile> RetrieveUserProfile(Guid accessToken)
		{
			Trace.Write("FakeRegistrationService.RetrieveUserProfile");
			return new UserProfile
			{
				Id = accessToken,
				EmailAddressExternalProvider = "fraser.jc@gmail.com",
				EmailAdressInternal = "jon.fraser@health.qld.gov.au",
				MobilePhoneNumber = "+61416272575",
                Confirmed = false
			};
		}

        public async Task<UserProfile> RetrieveUserProfile(Func<UserProfile, bool> predicate)
        {
            return await RetrieveUserProfile(Guid.NewGuid());
        }
		
		public async Task InsertOrUpdateUserProfile(UserProfile request)
		{
			Trace.Write("FakeRegistrationService.UpdateUserProfile " + request.Id.ToString());
			return;
		}


		public async Task<bool> TemporaryAccessTokenExists(Guid newRegistrationIdFromReturnUrl)
		{
			Trace.Write("FakeRegistrationService.TemporaryAccessTokenExists " + newRegistrationIdFromReturnUrl.ToString());
			return true;
		}


       
    }
}