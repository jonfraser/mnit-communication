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
        // GET api/<controller>
        [HttpGet]
		public async Task<HttpResponseMessage> Confirm(Guid id)
        {
            var profile = await userService.RetrieveUserProfile(id);
            profile.Confirmed = true;
            await userService.InsertOrUpdateUserProfile(profile);
            
            var response = Request.CreateResponse(HttpStatusCode.Found);
            response.Headers.Location = new Uri(string.Format("{0}://{1}/Home/Confirmed", Request.RequestUri.Scheme, Request.RequestUri.Authority));
            return response;

        }

    }
}