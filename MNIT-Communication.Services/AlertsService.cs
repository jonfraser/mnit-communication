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

		public async Task<Guid> RegisterNewUserForInitialAlerts(Guid newUserRegistrationId, string emailAddress, IEnumerable<Guid> alertables)
		{
			var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
			var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
			var queueExists = await namespaceManager.QueueExistsAsync(Queues.AlertsRegistration);
			if (!queueExists)
			{
				await namespaceManager.CreateQueueAsync(Queues.AlertsRegistration);
			}

			var client = QueueClient.CreateFromConnectionString(connectionString, Queues.AlertsRegistration);

			foreach (var alertable in alertables)
			{
				var message = new RegisterAlertBrokeredMessage
				{
					CorrelationId = Guid.NewGuid(),
					EmailAddress = emailAddress,
					AlertableId = alertable
				};
				await client.SendAsync(new BrokeredMessage(message));
			}

			return newUserRegistrationId;
		}

		public async Task RaiseAlert(Guid alertableId, string alertDetail, string alertInfoShort)
		{
			//push a topic onto the queue
			var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
			var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
			var queueExists = await namespaceManager.TopicExistsAsync(Topics.Alerts);
			if (!queueExists)
			{
				await namespaceManager.CreateTopicAsync(Topics.Alerts);
			}

			var client = TopicClient.CreateFromConnectionString(connectionString, Topics.Alerts);

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
	}
}