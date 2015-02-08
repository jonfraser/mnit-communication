using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNIT_Communication.Domain
{
	public class VerifyMobileNumberBrokeredMessage
	{
		public Guid CorrelationId { get; set; }
		public Guid NewUserRegistrationId { get; set; }
		public string MobileNumber { get; set; }
	}
}
