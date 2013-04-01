using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DJRM.Common.Infrastructure.Events
{
    public static class DomainEvents
    {
        public static IDomainEventHandlerFactory DomainEventHandlerFactory { get; set; }

        public static void Raise<T>(T domainEvent) where T : IDomainEvent
        {
            if (DomainEventHandlerFactory == null)
                throw new InvalidOperationException("DomainEventHandlerFactory not set.");

            foreach (IDomainEventHandler<T> handler in DomainEventHandlerFactory.GetDomainEventHandlersFor(domainEvent))
            {
                handler.Handle(domainEvent);
            }
        }
    }
}