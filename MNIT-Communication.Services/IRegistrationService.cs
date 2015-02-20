using System;
using System.Threading.Tasks;
using MNIT_Communication.Domain;
namespace MNIT_Communication.Services
{
	public interface IRegistrationService
	{
		Task<Guid> SendRegistrationRequest(string email);
		Task ProcessServiceBusRegistrationMessage(NewUserRegistrationBrokeredMessage message);

		Task RequestVerificationOfMobileNumber(string mobileNumber, Guid accessToken);

		Task VerifyMobileNumber(string mobileNumber, Guid accessToken);

		Task<NewUserProfile> RetrieveNewUserProfile(Guid id);

		Task InsertOrUpdateNewUserProfile(NewUserProfile request);
		
		Task<bool> TemporaryAccessTokenExists(Guid newRegistrationIdFromReturnUrl);
	}

}
