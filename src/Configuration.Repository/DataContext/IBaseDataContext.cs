using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using DJRM.Common.Infrastructure;

namespace DJRM.Configuration.Repository
{
    public interface IBaseDataContext
    {
        void Save();
        Dictionary<string, object> GetModifiedProperties(EntityBase entity);
    }
}
