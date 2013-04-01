using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using DJRM.Common.Security.Model;

namespace DJRM.Common.Security.Repository
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("SecurityConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}
