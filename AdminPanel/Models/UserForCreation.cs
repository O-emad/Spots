using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Models
{
    public class UserForCreation
    {
        [MaxLength(200)]
        [Required]
        public string Subject { get; set; }
        [MaxLength(200)]
        public string UserName { get; set; }
        [MaxLength(20)]
        public string Password { get; set; }
        [Required]
        public bool Active { get; set; }
        public ICollection<UserClaimForCreation> Claims { get; set; } = new List<UserClaimForCreation>();
    }

    public class UserClaimForCreation
    {
        [MaxLength(250)]
        [Required]
        public string Type { get; set; }
        [MaxLength(250)]
        [Required]
        public string Value { get; set; }
    }
}
