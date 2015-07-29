using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Microsoft.WindowsAzure;
using MNIT_Communication.Services;
using MNIT_Communication.Services.Fakes;

namespace MNIT_Communication.App_Start
{
	public class DependencyConfig
	{
		public static void BuildUpContainer()
		{
			var builder = new ContainerBuilder();
            
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<AlertsService>().As<IAlertsService>().SingleInstance();

#if DEBUG
		    builder.RegisterType<FakeServiceBus>().As<IServiceBus>();
            builder.RegisterType<FakeUrlShortener>().As<IUrlShorten>();
            builder.RegisterType<FakeSmsService>().As<ISendSms>();
            builder.RegisterType<FakeShortTermStorage>().As<IShortTermStorage>().SingleInstance();
		    builder.RegisterType<FakeRepository>().As<IRepository>().SingleInstance();
            builder.RegisterType<FakeEmailService>().As<ISendEmail>();
		    builder.RegisterType<FakeNamespaceManager>().As<INamespaceManager>();
#else
		    var serviceBusConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            builder.RegisterInstance(new AzureBus(serviceBusConnectionString)).As<IServiceBus>().SingleInstance();
            builder.RegisterType<GoogleUrlShortener>().As<IUrlShorten>();
            builder.RegisterType<SendTelstraSmsService>().As<ISendSms>();
            builder.RegisterType<RedisStore>().As<IShortTermStorage>();
            builder.RegisterType<SendGridEmailService>().As<ISendEmail>();
            builder.RegisterType<NamespaceManagerWrapper>().As<INamespaceManager>();
#endif

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
			builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

			var config = GlobalConfiguration.Configuration;

			var container = builder.Build();
            
            ServiceLocator.Container = container;
            
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
		}
	}
}