using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure;
using MNIT.ErrorLogging;
using MNIT_Communication.Services;

namespace ScheduledAlertNotifier
{
    class Program
    {
        public static void Main(string[] args)
        {

            ServiceLocator.RegisterType<MongoDbRepository>().As<IRepository>();

            ServiceLocator.RegisterType<WebJobRuntimeContext>().As<IRuntimeContext>();
            ServiceLocator.RegisterType<ErrorLogger<Guid>>().As<IErrorLogger<Guid>>();
            ServiceLocator.RegisterType<ErrorRepository>().As<IErrorRepository>();
            ServiceLocator.RegisterType<AuditService>().As<IAuditService>();

            ServiceLocator.RegisterType<AlertsService>().As<IAlertsService>();

            var host = new JobHost();
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }
    }
}
