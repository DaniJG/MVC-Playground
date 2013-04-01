using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DJRM.Common.Infrastructure.Events
{
    /// <summary>
    /// Event raised when a entity is being removed (before the deletion is done).
    /// </summary>
    /// <typeparam name="T">Type of the entity that is being removing.</typeparam>
    public class EntityDeletingEvent<T> : IDomainEvent
        where T : EntityBase
    {
        public T Entity { get; set; }
    }
}