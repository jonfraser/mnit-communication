using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using MNIT_Communication.Domain;

namespace MNIT_Communication.Services
{
    public class WebJobRuntimeContext: IRuntimeContext
    {
        public ClaimsPrincipal CurrentPrincipal
        {
            get
            {
                return new GenericPrincipal(new ClaimsIdentity(), new string[]{});
            }
        }

        public Task<UserProfile> CurrentProfile()
        {
            var nullProfile = new UserProfile
            {
                Id = Guid.Empty
            };

            return Task.FromResult(nullProfile);
        }

        public Task<bool> HasProfile()
        {
            return Task.FromResult(false);
        }

        public string UserHostAddress
        {
            get
            {
                return string.Empty;
            }
        }
    }
}
