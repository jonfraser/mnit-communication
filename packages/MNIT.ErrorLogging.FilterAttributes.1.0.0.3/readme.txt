MNI.ErrorLogging.FilterAttributes
=======================

This component centralizes and simplifies error logging in MVC/WebApi projects. 

Configuration
-------------

Currently autofac is the preferred way to configure this component. 

Autofac Sample: 

	//--------------------------------------------------------------
	// Global ASAX
	//--------------------------------------------------------------

	builder.Register(c => new MvcHandleErrorAttribute(c.Resolve<IErrorLogger>()))
        .AsExceptionFilterFor<Controller>().InstancePerRequest();

    builder.Register(c => new WebApiExceptionFilterAttribute(c.Resolve<IErrorLogger>()))
        .AsWebApiExceptionFilterFor<ApiController>().InstancePerRequest();

    builder.RegisterFilterProvider();
	builder.RegisterWebApiFilterProvider(config);

