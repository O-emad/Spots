using AdminPanel.Models;
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
        public Guid Id { get; set; } = Guid.Empty;

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
        [Display(Name = "First name")]
        public string GivenName { get; set; }
        [MaxLength(250)]
        [Display(Name = "Last name")]
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

        public UserCreateViewModel()
        {

        }

        public UserCreateViewModel(UserModel user)
        {
            Id = user.Id;
            Username = user.UserName;
            Password = user.Password;
            Active = user.Active;
            GivenName = user.Claims.Where(c => c.Type == "given_name").FirstOrDefault().Value;
            FamilyName = user.Claims.Where(c => c.Type == "family_name").FirstOrDefault().Value;
            Role = user.Claims.Where(c => c.Type == "role").FirstOrDefault().Value;
        }
    }
}
