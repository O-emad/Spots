using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.DTO
{
    public class VendorVideoForCreationDto
    {
        [Required]
        [MaxLength(250)]
        public string VideoUrl { get; set; }
    }
}
