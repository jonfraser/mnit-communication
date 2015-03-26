using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MNIT_Communication.Services
{
	public interface IServiceBus
	{
		Task SendToQueueAsync<T>(T message, string queueName) where T : IXmlSerializable;
		Task SendToTopicAsync<T>(T message, string topicName) where T : IXmlSerializable;
	}
}
