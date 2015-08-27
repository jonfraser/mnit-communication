using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security.Provider;
using MNIT_Communication.Domain;
using MNIT_Communication.Services;

namespace MNIT_Communication.Helpers
{
    public class AspNetRuntimeContext: IRuntimeContext
    {
        private readonly IUserService userService;

        public AspNetRuntimeContext(IUserService userService)
        {
            this.userService = userService;
        }

        public ClaimsPrincipal CurrentPrincipal
        {
            get { return System.Threading.Thread.CurrentPrincipal as ClaimsPrincipal; }
        }

        public async Task<UserProfile> CurrentProfile()
        {
            if (CurrentPrincipal == null || CurrentPrincipal.Identity.IsAuthenticated == false)
                return null;

            var externalId = CurrentPrincipal.GetClaimValue(ClaimTypes.NameIdentifier);

            var profile = await userService.RetrieveUserProfileByExternalId(externalId);

            return profile;
        }

        public async Task<bool> HasProfile()
        {
             return (await CurrentProfile()) != null; 
        }
        public string UserHostAddress
        {
            get
            {
                if (HttpContext.Current != null)
                    return HttpContext.Current.Request.UserHostAddress;

                return string.Empty;
            }
        }
    }
}