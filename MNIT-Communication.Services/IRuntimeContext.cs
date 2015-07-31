using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MNIT_Communication.Domain;

namespace MNIT_Communication.Services
{
    public interface IRuntimeContext
    {
        ClaimsPrincipal CurrentPrincipal { get; }
        Task<UserProfile> CurrentProfile();
        Task<bool> HasProfile();
    }
}
