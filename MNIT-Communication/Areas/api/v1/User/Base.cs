using MNIT_Communication.Domain;
using MNIT_Communication.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace MNIT_Communication.Areas.api.v1
{
	public partial class UserController : ApiController
	{
		private IUserService userService;
        public UserController(IUserService userService)
		{
            this.userService = userService;
		}
		
	}
}