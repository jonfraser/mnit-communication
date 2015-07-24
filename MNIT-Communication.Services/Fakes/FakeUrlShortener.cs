using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNIT_Communication.Services.Fakes
{
    public class FakeUrlShortener: IUrlShorten
    {
        public async Task<string> Shorten(string longUrl)
        {
            return await Task.FromResult(longUrl);
        }
    }
}
