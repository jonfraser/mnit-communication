using System.Web.Mvc;

namespace MNIT_Communication.Areas.api
{
    public class apiAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "api";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            return;
        }
    }
}