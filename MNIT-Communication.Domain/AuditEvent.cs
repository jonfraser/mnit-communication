using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNIT_Communication.Domain
{
    public partial class AuditEvent: BaseEntity
    {
        public AuditType AuditType { get; set; }
        public string EntityType { get; set; }
        public Guid? EntityId { get; set; }
        public dynamic Data { get; set; }
        public DateTime DateTimeStamp { get; set; }
        public string IpAddress { get; set; }
        public string Details { get; set; }
        public Guid? ChangedById { get; set; }
    }
}
