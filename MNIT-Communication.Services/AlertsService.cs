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
		private readonly IServiceBus serviceBus;
		private readonly INamespaceManager namespaceManager;
	    private readonly IUserService userService;
	    private readonly IRepository repository;
	    private readonly IOutageHub outageHub;

	    private readonly string serviceBusConnectionString;

		public AlertsService(IServiceBus serviceBus, INamespaceManager namespaceManager, IUserService userService, IRepository repository, IOutageHub outageHub)
		{
			this.serviceBus = serviceBus;
			this.serviceBusConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
			this.namespaceManager = namespaceManager;
		    this.userService = userService;
		    this.repository = repository;
		    this.outageHub = outageHub;
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
                await outageHub.SendNew(alert);
            }
		}

		public async Task<IEnumerable<Alert>> GetCurrentAlerts()
		{
		    //TODO - Only where current?
            return await repository.Get<Alert>();
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