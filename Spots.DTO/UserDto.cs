//using ExtraSW.IDP.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.DTO
{
    public class UserDto
    {
        public Guid Id { get; set; }
        [MaxLength(200)]
        [Required]
        public string Subject { get; set; }
        [MaxLength(200)]
        public string UserName { get; set; }
        [MaxLength(200)]
        public string Password { get; set; }
        [Required]
        public bool Active { get; set; }
        public ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();
    }

    public class UserClaim { 
        public Guid Id { get; set; }
        [MaxLength(250)]
        [Required]
        public string Type { get; set; }
        [MaxLength(250)]
        [Required]
        public string Value { get; set; }
        [Required]
        public Guid UserId { get; set; }

    }
}
