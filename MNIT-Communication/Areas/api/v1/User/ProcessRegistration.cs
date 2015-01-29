using MNIT_Communication.Domain;
using MNIT_Communication.Services;
using System.Threading.Tasks;
using System.Web.Http;
using System.Diagnostics;
using System;

namespace MNIT_Communication.Areas.api.v1
{
	public partial class UserController : ApiController
	{
		[HttpPost]
		public async Task ProcessRegistration(NewUserRegistrationBrokeredMessage newUserRegistrationBrokeredMessage)
		{
			//todo: both these are null but the object itself doesn't seem to be
			EventLog.WriteEntry("Application", "CorrelationID: " + newUserRegistrationBrokeredMessage.CorrelationId.ToString(), EventLogEntryType.Information);
			EventLog.WriteEntry("Application", "Email: " + newUserRegistrationBrokeredMessage.EmailAddress, EventLogEntryType.Information);

			try
			{
				await registrationService.ProcessRegistrationRequest(newUserRegistrationBrokeredMessage.CorrelationId, newUserRegistrationBrokeredMessage.EmailAddress);
			}
			catch (Exception ex)
			{
				EventLog.WriteEntry("Application", ex.ToString(), EventLogEntryType.Error);
				throw;
			}
		}


	}
}