using System;
using System.Collections.Generic;

namespace MNIT_Communication.Domain
{
    public class AlertsSubscriptionRequest
    {
        public Guid userId { get; set; }
        public IEnumerable<Guid> alertables { get; set; }
    }
}