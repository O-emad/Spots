using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.DTO
{
    public class VendorGalleryDto
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string FileName { get; set; }
        [StringLength(50)]
        public string Title { get; set; }
        public Guid VendorId { get; set; }
    }
}
