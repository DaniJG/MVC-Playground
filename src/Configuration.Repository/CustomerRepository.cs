using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using DJRM.Configuration.Model;
using DJRM.Common.Infrastructure.Repository;

namespace DJRM.Configuration.Repository
{
    public class CustomerRepository : RepositoryBase<Customer, IModuleDataContext>
    {
        public CustomerRepository(IUnitOfWork uow, IModuleDataContext dataContext)
            :base(uow, dataContext)
        {
        }

        protected override IQueryable<Customer> GetItems()
        {
            return _dataContext.Customers.Include(customer => customer.Tasks);
        }

        protected override System.Data.Entity.DbSet<Customer> GetDbSet()
        {
            return _dataContext.Customers;
        }
        
    }
}