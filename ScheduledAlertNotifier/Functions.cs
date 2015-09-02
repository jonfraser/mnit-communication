using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Data.Edm.Library.Expressions;
using Microsoft.WindowsAzure;
using MNIT.ErrorLogging;
using MNIT_Communication.Domain;
using MNIT_Communication.Services;

namespace ScheduledAlertNotifier
{
    public class Functions
    {
        private static IAuditService auditService;
        private static IErrorLogger errorLogger;
        private static IAlertsService alertService;


        public async static Task CheckForScheduledAlerts()
        {
            Console.WriteLine("CheckForScheduledAlerts called");

            auditService = ServiceLocator.Resolve<IAuditService>();
            errorLogger = ServiceLocator.Resolve<IErrorLogger<Guid>>();
            alertService = ServiceLocator.Resolve<IAlertsService>();
            
            var intervalSeconds = 60; //Default
            int.TryParse(CloudConfigurationManager.GetSetting("ScheduledAlertNotifier.IntervalSeconds"), out intervalSeconds);
            var milliseonds =TimeSpan.FromSeconds(intervalSeconds).TotalMilliseconds;

            await Task.Run(() =>
            {
                using (var timer = new Timer(milliseonds))
                {
                    timer.Elapsed += DoNotifications;

                    timer.Enabled = true;
                    timer.Start();

                    Console.WriteLine("Timer started");
                }
            });
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
