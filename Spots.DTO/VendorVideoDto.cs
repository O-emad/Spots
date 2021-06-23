using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.DTO
{
    public class VendorVideoDto
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(250)]
        public string VideoUrl { get; set; }
        public Guid VendorId { get; set; }
    }
}

