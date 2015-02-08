using MNIT_Communication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MNIT_Communication.Areas.api.v1
{
    public partial class UserController : ApiController
    {
		
        [HttpPost]
        public async Task UpdateProfile([FromBody]UpdateProfileRequest request)
        {
			if(!string.IsNullOrEmpty(request.mobile))
			{
			await registrationService.RequestVerificationOfMobileNumber(request.mobile, request.newUserRegistrationId);
			}
			//TODO: actually store these details
		}

		public class UpdateProfileRequest
		{
			public Guid newUserRegistrationId { get; set; }
			public string email { get; set; }
			public string mobile { get; set; }
		}
    }
}