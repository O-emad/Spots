using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.DTO
{
    public class UserForUpdateDto
    {
        [StringLength(20)]
        public string Password { get; set; }
        [Required]
        public bool Active { get; set; }
        public ICollection<UserClaimForCreation> Claims { get; set; } = new List<UserClaimForCreation>();
    }
}
