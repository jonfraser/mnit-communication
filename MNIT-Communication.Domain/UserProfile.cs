using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNIT_Communication.Domain
{
	public class NewUserProfile : UserProfile
	{
		public Guid NewUserRegistrationId { get; set; }
	}

	public class UserProfile
	{
		public int Id { get; set; }
		public string EmailAdressInternal { get; set; }
		public string EmailAddressExternalProvider { get; set; }
		public string MobilePhoneNumber { get; set; }
	}
}
