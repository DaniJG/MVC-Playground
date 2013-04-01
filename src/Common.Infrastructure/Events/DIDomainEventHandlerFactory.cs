using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;

namespace DJRM.Common.Infrastructure.Events
{
    public class DIDomainEventHandlerFactory : IDomainEventHandlerFactory
    {
        private UnityContainer _container;

        public DIDomainEventHandlerFactory(UnityContainer container)
        {
            _container = container;
        }

        public IEnumerable<IDomainEventHandler<T>> GetDomainEventHandlersFor<T>(T domainEvent) where T : IDomainEvent
        {
            //WARNING: when using ResolveAll with Unity, only named registrations are returned!
            return _container.ResolveAll<IDomainEventHandler<T>>();
        }
    }
}