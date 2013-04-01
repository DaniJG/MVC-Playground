using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using DJRM.Common.Infrastructure;
using DJRM.Common.Infrastructure.Repository;
using DJRM.Common.Infrastructure.Events;
using DJRM.Common.Infrastructure.Exceptions;

namespace DJRM.Configuration.Repository
{
    public abstract class RepositoryBase<T, TDataContext> : IRepository<T>, IUnitOfWorkRepository<T>
            where T : EntityBase, IAggregateRoot
            where TDataContext : IBaseDataContext
    {
        protected IUnitOfWork _uow;
        protected TDataContext _dataContext;

        public RepositoryBase(IUnitOfWork uow, TDataContext dataContext)
        {
            _uow = uow;
            _dataContext = dataContext;
        }

        #region IRepository<T>
        public void Add(T entity)
        {
            _uow.RegisterNew(entity, this);

            EntityAddedEvent<T> domainEvent = new EntityAddedEvent<T> { Entity = entity };
            DomainEvents.Raise(domainEvent);
        }

        public void Remove(T entity)
        {
            EntityDeletingEvent<T> domainEvent = new EntityDeletingEvent<T> { Entity = entity };
            DomainEvents.Raise(domainEvent);

            _uow.RegisterDeleted(entity, this);
        }

        public void Save(T entity)
        {
            _uow.RegisterAmended(entity, this);

            var modifiedProperties = _dataContext.GetModifiedProperties(entity);
            if (modifiedProperties.Count > 0)
            {
                EntityUpdatedEvent<T> domainEvent = new EntityUpdatedEvent<T>
                {
                    Entity = entity,
                    UpdatedProperties = modifiedProperties.Keys.ToList(),
                    OriginalValues = modifiedProperties
                };
                DomainEvents.Raise(domainEvent);
            }
        }

        public T FindBy(int id)
        {
            try
            {
                return GetItems().Single(m => m.Id == id);
            }
            catch (InvalidOperationException ex)
            {
                throw new EntityNotFoundException(typeof(T).Name, id, ex);
            }
        }

        public IQueryable<T> FindAll()
        {
            return GetItems();
        }
        #endregion

        protected abstract IQueryable<T> GetItems();        

        #region IUnitOfWorkRepository<T>
        public void PersistCreationOf(T entity)
        {
            GetDbSet().Add(entity);
        }

        public void PersistUpdateOf(T entity)
        {
            //Do nothing, EF keeps track of changes
        }

        public void PersistDeletionOf(T entity)
        {
            GetDbSet().Remove(entity);
        }

        protected abstract DbSet<T> GetDbSet();
        #endregion
    }
}