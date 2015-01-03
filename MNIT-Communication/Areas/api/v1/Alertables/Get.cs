using MNIT_Communication.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MNIT_Communication.Areas.api.v1
{
    public partial class AlertablesController : ApiController
    {
        public IEnumerable<Alertable> Get()
        {
            return (new Alertable[] { 
                new Alertable{ID=Guid.Parse("440394b8-8b90-46dd-bf13-a420a4358cc2"), Name="WardView" }
                ,
                new Alertable{ID = Guid.Parse("de93dcbe-e019-4914-a263-73e5b6bfde5a"), Name="refer"}
                ,
                new Alertable{ID = Guid.Parse("658aef09-251f-4578-9ba4-1eae2b0c7b59"), Name="FOCUS"}
                ,
                new Alertable{ID = Guid.Parse("3796569b-1275-4097-b341-cdf10177877f"), Name="MCA"}
                ,
                new Alertable{ID = Guid.Parse("588bd458-739f-49fe-b578-d558f4e6b410"), Name="MilkBank"}
                ,
                new Alertable{ID = Guid.Parse("0b612c88-a251-4914-8f13-262a7d21a3c9"), Name="PaTS"}
                ,
                new Alertable{ID = Guid.Parse("e201aa15-d37a-4516-9308-218182de700e"), Name="ADS"}
                ,
                new Alertable{ID = Guid.Parse("40b04f7d-eb0b-4694-a425-335a53e875e6"), Name="eDS"}
                ,
                new Alertable{ID = Guid.Parse("ccf53d4f-d06d-4d65-823d-b15d7df4c7e1"), Name="SSIS Tool"}
                ,
                new Alertable{ID = Guid.Parse("71657700-f95e-4528-b49d-67ed2a827c98"), Name="DUIT Diary"}
            });
        }

    }
}