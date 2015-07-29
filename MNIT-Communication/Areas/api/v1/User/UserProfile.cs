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
		[Route("api/User/UserProfile")]
		public async Task<UserProfile> UserProfileGet([FromUri] Guid id)
		{
			return await userService.RetrieveUserProfile(id);
		}

		[HttpPut]
		[Route("api/User/UserProfile")]
        public async Task UserProfileUpdate([FromBody]UserProfile userProfile)
        {
			//TODO: probably some validation? or a global handler for 400 bad request?
            await userService.InsertOrUpdateUserProfile(userProfile);
		}

        [HttpPost]
		[Route("api/User/UserProfile")]
        public async Task UserProfileInsert([FromBody]UserProfile userProfile)
		{
            //TODO: only run mobile check if number has changed
            if (!string.IsNullOrEmpty(userProfile.MobilePhoneNumber))
			{
                await userService.RequestVerificationOfMobileNumber(userProfile.MobilePhoneNumber, userProfile.Id);
			}

            await userService.InsertOrUpdateUserProfile(userProfile);
		}
    }
}