using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.ViewModels
{
    public class UserCreateViewModel
    {
        [MaxLength(200)]
        [Required]
        [Display(Name ="Username")]
        public string Username { get; set; }
        [MaxLength(200)]
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [MaxLength(250)]
        [Required]
        [Display(Name = "Given name")]
        public string GivenName { get; set; }
        [MaxLength(250)]
        [Required]
        [Display(Name = "Family name")]
        public string FamilyName { get; set; }
        [Display(Name = "Active Status")]
        public bool Active { get; set; }
        [MaxLength(50)]
        [Required]
        [Display(Name = "Account Type")]
        public string Role { get; set; }

        public SelectList Roles { get; set; } =
            new SelectList(
                new[]
                {
                    new {Type = "Admin", Value = "Admin"},
                    new {Type = "Vendor", Value = "Vendor"}
                },
                "Type",
                "Value"
                );
    }
}
