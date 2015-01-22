using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace MNIT_Communication.Areas.api
{
	public class BaseExceptionFilterAttribute : ExceptionFilterAttribute
	{
		public override void OnException(HttpActionExecutedContext actionExecutedContext)
		{
			//TODO: Log exception
			base.OnException(actionExecutedContext);
		}

		public override Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
		{
			//TODO: Log exception
			return base.OnExceptionAsync(actionExecutedContext, cancellationToken);
		}
	}
}