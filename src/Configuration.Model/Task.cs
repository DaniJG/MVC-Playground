using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DJRM.Common.Infrastructure;
using DJRM.Common.Infrastructure.Repository;

namespace DJRM.Configuration.Model
{
    public class Task : EntityBase, IAggregateRoot
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [UIHint("Multiline")]
        public string Description { get; set; }

        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public override Type ObjectType
        {
            get { return typeof(Task); }
        }
    }
}