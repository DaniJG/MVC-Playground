using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DJRM.Common.Infrastructure.Repository
{
    public interface IRepository<T>
        where T : EntityBase, IAggregateRoot
    {
        T FindBy(int id);
        IQueryable<T> FindAll();

        void Save(T entity);
        void Add(T entity);
        void Remove(T entity);
    }
}
