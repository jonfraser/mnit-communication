using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace MNIT_Communication.Services
{
    public interface INamespaceManager
    {
        Task<QueueDescription> CreateQueueAsync(string path);
        Task<bool> QueueExistsAsync(string path);
    }
}