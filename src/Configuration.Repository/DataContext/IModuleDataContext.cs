using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using DJRM.Configuration.Model;
using DJRM.Common.Infrastructure;

namespace DJRM.Configuration.Repository
{
    public interface IModuleDataContext : IBaseDataContext
    {
        DbSet<Customer> Customers { get; set; }
        DbSet<Task> Tasks { get; set; }
    }
}
