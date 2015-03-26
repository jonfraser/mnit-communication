﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace MNIT_Communication.Services
{
	public class AzureBus : IServiceBus
	{
		private readonly NamespaceManager _namespaceManager;
		private readonly string _serviceBusConnectionString;

		public AzureBus(string serviceBusConnectionString)
		{
			this._serviceBusConnectionString = serviceBusConnectionString;
			this._namespaceManager = NamespaceManager.CreateFromConnectionString(serviceBusConnectionString);

		}

		public async Task SendToQueueAsync<T>(T message, string queueName) where T : IXmlSerializable
		{
			var queueExists = await _namespaceManager.QueueExistsAsync(queueName);
			if (!queueExists)
			{
				await _namespaceManager.CreateQueueAsync(queueName);
			}
			var client = QueueClient.CreateFromConnectionString(_serviceBusConnectionString, queueName);

			await client.SendAsync(new BrokeredMessage(message));

		}

		public async Task SendToTopicAsync<T>(T message, string topicName) where T : IXmlSerializable
		{
			throw new NotImplementedException();
		}

	}
}
