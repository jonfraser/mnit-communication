using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNIT_Communication.Domain;
using MNIT_Communication.Services.Fakes;
using Xunit;

namespace MNIT_Communication.Tests.Services.Fakes
{
    public class FakeRepositoryTests
    {

        [Fact]
        public async Task CanStoreItemInRepository()
        {
            var repo = new FakeRepository();

            await repo.Upsert(new UserProfile());
        }
    }
}
