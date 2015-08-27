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
using MNIT.ErrorLogging;
using MNIT.ErrorLogging.FilterAttributes;
using MNIT_Communication.Attributes;
using MNIT_Communication.Controllers;
using MNIT_Communication.Helpers;
using MNIT_Communication.Hubs;
using MNIT_Communication.Models;
using MNIT_Communication.Services;
using MNIT_Communication.Services.Fakes;

namespace MNIT_Communication.App_Start
{
	public class DependencyConfig
	{
		public static void BuildUpContainer()
		{
			var builder = new ContainerBuilder();
            var globalConfig = GlobalConfiguration.Configuration;

		    builder.RegisterType<AspNetRuntimeContext>().As<IRuntimeContext>();
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<AlertsService>().As<IAlertsService>().SingleInstance();
		    builder.RegisterType<MongoDbRepository>().As<IRepository>(); //TODO - Singleton or InstancePerRequest
		    builder.RegisterType<OutageHub>().As<IOutageHub>();
            builder.Register(c => new ErrorLogger<Guid>(c.Resolve<IEnumerable<IErrorRepository>>(), identityGenerator: Guid.NewGuid)).As<IErrorLogger<Guid>>();
		    builder.RegisterType<ErrorRepository>().As<IErrorRepository>();
		    builder.RegisterType<AuditService>().As<IAuditService>();

#if DEBUG
            builder.RegisterType<FakeServiceBus>().As<IServiceBus>();
            builder.RegisterType<FakeUrlShortener>().As<IUrlShorten>();
            builder.RegisterType<FakeSmsService>().As<ISendSms>();
            builder.RegisterType<FakeShortTermStorage>().As<IShortTermStorage>().SingleInstance();
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
            //Register Filters 
            builder.Register(c => new UserProfileConfirmedAttribute(c.Resolve<IRuntimeContext>()))
                   .PropertiesAutowired(PropertyWiringOptions.PreserveSetValues);

            builder.Register(c => new IsAdministratorAttribute(c.Resolve<IRuntimeContext>()))
                   .PropertiesAutowired(PropertyWiringOptions.PreserveSetValues);

            builder.Register(c => new MvcErrorLoggingFilterAttribute(c.Resolve<IErrorLogger<Guid>>()))
                .AsExceptionFilterFor<Controller>().InstancePerRequest();

            builder.Register(c => new WebApiErrorLoggingFilterAttribute(c.Resolve<IErrorLogger<Guid>>()))
                .AsWebApiExceptionFilterFor<ApiController>().InstancePerRequest();

            builder.RegisterFilterProvider();
            builder.RegisterWebApiFilterProvider(globalConfig);
            
            
            //Register Controllers
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