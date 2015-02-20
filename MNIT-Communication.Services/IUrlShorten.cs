using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNIT_Communication.Services
{
	public interface IUrlShorten
	{
		Task<string> Shorten(string longUrl);
	}
}
