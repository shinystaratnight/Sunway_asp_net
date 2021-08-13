namespace Web.Template.IoC
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Web.Compilation;
    using System.Web.Http;
    using System.Web.Mvc;

    using Autofac;
    using Autofac.Integration.Mvc;
    using Autofac.Integration.WebApi;

    /// <summary>
    ///     Handles all the project dependency injection
    /// </summary>
    public class AutofacRoot
    {
        /// <summary>
        ///     Setups this instance.
        /// </summary>
        public static void Setup()
        {
            var builder = new ContainerBuilder();
            HttpConfiguration config = GlobalConfiguration.Configuration;
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            // Register model binders that require DI.
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinderProvider();

            // Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            // Enable property injection into action filters.
            builder.RegisterFilterProvider();

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

            builder.RegisterModule(new AutofacModule());
            builder.RegisterModule(new Data.IoC.AutofacModule());
            builder.RegisterModule(new Application.IoC.AutofacModule());

            // Set the dependency resolver to be Autofac.
            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        /// <summary>
        /// Setups for tests.
        /// </summary>
        public static void SetupForTests()
        {
            var builder = new ContainerBuilder();
            HttpConfiguration config = GlobalConfiguration.Configuration;
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            // Register model binders that require DI.
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinderProvider();

            // Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            // Enable property injection into action filters.
            builder.RegisterFilterProvider();

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            builder.RegisterAssemblyModules(assemblies);

            // Set the dependency resolver to be Autofac.
            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}