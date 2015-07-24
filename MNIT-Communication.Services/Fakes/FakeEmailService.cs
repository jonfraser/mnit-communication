using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MNIT_Communication.Services.Fakes
{
    public class FakeEmailService: ISendEmail
    {
        public async Task Send(string @from, List<string> to, string subject, string body)
        {
            Debug.WriteLine(string.Join(",", to), "TO");
            Debug.WriteLine(from, "FROM");
            Debug.WriteLine(subject, "SUBJECT");
            Debug.WriteLine(body, "MESSAGE");
        }
    }
}
