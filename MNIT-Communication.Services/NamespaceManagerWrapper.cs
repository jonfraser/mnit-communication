using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

namespace MNIT_Communication.Services
{
    public class NamespaceManagerWrapper: INamespaceManager
    {
        private NamespaceManager namespaceManager;
        
        public NamespaceManagerWrapper()
        {
            var serviceBusConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            namespaceManager = NamespaceManager.CreateFromConnectionString(serviceBusConnectionString);
        }
        
        public async Task<QueueDescription> CreateQueueAsync(string path)
        {
            return await namespaceManager.CreateQueueAsync(path);
        }

        public async Task<bool> QueueExistsAsync(string path)
        {
            return await namespaceManager.QueueExistsAsync(path);
        }
    }
}
