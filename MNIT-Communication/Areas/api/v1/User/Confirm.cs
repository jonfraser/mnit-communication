using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MNIT.ErrorLogging;
using MNIT_Communication.Domain;

namespace MNIT_Communication.Areas.api.v1
{
    public partial class UserController : ApiController
    { 

        // GET api/<controller>
        [HttpGet]
        public async Task<HttpResponseMessage> Confirm(Guid id, Guid secret)
        {
            HttpResponseMessage response = null;

            var profile = await userService.RetrieveUserProfile(id);

            if (profile == null || profile.ConfirmationSecret != secret)
            {
                var exception = new ArgumentException("Non-existent user of an invalid secret was submitted for Confirmation");
                await errorLogger.LogErrorAsync(exception);

                response = Request.CreateResponse(HttpStatusCode.Found);
                response.Headers.Location = new Uri(string.Format("{0}://{1}/Account/ConfirmationFailed", Request.RequestUri.Scheme, Request.RequestUri.Authority));
                return response;
            }

            profile.Confirmed = true;
            await userService.InsertOrUpdateUserProfile(profile);

            auditService.LogAuditEvent(new AuditEvent
            {
                AuditType = AuditType.UserConfirmed,
                EntityType = typeof(UserProfile).Name,
                EntityId = profile.Id
            });

            response = Request.CreateResponse(HttpStatusCode.Found);
            response.Headers.Location = new Uri(string.Format("{0}://{1}/Account/Confirmed", Request.RequestUri.Scheme, Request.RequestUri.Authority));
            return response;
        }
    }
}