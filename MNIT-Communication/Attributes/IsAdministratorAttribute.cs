using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MNIT_Communication.Services;

namespace MNIT_Communication.Attributes
{
    public class IsAdministratorAttribute : UserProfileConfirmedAttribute
    {
        public IsAdministratorAttribute()
        {
        }

        public IsAdministratorAttribute(IRuntimeContext runtimeContext): base(runtimeContext)
        {
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //First, check that user has a Confirmed Profile
            var confirmed = base.AuthorizeCore(httpContext);

            if (!confirmed)
                return false;

            var isAdmin = Task.Run(() => runtimeContext.CurrentProfile()).Result.IsAdmin;
            return isAdmin;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "RequestAdmin" }));
        } 
    }
}