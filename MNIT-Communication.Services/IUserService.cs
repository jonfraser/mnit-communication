﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        Task<UserProfile> RetrieveUserProfile(Expression<Func<UserProfile, bool>> predicate);

	    Task<UserProfile> RetrieveUserProfileByExternalId(string externalId);

		Task InsertOrUpdateUserProfile(UserProfile profile);
		
		Task<bool> TemporaryAccessTokenExists(Guid id);

        Task<IList<UserProfile>> ListAdministrators();

	    Task<bool> RequestAdmin(Guid userId, Guid administratorId);

        Task<bool> GrantAdmin(Guid userId, Guid administratorId);
	}

}
