using System;
using System.Collections.Generic;

namespace MNIT_Communication.Domain
{
    public class RaiseAlertRequest
    {
        public List<Alertable> Alertables { get; set; }
        public string AlertDetail { get; set; }
        public string AlertInfoShort { get; set; }
        public Guid RaisedById { get; set; }
    }
}