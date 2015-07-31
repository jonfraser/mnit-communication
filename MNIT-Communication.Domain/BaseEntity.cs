using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace MNIT_Communication.Domain
{
    public abstract class BaseEntity
    {
        [BsonId] //MongoDB id specification
        public Guid Id { get; set; }
    }
}
