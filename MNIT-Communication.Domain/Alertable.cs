using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNIT_Communication.Domain
{
    public class Alertable
    {
        public Guid ID { get; set; }
        public string Name { get; set; }

		public string Group { get; set; }
    }
}
