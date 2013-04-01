using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DJRM.Common.Infrastructure.Validation
{
    public class ValidationError
    {
        public string EntityName { get; set; }
        public int EntityId { get; set; }
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }
}