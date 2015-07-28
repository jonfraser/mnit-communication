using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace MNIT_Communication.Services.Fakes
{
    public class FakeNamespaceManager : INamespaceManager
    {
        public Task<QueueDescription> CreateQueueAsync(string path)
        {
            throw new NotImplementedException();
        }

        public Task<bool> QueueExistsAsync(string path)
        {
            throw new NotImplementedException();
        }
    }
}