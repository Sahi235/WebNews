using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.ViewModels.Accounts
{
    public class RegisterVM
    {
        [MaxLength(255)]
        [Remote(action:"IsUsernameInUse", controller:"Accounts")]
        public string UserName { get; set; }
        [MaxLength(255)]
        [DataType(DataType.EmailAddress)]
        [Remote(action: "IsEmailInUse", controller: "Accounts")]
        public string Email { get; set; }
        [MaxLength(255)]
        [DataType(DataType.EmailAddress)]
        [Compare("Email", ErrorMessage ="Emails dont match")]
        public string ConfirmEmail { get; set; }
        [MaxLength(255)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
