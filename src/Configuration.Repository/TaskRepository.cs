using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using DJRM.Configuration.Model;
using DJRM.Common.Infrastructure.Repository;

namespace DJRM.Configuration.Repository
{
    public class TaskRepository : RepositoryBase<Task, IModuleDataContext>
    {
        public TaskRepository(IUnitOfWork uow, IModuleDataContext dataContext)
            :base(uow, dataContext)
        {
        }

        protected override IQueryable<Task> GetItems()
        {
            return _dataContext.Tasks.Include(task => task.Customer);
        }

        protected override System.Data.Entity.DbSet<Task> GetDbSet()
        {
            return _dataContext.Tasks;
        }
    }
}