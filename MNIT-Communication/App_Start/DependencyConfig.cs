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
using MNIT_Communication.Services;

namespace MNIT_Communication.App_Start
{
	public class DependencyConfig
	{
		public static void BuildUpContainer()
		{
			var builder = new ContainerBuilder();

#if DEBUG
			builder.RegisterInstance(new FakeRegistrationService()).As<IRegistrationService>();
			builder.RegisterInstance(new FakeAlertsService()).As<IAlertsService>();
#else
			builder.RegisterInstance(new RegistrationService()).As<IRegistrationService>();
			builder.RegisterInstance(new AlertsService()).As<IAlertsService>();
#endif

			builder.RegisterInstance(new GoogleUrlShortener()).As<IUrlShorten>();
			builder.RegisterInstance(new SendTwilioSmsService()).As<ISendSms>();
			builder.RegisterInstance(new RedisStore()).As<IShortTermStorage>();
			builder.RegisterInstance(new SendGridEmailService()).As<ISendEmail>();

			builder.RegisterControllers(typeof(MvcApplication).Assembly);
			builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

			var config = GlobalConfiguration.Configuration;

			var container = builder.Build();
			config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
		}
	}
}