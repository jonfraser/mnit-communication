using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNIT_Communication.Services.Fakes
{
    public class FakeSmsService: ISendSms
    {
        public async Task SendSimple(string mobileNumber, string message)
        {
            Debug.WriteLine("-------SMS SENT---------");
            Debug.WriteLine(mobileNumber,"TO");
            Debug.WriteLine(message, "MESSAGE");
            Debug.WriteLine("-------END SMS---------");
        }
    }
}
