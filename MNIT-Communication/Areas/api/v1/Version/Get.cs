using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MNIT_Communication.Areas.api.v1.Version
{
    public partial class VersionController : System.Web.Mvc.Controller
    {
        public string Get()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().FullName;
        }

    }
}