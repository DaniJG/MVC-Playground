using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DJRM.Common.Infrastructure.Validation
{
    public class ValidationException
    {
        public ValidationException(List<ValidationError> errors)
        {
            Errors = errors;
        }

        public List<ValidationError> Errors { get; set; }
    }
}