using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        public UserProfile CurrentProfile
        {
            get
            {
                if (CurrentPrincipal == null)
                    return null;

                var externalId = CurrentPrincipal.GetClaimValue(ClaimTypes.NameIdentifier);

                var profile = userService.RetrieveUserProfileByExternalId(externalId).Result;

                return profile;
            }
        }

        public bool HasProfile
        {
            get { return CurrentProfile != null; }
        }
    }
}