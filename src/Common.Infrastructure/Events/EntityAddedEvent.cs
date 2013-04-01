using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DJRM.Common.Infrastructure.Events
{
    /// <summary>
    /// Event raised when a new entity of type T has been added.
    /// </summary>
    /// <typeparam name="T">Type of the added entity.</typeparam>
    public class EntityAddedEvent<T> : IDomainEvent
        where T : EntityBase
    {
        public T Entity { get; set; }
    }
}