using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using DJRM.Common.Infrastructure;

namespace DJRM.Configuration.Repository
{
    public abstract class BaseDataContext : DbContext, IBaseDataContext
    {        
        public BaseDataContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            this.Configuration.ProxyCreationEnabled = true;
            this.Configuration.LazyLoadingEnabled = true;
        }

        public void Save()
        {
            this.SaveChanges();
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    //Updates here to the way the database will be created (column names, tables, pluralization, etc)

        //}

        public Dictionary<string, object> GetModifiedProperties(EntityBase entity)
        {
            Dictionary<string, object> modifiedPropertiesOriginalValues = new Dictionary<string, object>();

            //This code will discover changes made to properties of the entity.
            //Navigation properties of type foreign key, where the foreign key field is part of the entity, 
            //like CustomerId in Task will also be tracked with this code.
            DbEntityEntry entityStateEntry = this.Entry(entity);
            if (entityStateEntry == null || entityStateEntry.State != EntityState.Modified)
                return modifiedPropertiesOriginalValues;

            foreach (string property in entityStateEntry.CurrentValues.PropertyNames)
            {
                var propertyState = entityStateEntry.Member(property);
                if (propertyState is DbPropertyEntry)
                {
                    var propertyEntry = propertyState as DbPropertyEntry;
                    if (propertyEntry.IsModified)
                    {
                        modifiedPropertiesOriginalValues.Add(property, propertyEntry.OriginalValue);
                    }
                }
                else if (propertyState is DbComplexPropertyEntry)
                {
                    var propertyEntry = propertyState as DbComplexPropertyEntry;
                    if (propertyEntry.IsModified)
                    {
                        modifiedPropertiesOriginalValues.Add(property, propertyEntry.OriginalValue);
                    }
                }
            }

            //For independent associations, we might not need to track them as they will also be added\deleted from the data context
            //so we will raise those events.
            //However we could try to discover changes on relationships...

            return modifiedPropertiesOriginalValues;
        }
    }
}