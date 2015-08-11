using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MNIT_Communication.Domain;
using MNIT_Communication.Models;
using MNIT_Communication.Services;

namespace MNIT_Communication.Areas.api.v1
{
    public partial class UserController : ApiController
    {
        // GET api/<controller>
        [HttpPost]
        public async Task<bool> RequestAdmin(AdminRequest request)
        {
            return await userService.RequestAdmin(request.UserId, request.AdministratorId);
        }
    }

    
}