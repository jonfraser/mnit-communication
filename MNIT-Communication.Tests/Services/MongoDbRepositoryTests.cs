using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNIT_Communication.Domain;
using MNIT_Communication.Services;
using Xunit;

namespace MNIT_Communication.Tests.Services
{
    public class MongoDbRepositoryTests
    {
        /// <summary>
        /// This test connects to a locally running MongoDB Server instance as defined in the app.config
        /// </summary>
        [Fact, 
        Microsoft.VisualStudio.TestTools.UnitTesting.TestCategory("Integration")]
        public void CanContruct()
        {
            var repo = new MongoDbRepository();
        }

        [Fact, 
        Microsoft.VisualStudio.TestTools.UnitTesting.TestCategory("Integration")]
        public async Task CanInsertAnItemIntoCollection()
        {
            var repo = new MongoDbRepository();

            var id = Guid.NewGuid();
            var newProfile = new UserProfile
            {
                Id = id,
                EmailAdressInternal = "employee@health.qld.gov.au",
                AlertSubscriptions = new List<Guid>
                {
                    Guid.NewGuid(),
                    Guid.NewGuid()
                }
            };

            var stored = await repo.Upsert(newProfile);

            var found = await repo.Get<UserProfile>(stored.Id);

            Assert.NotNull(found);
        }

        [Fact,
        Microsoft.VisualStudio.TestTools.UnitTesting.TestCategory("Integration")]
        public async Task CanUpdateAnItemInCollection()
        {
            var repo = new MongoDbRepository();

            var id = Guid.NewGuid();
            var newProfile = new UserProfile
            {
                Id = id,
                EmailAdressInternal = "employee@health.qld.gov.au",
                AlertSubscriptions = new List<Guid>
                {
                    Guid.NewGuid(),
                    Guid.NewGuid()
                }
            };

            var inserted = await repo.Upsert(newProfile);

            var toUpdate = await repo.Get<UserProfile>(inserted.Id);

            toUpdate.Confirmed = true;

            var updated = await repo.Upsert(toUpdate);

            Assert.True(updated.Confirmed);
        }

        [Fact,
       Microsoft.VisualStudio.TestTools.UnitTesting.TestCategory("Integration")]
        public async Task WillReturnNullIfIdIsNull()
        {
            var repo = new MongoDbRepository();

            var found = await repo.Get<UserProfile>(null); //id == null

            Assert.True(found == null);
        }
    }
}
