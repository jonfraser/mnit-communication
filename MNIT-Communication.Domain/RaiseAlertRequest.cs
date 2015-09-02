using System;
using System.Collections.Generic;

namespace MNIT_Communication.Domain
{
    public class RaiseAlertRequest
    {
        public List<Alertable> Alertables { get; set; }
        public string AlertDetail { get; set; }
        public string AlertInfoShort { get; set; }
        public UserProfile RaisedBy { get; set; }
        public DateTime? Start { get; set; }
        public bool Scheduled { get { return Start.HasValue; } }
        public DateTime? ExpectedFinish { get; set; }
    }
}