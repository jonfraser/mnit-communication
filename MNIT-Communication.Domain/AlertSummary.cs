using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNIT_Communication.Domain
{
	public class AlertSummary
	{
		public string Service { get; set; }
		public DateTime Start { get; set; }
		public string Update { get; set; }
		public DateTime UpdateDate { get; set; }
	}
}
