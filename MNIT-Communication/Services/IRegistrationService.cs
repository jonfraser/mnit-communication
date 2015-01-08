using System;
using System.Threading.Tasks;
namespace MNIT_Communication.Services
{
	public interface IRegistrationService
	{
		Task ProcessRegistrationRequest(Guid accessToken, string emailAddress);
		Task<Guid> SendRegistrationRequest(string email);
	}
}
