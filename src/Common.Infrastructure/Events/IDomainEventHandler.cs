using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DJRM.Common.Infrastructure.Events
{
    public interface IDomainEventHandler<T>
        where T : IDomainEvent
    {
        bool WillHandle(T domainEvent);
        void Handle(T domainEvent);
    }
}
