using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DJRM.Common.Infrastructure.Validation
{
    public class ValidationResult
    {
        /// <summary>Default constructor which initialises errors list.</summary>
        public ValidationResult()
        {
            Errors = new List<ValidationError>();
        }

        /// <summary>Status of the validation.</summary>
        public bool Success { get { return Errors.Count() == 0; } }

        /// <summary>Errors reported by the validation process.</summary>
        public List<ValidationError> Errors { get; set; }
    }
}
