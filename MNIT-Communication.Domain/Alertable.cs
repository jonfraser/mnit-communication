using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace MNIT_Communication.Domain
{
    public class Alertable: BaseEntity
    {
        public string Name { get; set; }
        public string Group { get; set; }
        
    }
}
