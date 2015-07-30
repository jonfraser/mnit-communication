using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace MNIT_Communication.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static Claim GetClaim(this ClaimsPrincipal principal, string claimType)
        {
            if (principal.HasClaim(c => c.Type == claimType))
            {
                return principal.Claims.FirstOrDefault(c => c.Type == claimType);
            }

            //else
            return null;
        }

        public static string GetClaimValue(this ClaimsPrincipal principal, string claimType)
        {
            var claim = principal.GetClaim(claimType);
            
            if(claim == null)
                return null;
            
            //else
            return claim.Value;
        }
    }
}