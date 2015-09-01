using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNIT_Communication.Domain
{
	public class AlertBrokeredMessage
	{
		public Guid CorrelationId { get; set; }
        public Guid AlertableId { get; set; }
	    public AlertStatus AlertStatus { get; set; }
		public string AlertDetail { get; set; }
		public string AlertInfoShort { get; set; }
		public UserProfile AlertRaiser { get; set; }
	}
}
