﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DJRM.Common.Security.Model
{
    public class RegisterExternalLoginModel
    {
        [Required]
        [Remote("UserNameIsUnique", "Account")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        public string PictureURL { get; set; }

        public string ExternalLoginData { get; set; }
    }
}
