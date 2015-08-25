MNI.ErrorLogging
=======================

This component centralizes and simplifies error logging. 

Configuration
-------------

Currently autofac is the preferred way to configure this component.

You may need to write an implementation of IErrorRepository/IErrorLogger if autofac is not an option

Autofac Sample: 

	//--------------------------------------------------------------
	// Global ASAX
	//--------------------------------------------------------------

	var applicationName = "Sample"; // Get from config if you prefer

	// Setup our logging component. We take an IEnumerable here so multiple Error Repositories can be 
	// registered. i.e. One for Event log + a local db write
	builder.Register(context =>

		new ErrorLogger(context.Resolve<IEnumerable<IErrorRepository>>(), applicationName)

	).As<IErrorLogger>();

	// Register your implementation of repository
	builder.RegisterType<MY IMP HERE>().As<IErrorRepository>();	

Usage
-----

Anywhere IErrorLogger is available the LogError(IError error) [Some override's available] will be
used to call the Log implementation on all registered IErrorRepository items. 
