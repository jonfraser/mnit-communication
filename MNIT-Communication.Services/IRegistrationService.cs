using System;
using System.Threading.Tasks;
using MNIT_Communication.Domain;
namespace MNIT_Communication.Services
{
	public interface IRegistrationService
	{
		Task ProcessRegistrationRequest(Guid accessToken, string emailAddress);
		Task<Guid> SendRegistrationRequest(string email);
		Task ProcessServiceBusRegistrationMessage(string baseWebUrl, NewUserRegistrationBrokeredMessage message);
		
	}

}
