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
        public async Task<HttpResponseMessage> Confirm(Guid newUserRegistrationId)
        {
            var response = Request.CreateResponse(HttpStatusCode.Found);
            response.Headers.Location = new Uri(Request.RequestUri.AbsolutePath + "/Home/Confirmed");
            return response;

        }

    }
}