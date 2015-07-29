﻿using System;
using System.Threading.Tasks;
using MNIT_Communication.Domain;
namespace MNIT_Communication.Services
{
	public interface IUserService
	{
		Task<Guid> SendRegistrationRequest(string email);
		
        Task ProcessServiceBusRegistrationMessage(NewUserRegistrationBrokeredMessage message);

		Task RequestVerificationOfMobileNumber(string mobileNumber, Guid id);

		Task VerifyMobileNumber(string mobileNumber, Guid id);

		Task<UserProfile> RetrieveUserProfile(Guid id);

        Task<UserProfile> RetrieveUserProfile(Func<UserProfile, bool> predicate);

		Task InsertOrUpdateUserProfile(UserProfile profile);
		
		Task<bool> TemporaryAccessTokenExists(Guid id);
	}

}
