using MNIT_Communication.Domain;
using MNIT_Communication.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace MNIT_Communication.Areas.api.v1
{
	public partial class UserController : ApiController
	{
		private IRegistrationService registrationService;
        public UserController(IRegistrationService registrationService)
		{
            this.registrationService = registrationService;
		}
		
	}
}