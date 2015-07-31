using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNIT_Communication.Domain;
using MongoDB.Bson;
using Xunit;

namespace MNIT_Communication.Tests.Domain
{
    public class BaseEntityTests
    {
        [Fact]
        public void CanSerializeToJsonUsingMongoDbAttributes()
        {
            var id = Guid.NewGuid();
            var entity = new UserProfile
            {
                Id = id,
                EmailAdressInternal = "employee@health.qld.gov.au"
            };

            var json = entity.ToJson();

            Assert.True(json.Contains("_id")); //Id Conversion
        }
    }
}
