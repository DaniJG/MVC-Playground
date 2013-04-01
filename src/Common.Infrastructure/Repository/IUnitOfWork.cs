using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DJRM.Common.Infrastructure.Repository
{
    public interface IUnitOfWork
    {
        void RegisterAmended<T>(T entity, IUnitOfWorkRepository<T> repository) where T : IAggregateRoot;
        void RegisterNew<T>(T entity, IUnitOfWorkRepository<T> repository) where T : IAggregateRoot;
        void RegisterDeleted<T>(T entity, IUnitOfWorkRepository<T> repository) where T : IAggregateRoot;

        void Commit();
    }
}
