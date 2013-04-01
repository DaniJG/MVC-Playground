using System.Web.Mvc;
using Unity.Mvc3;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using DJRM.Common.Infrastructure.Events;
using DJRM.Common.Infrastructure.Profiling;
using DJRM.Common.Infrastructure.Repository;
using DJRM.Common.Infrastructure.Validation;
using DJRM.Configuration.Model;
using DJRM.Configuration.Model.Validation;
using DJRM.Configuration.Repository;

namespace DJRM.UI.Web
{
    public static class UnityBootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            container.AddNewExtension<Interception>();

            //standard registration (TRANSIENT LIFETIME)
            container.RegisterType<IDomainEventHandlerFactory, DIDomainEventHandlerFactory>()
                     .RegisterType<IUnitOfWork, UnitOfWork>()
                     .RegisterType<IRepository<Customer>, CustomerRepository>(
                                            new Interceptor<InterfaceInterceptor>(),
                                            new InterceptionBehavior<ProfilerInterceptor>())
                     .RegisterType<IRepository<Task>, TaskRepository>(
                                            new Interceptor<InterfaceInterceptor>(),
                                            new InterceptionBehavior<ProfilerInterceptor>());

            //single per request - disposable registration, like EF dbContext
            container.RegisterType<IModuleDataContext, ModuleDataContext>(new HierarchicalLifetimeManager())
                     .RegisterType<IValidator<Customer>, CustomerValidator>(new HierarchicalLifetimeManager(),
                                            new Interceptor<InterfaceInterceptor>(),
                                            new InterceptionBehavior<ProfilerInterceptor>())
                     .RegisterType<IValidator<Task>, TaskValidator>(new HierarchicalLifetimeManager(),
                                            new Interceptor<InterfaceInterceptor>(),
                                            new InterceptionBehavior<ProfilerInterceptor>());


            DomainEvents.DomainEventHandlerFactory = container.Resolve<IDomainEventHandlerFactory>();

            return container;
        }
    }
}