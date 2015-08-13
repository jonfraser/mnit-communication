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
		public async Task<HttpResponseMessage> RejectMobile(Guid id)
        {
            return await Task.Run(() =>
            {
                //TODO: For the new user registration id passed in, remove the mobile number on the account
                var response = Request.CreateResponse(HttpStatusCode.Found);
			    response.Headers.Location = new Uri(string.Format("{0}://{1}/Account/MobileNumberRemoved", Request.RequestUri.Scheme, Request.RequestUri.Authority));
			    return Task.FromResult(response);
            });
        }
    }
}