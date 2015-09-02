using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure;
using MNIT.ErrorLogging;
using MNIT_Communication.Domain;
using MNIT_Communication.Services;
using MNIT_Communication.Services.Fakes;
using Timer = System.Timers.Timer;

namespace ScheduledAlertNotifier
{
    public class Program
    {
        private static IAuditService auditService;
        private static IErrorLogger errorLogger;
        private static IAlertsService alertService;
        
        public static void Main()
        {

            ServiceLocator.RegisterType<MongoDbRepository>().As<IRepository>();
            ServiceLocator.RegisterType<WebJobRuntimeContext>().As<IRuntimeContext>();
            ServiceLocator.RegisterType<ErrorLogger<Guid>>().As<IErrorLogger<Guid>>();
            ServiceLocator.RegisterType<ErrorRepository>().As<IErrorRepository>();
            ServiceLocator.RegisterType<AuditService>().As<IAuditService>();
            ServiceLocator.RegisterType<AlertsService>().As<IAlertsService>();
            
            CheckForScheduledAlerts();

            Console.ReadLine();
        }

        public static void CheckForScheduledAlerts()
        {
            Console.WriteLine("CheckForScheduledAlerts called");

            auditService = ServiceLocator.Resolve<IAuditService>();
            errorLogger = ServiceLocator.Resolve<IErrorLogger<Guid>>();
            alertService = ServiceLocator.Resolve<IAlertsService>();

            var intervalSeconds = 15; //Default
            //int.TryParse(CloudConfigurationManager.GetSetting("ScheduledAlertNotifier.IntervalSeconds"), out intervalSeconds);
            var interval = TimeSpan.FromSeconds(intervalSeconds);

            Repeat(DoNotifications, interval, new CancellationToken());
            
            Console.WriteLine("Repeat started");
        }

        private static async Task DoNotifications()
        {
            Console.WriteLine("DoNotifications called");

            try
            {
                auditService.LogAuditEvent(new AuditEvent
                {
                    AuditType = AuditType.ScheduledAlertsNotified,
                    Details = "CheckForScheduledAlerts was called"
                });

                await alertService.NotifyScheduledAlerts();
                Console.WriteLine("alertService.NotifyScheduledAlerts called");
            }
            catch (Exception ex)
            {
                errorLogger.LogError(ex);
                Console.WriteLine(ex);
            }
        }

        private static async Task Repeat(Func<Task> action, TimeSpan interval, CancellationToken cancellationToken)
        {
            while (true)
            {
                await action();
                Task task = Task.Delay(interval, cancellationToken);

                try
                {
                    await task;
                }

                catch (TaskCanceledException)
                {
                    return;
                }
            }
        }
    }
}
