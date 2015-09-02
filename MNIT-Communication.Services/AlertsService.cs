using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using MNIT_Communication.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MNIT.ErrorLogging;
using Newtonsoft.Json;

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
	    private readonly string rootUri;

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
            this.rootUri = CloudConfigurationManager.GetSetting("RootUri");
		}

        public AlertsService(IRepository repository, IRuntimeContext runtimeContext)
        {
            this.repository = repository;
            this.runtimeContext = runtimeContext;
            this.rootUri = CloudConfigurationManager.GetSetting("RootUri");
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
                //TODO - really bad hack her to get around TimeZone issues - there has to be a better way!
#if RELEASE
                if (request.Scheduled)
                {
                    request.Start = request.Start.Value.AddHours(-10);
                }
                if (request.ExpectedFinish.HasValue)
                {
                    request.ExpectedFinish = request.ExpectedFinish.Value.AddHours(-10);
                }
#endif
                
                var uniqueIdentifier = Guid.NewGuid();
                var scheduled = request.Scheduled;

                var start = scheduled ? request.Start.Value : DateTime.Now;
                if (start < DateTime.Now) //If scheduled for the past, then it is actually 'Raised'
                    scheduled = false;

                var status = scheduled ? AlertStatus.Scheduled : AlertStatus.Raised;
                var auditType = scheduled ? AuditType.AlertScheduled : AuditType.AlertRaised;
                
                var alert = new Alert
                {
                    Id = uniqueIdentifier,
                    Service = alertable,
                    Summary = request.AlertInfoShort,
                    Start = start,
                    ExpectedFinish = request.ExpectedFinish,
                    RaisedBy = request.RaisedBy
                };

                var createEvent = new AlertHistory
                {
                    Status = status,
                    Timestamp = DateTime.Now,
                    Detail = request.AlertDetail,
                    UpdatedBy = alert.RaisedBy
                };

                alert.History.Add(createEvent);

                await repository.Upsert(alert);

                var message = new AlertBrokeredMessage
                {
                    CorrelationId = uniqueIdentifier,
                    AlertableId = alert.Service.Id,
                    AlertStatus = status,
                    AlertDetail = alert.LastUpdate.Detail,
                    AlertInfoShort = alert.Summary,
                    AlertRaiser = alert.RaisedBy
                };

                await auditService.LogAuditEventAsync(new AuditEvent
                {
                    AuditType = auditType,
                    Details = "An Alert has been scheduled/raised for " + alert.Service.Name,
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
                throw new ArgumentException(string.Format("No Alert with the Id {0} was found to Update!", request.AlertId), "request.AlertId");

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

            //TODO - really bad hack her to get around TimeZone issues - there has to be a better way!
#if RELEASE
	        alert.LastUpdate.Timestamp = alert.LastUpdate.Timestamp.AddHours(-10);
            if (request.ExpectedFinish.HasValue)
            {
                request.ExpectedFinish = request.ExpectedFinish.Value.AddHours(-10);
            }
#endif
	        alert.ExpectedFinish = request.ExpectedFinish;

            await repository.Upsert(alert);

            if (publishMessage)
            {
                var message = new AlertBrokeredMessage
                {
                    CorrelationId = alert.Id,
                    AlertableId = alert.Service.Id,
                    AlertStatus = request.Update.Status,
                    AlertDetail = alert.LastUpdate.Detail,
                    AlertInfoShort = alert.Summary,
                    AlertRaiser = request.Update.UpdatedBy
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

	    public async Task NotifyScheduledAlerts()
	    {
	        var scheduled = await GetAlerts(a => a.LastUpdate.Status.Equals(AlertStatus.Scheduled));

	        foreach (var alert in scheduled)
	        {
	            if (!alert.IsFuture) //If has passed the Start time
	            {
	                var request = new UpdateAlertRequest
	                {
	                    AlertId = alert.Id,
	                    Update = new AlertHistory
	                    {
	                        Status = AlertStatus.Raised,
	                        Detail = "Scheduled Alert begun",
	                        Timestamp = DateTime.Now,
	                        UpdatedBy = alert.RaisedBy
	                    }
	                };

	                //await UpdateAlert(request);

	                var uri = new Uri(rootUri + "/api/Alerts/Update");
                    using (var client = new HttpClient())
	                {
                        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                        var responseMessage = await client.PostAsync(uri, content);
                    }
	            }
	        }
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
            Func<Alert, bool> predicate = alert => alert.IsCurrent;
            return await GetAlerts(predicate);
        }

        public async Task<IEnumerable<Alert>> GetPastAlerts()
        {
            Func<Alert, bool> predicate = alert => alert.IsPast;
            return await GetAlerts(predicate);
        }

        public async Task<IEnumerable<Alert>> GetFutureAlerts()
        {
            Func<Alert, bool> predicate = alert => alert.IsFuture;
            return await GetAlerts(predicate);
        }
        public async Task<IEnumerable<Alert>> GetAlerts(Func<Alert, bool> predicate)
        {
            //TODO - MongoDB barfs on this query , but pulling back ALL Alerts is not cool. Should I store the latest status in the DB and query that?
            var allAlerts = await repository.Get<Alert>();
            var matchingAlerts = allAlerts.Where(predicate).ToArray();
            await MarkSubscribed(matchingAlerts);

            return matchingAlerts;
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
            //TODO - this really shouldn't pull ALL User Profiles into memory
            var subscribers = (await repository.Get<UserProfile>()).Where(u => u.AlertSubscriptions.Any(alertables.Contains));
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