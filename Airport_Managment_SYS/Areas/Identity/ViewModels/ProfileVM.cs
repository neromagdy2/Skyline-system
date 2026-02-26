using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesApp.ViewModels
{
    public class ProfileVM
    {
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Nationality")]
        public string? Nationality { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();
    }
}

