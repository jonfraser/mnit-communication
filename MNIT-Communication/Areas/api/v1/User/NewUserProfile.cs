using MNIT_Communication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MNIT_Communication.Domain;

namespace MNIT_Communication.Areas.api.v1
{
    public partial class UserController : ApiController
    {
		[HttpGet]
		[Route("api/User/NewUserProfile")]
		public async Task<NewUserProfile> NewUserProfileGet(Guid newUserRegistrationId)
		{
			return await registrationService.RetrieveNewUserProfile(newUserRegistrationId) as NewUserProfile;
		}

		[HttpPut]
		[Route("api/User/NewUserProfile")]
        public async Task NewUserProfileUpdate([FromBody]NewUserProfile request)
        {
			//TODO: probably some validation? or a global handler for 400 bad request?
			await registrationService.InsertOrUpdateNewUserProfile(request);
		}

        [HttpPost]
		[Route("api/User/NewUserProfile")]
		public async Task NewUserProfileInsert([FromBody]NewUserProfile request)
		{
			//todo: only run mobile check if number has changed
			if (!string.IsNullOrEmpty(request.MobilePhoneNumber))
			{
				await registrationService.RequestVerificationOfMobileNumber(request.MobilePhoneNumber, request.NewUserRegistrationId);
			}
			await registrationService.InsertOrUpdateNewUserProfile(request);
		}
    }
}