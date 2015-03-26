using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using MNIT_Communication.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MNIT_Communication.Services
{
	public class AlertsService : IAlertsService
	{
		private readonly IServiceBus _serviceBus;
		private readonly NamespaceManager _namespaceManager;
		private readonly string _serviceBusConnectionString;
		public AlertsService(IServiceBus serviceBus)
		{
			this._serviceBus = serviceBus;
			this._serviceBusConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
			this._namespaceManager = NamespaceManager.CreateFromConnectionString(_serviceBusConnectionString);
		
		}

		public async Task<Guid> RegisterNewUserForInitialAlerts(Guid newUserRegistrationId, string emailAddress, IEnumerable<Guid> alertables)
		{
			foreach (var alertable in alertables)
			{
				var message = new RegisterAlertBrokeredMessage
				{
					CorrelationId = Guid.NewGuid(),
					EmailAddress = emailAddress,
					AlertableId = alertable
				};
				await _serviceBus.SendToQueueAsync(new BrokeredMessage(message), Queues.AlertsRegistration);
			}

			return newUserRegistrationId;
		}

		public async Task RaiseAlert(Guid alertableId, string alertDetail, string alertInfoShort)
		{
			var queueExists = await _namespaceManager.TopicExistsAsync(Topics.Alerts);
			if (!queueExists)
			{
				await _namespaceManager.CreateTopicAsync(Topics.Alerts);
			}

			var client = TopicClient.CreateFromConnectionString(_serviceBusConnectionString, Topics.Alerts);

			var message = new AlertBrokeredMessage
			{
				CorrelationId = Guid.NewGuid(),
				AlertableId = alertableId,
				AlertDetail = alertDetail,
				AlertInfoShort = alertInfoShort,
				AlertRaiser = 0 //TODO
			};

			await client.SendAsync(new BrokeredMessage(message));
		}

		public async Task<IEnumerable<AlertSummary>> GetCurrentAlerts()
		{
			//todo: get current alerts out of SQL			
			return await Task.Run(() => new List<AlertSummary>());

		}

		private async Task<QueueClient> EnsureQueueExists(string queueName)
		{
			var queueExists = await _namespaceManager.QueueExistsAsync(queueName);
			if (!queueExists)
			{
				await _namespaceManager.CreateQueueAsync(queueName);
			}
			return QueueClient.CreateFromConnectionString(_serviceBusConnectionString, queueName);
		}
	}
}