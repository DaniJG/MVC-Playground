using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using DJRM.Configuration.Model;
using DJRM.Common.Infrastructure;

namespace DJRM.Configuration.Repository
{
    public class ModuleDataContext : BaseDataContext, IModuleDataContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Task> Tasks { get; set; }

        public ModuleDataContext()
            : base("name=ModuleDataContextConnection")
        {
            //We could override here BaseDataContext settings like proxy creation and lazy loading:
            //this.Configuration.ProxyCreationEnabled = true;
            //this.Configuration.LazyLoadingEnabled = true;

            // If you want Entity Framework to drop and regenerate your database
            // automatically whenever you change your model schema, add the following
            // code to the Application_Start method in your Global.asax file.
            // Note: this will destroy and re-create your database with every model change.
            // 
            // System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<CRMFun.Models.CRMFunContext>());
        }
                
    }
}