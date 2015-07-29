using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MNIT_Communication.Domain
{
    public class UserProfile: BaseEntity
	{
        public string EmailAdressInternal { get; set; }
		public string ExternalProvider { get; set; }
        public string ExternalId { get; set; }
		public string EmailAddressExternalProvider { get; set; }
		public string MobilePhoneNumber { get; set; }
        public bool Confirmed { get; set; }

        private IList<Guid> alertSubscriptions = new List<Guid>();
        
        public IList<Guid> AlertSubscriptions
        {
            get { return alertSubscriptions; }
            set { alertSubscriptions = value; }
        }
	}
}
