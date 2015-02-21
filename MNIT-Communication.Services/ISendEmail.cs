using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNIT_Communication.Services
{
	public interface ISendEmail
	{
		Task Send(string from, List<String> to, string subject, string body);
	}
}
