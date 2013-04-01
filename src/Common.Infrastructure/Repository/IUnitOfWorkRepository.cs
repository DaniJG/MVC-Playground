using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DJRM.Common.Infrastructure.Repository
{
    public interface IUnitOfWorkRepository<T> where T : IAggregateRoot
    {
        void PersistCreationOf(T entity);
        void PersistUpdateOf(T entity);
        void PersistDeletionOf(T entity);
    }
}
