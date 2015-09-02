using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNIT_Communication.Domain
{
    public class AlertStatus
    {
        public static AlertStatus Scheduled = new AlertStatus { Name = "Scheduled", Order = 0};
        public static AlertStatus Raised = new AlertStatus {Name = "Raised", Order = 1};
        public static AlertStatus Updated = new AlertStatus {Name = "Updated", Order = 2};
        public static AlertStatus Resolved = new AlertStatus {Name = "Resolved", Order = 3};
        public static AlertStatus Cancelled = new AlertStatus {Name = "Cancelled", Order = 4};

        public string Name { get; set; }
        public int Order { get; private set; }

        protected bool Equals(AlertStatus other)
        {
            return string.Equals(Name, other.Name);
        }
        public override bool Equals(object obj)
        {
            var compareTo = obj as AlertStatus;
            if (compareTo == null)
                return false;

            return Name == compareTo.Name;
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}
