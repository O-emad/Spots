using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Models
{
    public class PasswordChangeModel
    {
        [StringLength(20, MinimumLength = 8,
            ErrorMessage = "Password must be with minimum length 8 and maximum length 20")]
        [Required]
        public string OldPassword { get; set; }
        [StringLength(20, MinimumLength = 8,
            ErrorMessage = "Password must be with minimum length 8 and maximum length 20")]
        [Required]
        public string NewPassword { get; set; }
    }
}
