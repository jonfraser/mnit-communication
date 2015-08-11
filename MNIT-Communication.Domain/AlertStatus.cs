using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNIT_Communication.Domain
{
    public class AlertStatus
    {
        protected bool Equals(AlertStatus other)
        {
            return string.Equals(Name, other.Name);
        }

        public static AlertStatus Raised = new AlertStatus {Name = "Raised"};
        public static AlertStatus Updated = new AlertStatus {Name = "Updated"};
        public static AlertStatus Cancelled = new AlertStatus {Name = "Cancelled"};
        public static AlertStatus Resolved = new AlertStatus {Name = "Resolved"};

        public string Name { get; set; }

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
