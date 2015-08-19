using System;
using MNIT_Communication.Domain;
using MNIT_Communication.Services;
using System.Threading.Tasks;
using System.Web.Http;
using MNIT.ErrorLogging;

namespace MNIT_Communication.Areas.api.v1
{
	public partial class UserController : ApiController
	{
		private IUserService userService;
	    private readonly IErrorLogger<Guid> errorLogger;

	    public UserController(IUserService userService, IErrorLogger<Guid> errorLogger)
	    {
	        this.userService = userService;
	        this.errorLogger = errorLogger;
	    }
	}
}