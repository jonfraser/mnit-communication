using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNIT_Communication.Domain
{
    public class SubscribeToAlertBrokeredMessage
    {
        public Guid CorrelationId { get; set; }
        public Guid UserId { get; set; }
        public Guid AlertableId { get; set; }
        public string EmailAddress { get; set; }
    }
}
