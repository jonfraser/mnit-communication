using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNIT_Communication.Domain
{
	public static class Queues
	{
		public const string Registration = "RegistrationQueue";
		public const string AlertsRegistration = "AlertsRegistrationQueue";
		public const string MobileNumberVerify = "MobileNumberVerify";
	}

	public static class Topics
	{
		public const string Alerts = "AlertsTopic";
	}
}
