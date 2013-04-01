using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DJRM.Common.Infrastructure;
using DJRM.Common.Infrastructure.Repository;

namespace DJRM.Configuration.Model
{
    public class Customer : EntityBase, IAggregateRoot
    {
        public Customer()
        {
            Tasks = new List<Task>();
        }

        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [DisplayName("Date of Birth")]
        public DateTime DoB { get; set; }

        [UIHint("Multiline")]
        public string Address { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }

        public override Type ObjectType
        {
            get { return typeof(Customer); }
        }
    }
}