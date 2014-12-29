using MNIT_Communication.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MNIT_Communication.Areas.api
{
    public partial class AlertablesController : System.Web.Mvc.Controller
    {
        public System.Web.Mvc.ActionResult Get()
        {
            return Json(new Alertable[] { 
                new Alertable{ID=Guid.NewGuid(), Name="WardView" }
                ,
                new Alertable{ID = Guid.NewGuid(), Name="refer"}
                ,
                new Alertable{ID = Guid.NewGuid(), Name="FOCUS"}
            }, System.Web.Mvc.JsonRequestBehavior.AllowGet);
        }


    }
}