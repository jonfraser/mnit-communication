using System;
using System.Diagnostics;
using System.Linq.Expressions;
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
            await Task.Run(() => Trace.Write("FakeRegistrationService.ProcessServiceBusRegistrationMessage " + message.CorrelationId));
        }

		public async Task VerifyMobileNumber(string mobileNumber, Guid accessToken)
		{
            await Task.Run(() => Trace.Write("FakeRegistrationService.VerifyMobileNumber " + mobileNumber)); 
		}

		public async Task RequestVerificationOfMobileNumber(string mobileNumber, Guid accessToken)
		{
            await Task.Run(() => Trace.Write("FakeRegistrationService.RequestVerificationOfMobileNumber " + mobileNumber));
            
		}
        
		public async Task<UserProfile> RetrieveUserProfile(Guid accessToken)
		{
            return await Task.Run(() =>
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
            });
		}

        public async Task<UserProfile> RetrieveUserProfile(Expression<Func<UserProfile, bool>> predicate)
        {
            return await RetrieveUserProfile(Guid.NewGuid());
        }

	    public async Task<UserProfile> RetrieveUserProfileByExternalId(string externalId)
	    {
            return await RetrieveUserProfile(Guid.NewGuid());
	    }

	    public async Task InsertOrUpdateUserProfile(UserProfile request)
		{
            await Task.Run(() => Trace.Write("FakeRegistrationService.UpdateUserProfile " + request.Id.ToString()));
            
		}


		public async Task<bool> TemporaryAccessTokenExists(Guid newRegistrationIdFromReturnUrl)
		{
            return await Task.Run(() =>
            {
                Trace.Write("FakeRegistrationService.TemporaryAccessTokenExists " + newRegistrationIdFromReturnUrl.ToString());
			    return true;
            });
		}

        public Task<System.Collections.Generic.IList<UserProfile>> ListAdministrators()
        {
            throw new NotImplementedException();
        }

	    public Task<bool> RequestAdmin(Guid userId, Guid administratorId)
	    {
	        throw new NotImplementedException();
	    }

	    public Task<bool> GrantAdmin(Guid userId, Guid administratorId)
	    {
	        throw new NotImplementedException();
	    }
	}
}