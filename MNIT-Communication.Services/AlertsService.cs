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
		private readonly INamespaceManager _namespaceManager;
	    private readonly IUserService userService;
	    private readonly string _serviceBusConnectionString;
		public AlertsService(IServiceBus serviceBus, INamespaceManager namespaceManager, IUserService userService)
		{
			this._serviceBus = serviceBus;
			this._serviceBusConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
			this._namespaceManager = namespaceManager;
		    this.userService = userService;
		}

		public async Task<Guid> SubscribeToAlerts(Guid userId, IEnumerable<Guid> alertables)
		{
		    var userProfile = await userService.RetrieveUserProfile(userId);
		    userProfile.AlertSubscriptions = alertables.ToList();

		    await userService.InsertOrUpdateUserProfile(userProfile);
            
            //TODO - do we want/need to publish this message to the bus?
            //foreach (var alertable in alertables)
            //{
            //    var message = new SubscribeToAlertBrokeredMessage
            //    {
            //        CorrelationId = Guid.NewGuid(),
            //        UserId = userProfile.Id,
            //        EmailAddress = userProfile.EmailAddressExternalProvider,
            //        AlertableId = alertable
            //    };
            //    await _serviceBus.SendToQueueAsync(message, Queues.AlertsSubscription);
            //}

			return userId;
		}

		public async Task RaiseAlert(Guid alertableId, string alertDetail, string alertInfoShort)
		{
			
			var message = new AlertBrokeredMessage
			{
				CorrelationId = Guid.NewGuid(),
				AlertableId = alertableId,
				AlertDetail = alertDetail,
				AlertInfoShort = alertInfoShort,
				AlertRaiser = 0 //TODO
			};

			await _serviceBus.SendToTopicAsync(message, Topics.Alerts);
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