using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.DynamicData;
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
       

        public async Task<IEnumerable<dynamic>> Get()
        {
            var data = (await GetAlertables());

            var grouped = data.Select(g =>
                new
                {
                    Group = g.Key,
                    Alertables = g.AsEnumerable()
                });

            return grouped;
        }

		public async Task<IEnumerable<Alertable>> Get(string groupName)
		{
		    var alertables = await GetAlertables();

		    var group = alertables.FirstOrDefault(g => g.Key == groupName);
		    if (group != null)
		        return group;

		    return null;
		}

        private async Task<IEnumerable<IGrouping<string, Alertable>>> GetAlertables()
        {
            var alertables = (new List<Alertable>
            { 
                new Alertable{Id=Guid.Parse("440394b8-8b90-46dd-bf13-a420a4358cc2"), Name="WardView", Group = "Applications" }
                ,
                new Alertable{Id = Guid.Parse("de93dcbe-e019-4914-a263-73e5b6bfde5a"), Name="refer", Group = "Applications"}
                ,
                new Alertable{Id = Guid.Parse("658aef09-251f-4578-9ba4-1eae2b0c7b59"), Name="FOCUS", Group = "Applications"}
                ,
                new Alertable{Id = Guid.Parse("3796569b-1275-4097-b341-cdf10177877f"), Name="MCA", Group = "Applications"}
                ,
                new Alertable{Id = Guid.Parse("588bd458-739f-49fe-b578-d558f4e6b410"), Name="MilkBank", Group = "Applications"}
                ,
                new Alertable{Id = Guid.Parse("0b612c88-a251-4914-8f13-262a7d21a3c9"), Name="PaTS", Group = "Applications"}
                ,
                new Alertable{Id = Guid.Parse("e201aa15-d37a-4516-9308-218182de700e"), Name="ADS", Group = "Applications"}
                ,
                new Alertable{Id = Guid.Parse("40b04f7d-eb0b-4694-a425-335a53e875e6"), Name="eDS", Group = "Applications"}
                ,
                new Alertable{Id = Guid.Parse("ccf53d4f-d06d-4d65-823d-b15d7df4c7e1"), Name="SSIS Tool", Group = "Applications"}
                ,
                new Alertable{Id = Guid.Parse("71657700-f95e-4528-b49d-67ed2a827c98"), Name="DUIT Diary", Group = "Applications"}
				,
                new Alertable{Id = Guid.Parse("055C411E-F213-42C1-9B7C-9F3C19EAC1E2"), Name="ePADT", Group = "Interfaces"}
				,
                new Alertable{Id = Guid.Parse("70D74602-FBA7-4B5C-8A83-F67EF89BF223"), Name="EDIS", Group = "Interfaces"}
				,
                new Alertable{Id = Guid.Parse("7027CAAB-A85D-4ED2-AC87-49A03C16B6C5"), Name="Client Directory", Group = "Interfaces"}
				,
                new Alertable{Id = Guid.Parse("50E0AC00-4520-468E-81F3-1591E5A6BA85"), Name="iPharmacy", Group = "Interfaces"}
				,
                new Alertable{Id = Guid.Parse("995CE0C0-038D-45AA-A6FD-4C6481F70ED9"), Name="tpch-p268", Group = "Servers"}
				,
                new Alertable{Id = Guid.Parse("E878A53A-5F50-4E13-9EB2-E3CA80C39EE5"), Name="tpch-p269", Group = "Servers"}
				,
                new Alertable{Id = Guid.Parse("42879978-FEC0-423B-BC03-39C42CCFB6D2"), Name="tpch-p271", Group = "Servers"}
				,
                new Alertable{Id = Guid.Parse("EA21CDDF-9BE0-45EE-9C65-F8F7987F8B42"), Name="Caboolture", Group = "Sites"}
				,
                new Alertable{Id = Guid.Parse("5236CCE5-B335-482E-A617-D93FE8D34D2E"), Name="Redcliffe", Group = "Sites"}
				,
                new Alertable{Id = Guid.Parse("41869C65-17C9-4A5D-BC36-796BBFEFF7FB"), Name="TPCH", Group = "Sites"}
				,
                new Alertable{Id = Guid.Parse("1723E8DE-7B40-460C-B24B-107EDA3DD802"), Name="RBWH", Group = "Sites"}
				,
                new Alertable{Id = Guid.Parse("5D773B68-57AB-4F3D-8E57-AA69FF87C79E"), Name="SaAS", Group = "Sites"}
            });

            return alertables.GroupBy(a => a.Group);

        }

        
    }
}