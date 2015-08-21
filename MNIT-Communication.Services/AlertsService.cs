﻿using Microsoft.ServiceBus;
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
		private readonly IServiceBus serviceBus;
		private readonly INamespaceManager namespaceManager;
	    private readonly IUserService userService;
	    private readonly IRepository repository;
	    private readonly IOutageHub outageHub;
	    private readonly IRuntimeContext runtimeContext;

	    private readonly string serviceBusConnectionString;

		public AlertsService(IServiceBus serviceBus, INamespaceManager namespaceManager, IUserService userService, IRepository repository, IOutageHub outageHub, IRuntimeContext runtimeContext)
		{
			this.serviceBus = serviceBus;
			this.serviceBusConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
			this.namespaceManager = namespaceManager;
		    this.userService = userService;
		    this.repository = repository;
		    this.outageHub = outageHub;
		    this.runtimeContext = runtimeContext;
		}

        public AlertsService(IRepository repository)
        {
            this.repository = repository;
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

		public async Task RaiseAlert(RaiseAlertRequest request)
		{
		    var uniqueIdentifier = Guid.NewGuid();

            foreach (var alertable in request.Alertables)
            {
                var alert = new Alert
                {
                    Id = uniqueIdentifier,
                    Service = alertable,
                    Summary = request.AlertInfoShort,
                    Start = DateTime.Now,
                    RaisedBy = request.RaisedById
                };

                var raisedEvent = new AlertHistory
                {
                    Status = AlertStatus.Raised,
                    Timestamp = DateTime.Now,
                    Detail = request.AlertDetail,
                    UserId = alert.RaisedBy
                };

                alert.History.Add(raisedEvent);

                await repository.Upsert(alert);

                var message = new AlertBrokeredMessage
                {
                    CorrelationId = uniqueIdentifier,
                    AlertableId = alert.Service.Id,
                    AlertDetail = alert.LastUpdate.Detail,
                    AlertInfoShort = alert.Summary,
                    AlertRaiser = alert.RaisedBy
                };

                await serviceBus.SendToTopicAsync(message, Topics.Alerts);
                await outageHub.NotifyChange(alert);
            }
		}

		public async Task<IEnumerable<Alert>> GetCurrentAlerts()
		{
            //TODO - MongoDB barfs on this query , but pulling back ALL Alerts is not cool. Should I store the latest status in the DB and query that?
            var allAlerts = await repository.Get<Alert>();
            var currentAlerts = allAlerts.Where(alert => alert.IsCurrent).ToArray();
            await MarkSubscribed(currentAlerts);

		    return currentAlerts;
		}

	    private async Task MarkSubscribed(params Alert[] alerts)
	    {
            //Only attempt if we have a Profile to interrogate
            if (await runtimeContext.HasProfile())
            {
                var subscriptions = (await runtimeContext.CurrentProfile()).AlertSubscriptions;
                foreach (var alert in alerts)
                {
                    //If the user's subscriptions contains the Id of the service, then mark as subscribed
                    alert.UserSubscribed = subscriptions.Contains(alert.Service.Id);
                }
	        }
	    }

        public async Task<IEnumerable<UserProfile>> GetSubscribersFor(params Guid[] alertables)
	    {
	        //Find the users who are subscribed to ANY of the systems in the 'alertables' collection
            var subscribers = await repository.Get<UserProfile>(u => u.AlertSubscriptions.Intersect(alertables.AsEnumerable()).Any());
	        return subscribers;
	    }

	    private async Task<QueueClient> EnsureQueueExists(string queueName)
		{
			var queueExists = await namespaceManager.QueueExistsAsync(queueName);
			if (!queueExists)
			{
				await namespaceManager.CreateQueueAsync(queueName);
			}
			return QueueClient.CreateFromConnectionString(serviceBusConnectionString, queueName);
		}
	}
}