using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure;
using MNIT.ErrorLogging;
using MNIT_Communication.Domain;
using MNIT_Communication.Services;

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
        }

        public static void CheckForScheduledAlerts()
        {
            Console.WriteLine("CheckForScheduledAlerts called");

            auditService = ServiceLocator.Resolve<IAuditService>();
            errorLogger = ServiceLocator.Resolve<IErrorLogger<Guid>>();
            alertService = ServiceLocator.Resolve<IAlertsService>();

            var intervalSeconds = 60; //Default
            //int.TryParse(CloudConfigurationManager.GetSetting("ScheduledAlertNotifier.IntervalSeconds"), out intervalSeconds);
            var milliseconds = TimeSpan.FromSeconds(intervalSeconds).TotalMilliseconds;

            using (var timer = new Timer(milliseconds))
            {
                timer.Elapsed += DoNotifications;
                timer.Start();

                Console.WriteLine("Timer started");
            }
        }

        private static void DoNotifications(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("DoNotifications called");

            Task.Run(async () =>
            {
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
                }
            });
        }
    }
}
