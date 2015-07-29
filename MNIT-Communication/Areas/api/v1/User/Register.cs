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
        public async Task<Guid> Register([FromBody]string emailAddress)
        {
            if (!emailAddress.ToLower().EndsWith("@health.qld.gov.au"))
            {
                throw new NotSupportedException("The email address used must be your health address.");
            }
            return await userService.SendRegistrationRequest(emailAddress);
            
            //TODO: Somehow (maybe in the above service) we need to partially register this user before sending them to the next step

        }

    }
}