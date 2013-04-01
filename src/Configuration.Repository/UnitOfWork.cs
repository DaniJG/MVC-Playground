using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Data.Entity;
using DJRM.Common.Infrastructure;
using DJRM.Common.Infrastructure.Repository;
using DJRM.Common.Infrastructure.Events;
using DJRM.Common.Infrastructure.Exceptions;

namespace DJRM.Configuration.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IModuleDataContext _moduleDataContext;
        public UnitOfWork(IModuleDataContext moduleDataContext)
        {
            _moduleDataContext = moduleDataContext;
        }

        public void RegisterAmended<T>(T entity, IUnitOfWorkRepository<T> repository) where T : IAggregateRoot
        {
            repository.PersistUpdateOf(entity);
        }

        public void RegisterNew<T>(T entity, IUnitOfWorkRepository<T> repository) where T : IAggregateRoot
        {
            repository.PersistCreationOf(entity);
        }

        public void RegisterDeleted<T>(T entity, IUnitOfWorkRepository<T> repository) where T : IAggregateRoot
        {
            repository.PersistDeletionOf(entity);
        }

        public void Commit()
        {
            using (TransactionScope scope = new TransactionScope())
            {
                //Save changes on each data context (one property per data context, singleton for each HTTP request)
                _moduleDataContext.Save();
                
                scope.Complete();
            }
        }

    }
}
