using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MNIT_Communication.Services;
using Xunit;

namespace MNIT_Communication.Tests.Services
{
    public class SendTelstraSmsServiceTests
	{
        //[Fact, TestCategory("Integration")]
        //public async Task CanSendSmsThroughTelstra()
        //{
        //    var sms = new SendTelstraSmsService();
        //    await sms.SendSimple("0466646967", "Love you! This is what I'm playing with :p");
        //}

        [Fact, TestCategory("Integration")]
        public async Task CanSendSmsThroughTelstra()
        {
            var sms = new SendTelstraSmsService();
            await sms.SendSimple("0400099743", "Test Text Messaiag via Telstra API :p");
        }
	}
}
