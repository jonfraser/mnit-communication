using System;

namespace MNIT_Communication.Domain
{
    public class UpdateAlertRequest
    {
        public Guid AlertId { get; set; }
        public AlertHistory Update { get; set; }
        public DateTime? ExpectedFinish { get; set; }
    }
}