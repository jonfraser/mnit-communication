using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using MNIT_Communication.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MNIT.ErrorLogging;

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
	    private readonly IAuditService auditService;

	    private readonly string serviceBusConnectionString;

		public AlertsService(IServiceBus serviceBus, INamespaceManager namespaceManager, IUserService userService, IRepository repository, IOutageHub outageHub, IRuntimeContext runtimeContext, IAuditService auditService)
		{
			this.serviceBus = serviceBus;
			this.serviceBusConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
			this.namespaceManager = namespaceManager;
		    this.userService = userService;
		    this.repository = repository;
		    this.outageHub = outageHub;
		    this.runtimeContext = runtimeContext;
		    this.auditService = auditService;
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
            foreach (var alertable in request.Alertables)
            {
                var uniqueIdentifier = Guid.NewGuid();

                var alert = new Alert
                {
                    Id = uniqueIdentifier,
                    Service = alertable,
                    Summary = request.AlertInfoShort,
                    Start = DateTime.Now,
                    RaisedBy = request.RaisedBy
                };

                var raisedEvent = new AlertHistory
                {
                    Status = AlertStatus.Raised,
                    Timestamp = DateTime.Now,
                    Detail = request.AlertDetail,
                    UpdatedBy = alert.RaisedBy
                };

                alert.History.Add(raisedEvent);

                await repository.Upsert(alert);

                var message = new AlertBrokeredMessage
                {
                    CorrelationId = uniqueIdentifier,
                    AlertableId = alert.Service.Id,
                    AlertDetail = alert.LastUpdate.Detail,
                    AlertInfoShort = alert.Summary,
                    AlertRaiser = alert.RaisedBy.Id
                };

                await auditService.LogAuditEventAsync(new AuditEvent
                {
                    AuditType = AuditType.AlertRaised,
                    Details = "An Alert has been raised for " + alert.Service.Name,
                    EntityType = typeof (Alert).Name,
                    EntityId = alert.Id
                });

                await serviceBus.SendToTopicAsync(message, Topics.Alerts);
                await outageHub.NotifyChange(alert);
            }
		}

	    public async Task UpdateAlert(UpdateAlertRequest request)
	    {
	        var publishMessage = false;
            var alert = await repository.Get<Alert>(request.AlertId);

            if(alert == null)
                throw new ArgumentException(string.Format("No Alert with the Id {0} was fiund to Update!", request.AlertId), "request.AlertId");

	        if (request.Update.IsNew)
	        {
	            request.Update.Id = Guid.NewGuid();
	            alert.History.Add(request.Update);
	            publishMessage = true;
	        }
	        else
	        {
	            var existingHistory = alert.History.FirstOrDefault(history => history.Id == request.Update.Id);

                if (existingHistory == null)
                    throw new ArgumentException("Attempted to Update a History entry that does not exist!");

	            alert.History.Remove(existingHistory);//replace the existing history item with the updated one
                alert.History.Add(request.Update);

                //NOT publishing for History updates..
	        }

	        await repository.Upsert(alert);

            if(publishMessage)
            {
                var message = new AlertBrokeredMessage
                {
                    CorrelationId = alert.Id,
                    AlertableId = alert.Service.Id,
                    AlertDetail = alert.LastUpdate.Detail,
                    AlertInfoShort = alert.Summary,
                    AlertRaiser = alert.LastUpdate.UpdatedBy.Id
                };

                await serviceBus.SendToTopicAsync(message, Topics.Alerts);
            }

            await auditService.LogAuditEventAsync(new AuditEvent
            {
                AuditType = GetAuditTypeForStatus(request.Update.Status),
                Details = "An Alert has been updated for " + alert.Service.Name,
                EntityType = typeof(Alert).Name,
                EntityId = alert.Id
            });

            await outageHub.NotifyChange(alert);
        }

	    private AuditType GetAuditTypeForStatus(AlertStatus status)
	    {
	        if (status.Equals(AlertStatus.Raised))
	            return AuditType.AlertRaised;
	        
	        if (status.Equals(AlertStatus.Updated))
                return AuditType.AlertUpdated;

	        if (status.Equals(AlertStatus.Cancelled))
	            return AuditType.AlertCancelled;

            if (status.Equals(AlertStatus.Resolved))
                return AuditType.AlertResolved;

	        return null;
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
            var subscribers = await repository.Get<UserProfile>(u => u.AlertSubscriptions.Any(alertables.Contains));
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