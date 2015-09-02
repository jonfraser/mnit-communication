﻿using System;
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
        public static void CheckForScheduledAlerts()
        {
            Console.WriteLine("CheckForScheduledAlerts called");

            var intervalSeconds = 60; //Default
            int.TryParse(CloudConfigurationManager.GetSetting("ScheduledAlertNotifier.IntervalSeconds"), out intervalSeconds);
            var milliseonds =TimeSpan.FromSeconds(intervalSeconds).TotalMilliseconds;

            using (var timer = new Timer(milliseonds))
            {
                timer.Elapsed += DoNotifications;

                timer.Enabled = true;
                timer.Start();

                Console.WriteLine("Timer started");
            }
        }

        private static void DoNotifications(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("DoNotifications called");

            Task.Run(async () =>
            {
                var auditService = ServiceLocator.Resolve<IAuditService>();
                var errorLogger = ServiceLocator.Resolve<IErrorLogger<Guid>>();
                var alertService = ServiceLocator.Resolve<IAlertsService>();

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
