using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.ViewModels
{
    public class ChangePasswordViewModel
    {
        [StringLength(20, MinimumLength = 8,
            ErrorMessage = "Password must be with minimum length 8 and maximum length 20")]
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }
        [StringLength(20, MinimumLength = 8,
            ErrorMessage = "Password must be with minimum length 8 and maximum length 20")]
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }
        [StringLength(20, MinimumLength = 8,
            ErrorMessage = "Password must be with minimum length 8 and maximum length 20")]
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]

        public string ConfirmNewPassword { get; set; }

        public ChangePasswordViewModel()
        {

        }
    }
}
