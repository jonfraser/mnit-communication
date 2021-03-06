﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure;
using MNIT.ErrorLogging;
using MNIT_Communication.Services;

namespace AlertUserViaExternalEmailJob
{
	// To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
	class Program
	{
		// Please set the following connection strings in app.config for this WebJob to run:
		// AzureWebJobsDashboard and AzureWebJobsStorage
		static void Main()
		{
            ServiceLocator.RegisterType<MongoDbRepository>().As<IRepository>();
		    
            ServiceLocator.RegisterType<WebJobRuntimeContext>().As<IRuntimeContext>();
            ServiceLocator.RegisterType<ErrorLogger<Guid>>().As<IErrorLogger<Guid>>();
            ServiceLocator.RegisterType<ErrorRepository>().As<IErrorRepository>();
            ServiceLocator.RegisterType<AuditService>().As<IAuditService>();

            ServiceLocator.RegisterType<AlertsService>().As<IAlertsService>();
            ServiceLocator.RegisterType<SendGridEmailService>().As<ISendEmail>();
            
            var host = new JobHost(new JobHostConfiguration
			{
				ServiceBusConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString")
			});
			// The following code ensures that the WebJob will be running continuously
			host.RunAndBlock();
		}
	}
}
