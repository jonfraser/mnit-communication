using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using MNIT_Communication.Services;

namespace MNIT_Communication.Attributes
{
    public class UserProfileConfirmedAttribute : AuthorizeAttribute
    {
        protected IRuntimeContext runtimeContext;

        public UserProfileConfirmedAttribute()
        {
        }
        
        public UserProfileConfirmedAttribute(IRuntimeContext runtimeContext)
        {
            this.runtimeContext = runtimeContext;
        }

        public IRuntimeContext RuntimeContext
        {
            get { return runtimeContext; }
            set { runtimeContext = value; }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //First, check that we have authenticated the user
            var authenticated = base.AuthorizeCore(httpContext);
            
            if (!authenticated)
                return false;

            var hasProfile = Task.Run(() => runtimeContext.HasProfile()).Result;
            return hasProfile;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new {controller = "Account", action = "Unconfirmed"}));
        } 
       
    }
}