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
        public async Task<UserProfile> Get(Guid id)
        {
			return await userService.RetrieveUserProfile(id);
		}

    }
}